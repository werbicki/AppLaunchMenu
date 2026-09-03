using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Windows.Devices.Power;
using Windows.Media.Audio;

namespace AppLaunchMenu.DataModels
{
    public abstract class DataModelBase : IComparable
    {
        private readonly LaunchMenuFile? m_objMenuFile;
        private readonly Type[] m_objXmlChildNodeTypes = [];
        private XmlNode m_objXmlNode;

        protected DataModelBase(Type[] p_objXmlChildNodeTypes, XmlDocument p_objXmlDocument)
        {
            m_objXmlChildNodeTypes = p_objXmlChildNodeTypes;
            m_objXmlNode = p_objXmlDocument;
        }

        protected DataModelBase(LaunchMenuFile p_objMenuFile, Type[] p_objXmlChildNodeTypes, XmlNode p_objXmlNode)
        {
            m_objMenuFile = p_objMenuFile;
            m_objXmlChildNodeTypes = p_objXmlChildNodeTypes;
            m_objXmlNode = p_objXmlNode;
        }

        protected DataModelBase(LaunchMenuFile p_objMenuFile, Type[] p_objXmlChildNodeTypes, string p_strName)
        {
            m_objMenuFile = p_objMenuFile;
            m_objXmlChildNodeTypes = p_objXmlChildNodeTypes;
            m_objXmlNode = CreateNode();

            if (!string.IsNullOrEmpty(p_strName))
            {
                XmlAttribute objNameAttribute = m_objMenuFile.XmlDocument.CreateAttribute("Name");
                objNameAttribute.Value = p_strName;
                m_objXmlNode.Attributes?.Append(objNameAttribute);
            }

            Name = p_strName;
        }

        internal void SetXmlNode(DataModelBase p_objObject, XmlNode p_objXmlNode)
        {
            if (p_objObject.GetType().IsSubclassOf(typeof(DataAccessBase)))
            {
                m_objXmlNode = p_objXmlNode;
                //UpdateItems();
            }
        }

        internal Type[] ChildNodeTypes
        {
            get { return m_objXmlChildNodeTypes; }
        }

        internal virtual LaunchMenuFile MenuFile
        {
            get
            {
                if (m_objMenuFile == null)
                    throw new AccessViolationException();

                return m_objMenuFile;
            }
        }

        internal XmlNode XmlNode
        {
            get { return m_objXmlNode; }
        }

        public bool HasEditAccess
        {
            get { return MenuFile.HasEditAccess; }
        }

        protected abstract string _ElementName
        {
            get;
        }

        protected XmlNode CreateNode()
        {
            return MenuFile.XmlDocument.CreateNode("element", _ElementName, "");
        }

        protected bool IsValidChildNodeType(Type p_objType)
        {
            foreach (Type objType in m_objXmlChildNodeTypes)
            {
                if (p_objType == objType)
                    return true;
            }

            return false;
        }

        protected Type? GetValidChildNodeType(DataModelBase p_objObject)
        {
            foreach (Type objType in m_objXmlChildNodeTypes)
            {
                if (p_objObject.GetType() == objType)
                    return objType;
            }

            return null;
        }

        protected Type? GetValidChildNodeType(XmlNode p_objXmlNode)
        {
            foreach (Type objType in m_objXmlChildNodeTypes)
            {
                if (p_objXmlNode.Name == objType.Name)
                    return objType;
            }

            return null;
        }

        internal Type? GetValidChildNodeType(string p_strTypeName)
        {
            foreach (Type objType in m_objXmlChildNodeTypes)
            {
                if (p_strTypeName == objType.Name)
                    return objType;
            }

            return null;
        }

        private DataModelBase CreateChildNode(Type p_objChildNodeType, string p_strName = "")
        {
            if ((XmlNode != null) && (IsValidChildNodeType(p_objChildNodeType)))
            {
                PropertyInfo? objPropertyInfo = p_objChildNodeType.GetProperty("ElementName", BindingFlags.Static | BindingFlags.NonPublic);
                string strElementName = "";

                if ((objPropertyInfo != null) && (objPropertyInfo.GetValue(null) != null))
                {
                    string? strProperty = (string?)objPropertyInfo.GetValue(null);

                    if (strProperty != null)
                        strElementName = strProperty;
                }

                XmlElement? objChildNodeElement = MenuFile.XmlDocument.CreateElement(strElementName);
                if (objChildNodeElement != null)
                {
                    if (p_strName != "")
                    {
                        XmlAttribute? objChildNodeNameAttribute = MenuFile.XmlDocument.CreateAttribute("Name");
                        if (objChildNodeNameAttribute != null)
                        {
                            objChildNodeNameAttribute.Value = p_strName;
                            objChildNodeElement.Attributes.Append(objChildNodeNameAttribute);
                        }
                    }

                    object[] arrConstructorArgs = new object[] { MenuFile, objChildNodeElement };
                    DataModelBase? objObject = (DataModelBase?)Activator.CreateInstance(p_objChildNodeType, arrConstructorArgs);

                    if (objObject != null)
                        return objObject;
                }
            }

            throw new ArgumentException();
        }

        internal protected T NewItem<T>(String p_strItemName = "") where T : DataModelBase
        {
            T objItem = (T)CreateChildNode(typeof(T), p_strItemName);

            return objItem;
        }

        internal protected object NewItem(Type p_objType, String p_strItemName = "")
        {
            object objItem = CreateChildNode(p_objType, p_strItemName);

            return objItem;
        }

        internal protected void DeleteItem<T>(T p_objItem) where T : DataModelBase
        {
        }

        public virtual DataModelBase[] Items
        {
            get
            {
                List<DataModelBase> objItems = [];

                if (XmlNode != null)
                {
                    XmlNodeList? objNodes = XmlNode.SelectNodes("*");
                    if (objNodes != null)
                    {
                        foreach (XmlNode objItemNode in objNodes)
                        {
                            bool blnInclude = true;

                            /*
                            if (!HostnameMatches(objItemNode))
                                    blnInclude = false;
                            */

                            if (blnInclude)
                            {
                                Type? objType = GetValidChildNodeType(objItemNode);
                                if (objType != null)
                                {
                                    object[] arrConstructorArgs = new object[] { MenuFile, objItemNode };
                                    DataModelBase? objObject = (DataModelBase?)Activator.CreateInstance(objType, arrConstructorArgs);

                                    if (objObject != null)
                                        objItems.Add(objObject);
                                }
                            }
                        }
                    }
                }

                return [.. objItems];
            }
        }

        virtual protected void UpdateItems()
        {
            //throw new NotImplementedException();
        }

        internal virtual void InsertItem(DataModelBase p_objObject, int p_intIndex = -1)
        {
            Type? objType = GetValidChildNodeType(p_objObject);

            if ((XmlNode != null) && (objType != null))
            {
                if (p_intIndex >= 0)
                    XmlNode.InsertBefore(p_objObject.XmlNode, XmlNode.ChildNodes[p_intIndex]);
                else
                    XmlNode.AppendChild(p_objObject.XmlNode);

                UpdateItems();
            }
            else
                throw new ArgumentException();
        }

        internal virtual void RemoveItem(DataModelBase p_objObject)
        {
            Type? objType = GetValidChildNodeType(p_objObject);

            if ((XmlNode != null) && (XmlNode.ParentNode != null) && (objType != null))
            {
                p_objObject.XmlNode.ParentNode?.RemoveChild(p_objObject.XmlNode);

                UpdateItems();
            }
            else
                throw new ArgumentException();
        }

        protected bool HasXmlAttribute(string p_strPropertyName)
        {
            if (m_objXmlNode != null
                && m_objXmlNode.Attributes != null
                && m_objXmlNode.Attributes[p_strPropertyName] != null
                )
                return !string.IsNullOrEmpty(m_objXmlNode.Attributes[p_strPropertyName]!.Value);

            return false;
        }

        protected string GetXmlAttribute(string p_strPropertyName)
        {
            if (m_objXmlNode != null
                && m_objXmlNode.Attributes != null
                && m_objXmlNode.Attributes[p_strPropertyName] != null
                )
                return m_objXmlNode.Attributes[p_strPropertyName]!.Value;

            return "";
        }

        protected bool GetXmlAttributeBool(string p_strPropertyName, bool p_blnEmptyIsTrue = false)
        {
            string strValue = GetXmlAttribute(p_strPropertyName);

            if (p_blnEmptyIsTrue)
            {
                return ((String.IsNullOrEmpty(strValue))
                    || (strValue.ToLower() == "true")
                    );
            }
            else
                return (strValue.ToLower() == "true");
        }

        protected void SetXmlAttribute(string p_strPropertyName, string value)
        {
            if ((m_objMenuFile != null)
                && (m_objXmlNode != null)
                && (m_objXmlNode.Attributes != null)
                )
            {
                if ((m_objXmlNode.Attributes[p_strPropertyName] == null)
                    && (!string.IsNullOrEmpty(value))
                    )
                {
                    XmlAttribute objXmlAttribute = m_objMenuFile.XmlDocument.CreateAttribute(p_strPropertyName);
                    objXmlAttribute.Value = value;
                    m_objXmlNode.Attributes.Append(objXmlAttribute);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        XmlElement objXmlElement = (XmlElement)m_objXmlNode;
                        objXmlElement.RemoveAttribute(p_strPropertyName);
                    }
                    else
                        m_objXmlNode.Attributes[p_strPropertyName]!.Value = value;
                }
            }
        }

        protected void SetXmlAttributeBool(string p_strPropertyName, bool value, bool p_blnEmptyIsTrue = false)
        {
            if (p_blnEmptyIsTrue)
                SetXmlAttribute(p_strPropertyName, value ? "" : "False");
            else
                SetXmlAttribute(p_strPropertyName, value ? "True" : "");
        }


        protected string GetXmlCData()
        {
            if ((m_objXmlNode != null)
                && (m_objXmlNode.ChildNodes.Count > 0)
                && (m_objXmlNode.ChildNodes[0] is XmlCDataSection)
                )
            {
                XmlCDataSection? objCDataSection = m_objXmlNode.ChildNodes[0] as XmlCDataSection;
                if (objCDataSection != null)
                {
                    string? strValue = objCDataSection.Value;
                    if (strValue != null)
                        return strValue;
                }
            }

            return "";
        }

        protected void SetXmlCData(string p_strXmlCData)
        {
            if ((m_objXmlNode != null)
                && (m_objXmlNode.ChildNodes.Count > 0)
                && (m_objXmlNode.ChildNodes[0] is XmlCDataSection)
                )
            {
                XmlCDataSection? objCDataSection = m_objXmlNode.ChildNodes[0] as XmlCDataSection;
                if (objCDataSection != null)
                    objCDataSection.Value = p_strXmlCData;
            }
        }

        public string Name
        {
            get { return GetXmlAttribute(nameof(Name)); }
            set { SetXmlAttribute(nameof(Name), value); }
        }

        public string SecurityGroup
        {
            get { return GetXmlAttribute(nameof(SecurityGroup)); }
            set { SetXmlAttribute(nameof(SecurityGroup), value); }
        }

        public bool Accessible
        {
            get { return MemberOf(SecurityGroup); }
        }

        public bool Enabled
        {
            get { return GetXmlAttributeBool(nameof(Enabled), true); }
            set { SetXmlAttributeBool(nameof(Enabled), value, true); }
        }

        public string Username
        {
            get { return GetXmlAttribute(nameof(Username)); }
            set { SetXmlAttribute(nameof(Username), value); }
        }

        public string Hostname
        {
            get { return GetXmlAttribute(nameof(Hostname)); }
            set { SetXmlAttribute(nameof(Hostname), value); }
        }

        public string Subnet
        {
            get { return GetXmlAttribute(nameof(Subnet)); }
            set { SetXmlAttribute(nameof(Subnet), value); }
        }

        public string DataCenter
        {
            get { return GetXmlAttribute(nameof(DataCenter)); }
            set { SetXmlAttribute(nameof(DataCenter), value); }
        }

        private bool Matches(string p_strPattern, string[] p_strValues)
        {
            bool blnResult = false;

            foreach (string strValue in p_strValues)
                blnResult = blnResult || Matches(p_strPattern, strValue);

            return blnResult;
        }

        private bool Matches(string p_strPattern, string p_strValue)
        {
            bool blnResult = true;

            if (!string.IsNullOrWhiteSpace(p_strPattern))
            {
                if (p_strPattern == p_strValue)
                    blnResult = true;
                else
                {
                    try
                    {
                        Regex objRegex = new Regex(p_strPattern, RegexOptions.None);

                        // Assume it is a Regex and try to match first
                        blnResult = objRegex.IsMatch(p_strValue);
                    }
                    catch (ArgumentException)
                    {
                        blnResult = false;
                    }
                }
            }

            return blnResult;
        }

        public bool IsVisible
        {
            get
            {
                return Enabled
                    && Matches(Username, MenuFile.LocalUsername)
                    && Matches(Hostname, MenuFile.LocalHostname)
                    && Matches(Subnet, MenuFile.LocalIpAddress.ToString())
                    && Matches(DataCenter, MenuFile.LocalDataCenter)
                    ;
            }
        }

        public int CompareTo(object? p_objObject)
        {
            if (p_objObject != null
                && p_objObject.GetType() == typeof(DataModelBase)
                )
            {
                DataModelBase objDataModelBase = (DataModelBase)p_objObject;
                return Name.CompareTo(objDataModelBase.Name);
            }

            throw new ArgumentException("p_objObject is not a ConfigNode");
        }

        public bool MemberOf(string p_strSecurityGroup)
        {
            List<string> objGroups = [];

            WindowsIdentity objWindowsIdentity = WindowsIdentity.GetCurrent();
            if (objWindowsIdentity.Groups != null)
            {
                foreach (var group in objWindowsIdentity.Groups)
                {
                    try
                    {
                        objGroups.Add(group.Translate(typeof(NTAccount)).ToString());
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                }

                return objGroups.Contains(p_strSecurityGroup);
            }

            return false;
        }
    }
}
