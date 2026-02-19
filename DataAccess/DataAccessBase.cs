using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.Media.Audio;

namespace AppLaunchMenu.DataModels
{
    public abstract class DataAccessBase : IComparable
    {
        public class DataChangedEventArgs : EventArgs
        {
            public DataChangedEventArgs()
            {
            }
        }

        public delegate void DataChangedEventHandler(object? sender, DataChangedEventArgs e);
        public event DataChangedEventHandler? DataChanged;

        protected virtual void OnDataChanged()
        {
            var eventHandler = DataChanged;
            if (eventHandler != null)
                eventHandler(this, new DataChangedEventArgs());
        }

        protected XmlDocument m_objXmlDocument;
        protected XmlNode? m_objXmlNode;
        protected bool m_blnIsDirty = false;

        protected DataAccessBase(XmlDocument p_objXmlDocument)
        {
            m_objXmlDocument = p_objXmlDocument;

            m_objXmlDocument.NodeChanged += XmlDocument_NodeChanged;
            m_objXmlDocument.NodeInserted += XmlDocument_NodeChanged;
            m_objXmlDocument.NodeRemoved += XmlDocument_NodeChanged;
        }

        protected void XmlDocument_NodeChanged(object sender, XmlNodeChangedEventArgs e)
        {
            IsDirty = true;
        }

        internal XmlDocument XmlDocument
        {
            get { return m_objXmlDocument; }
        }

        protected abstract string _ElementName
        {
            get;
        }

        protected XmlNode CreateNode()
        {
            return m_objXmlDocument.CreateNode("element", _ElementName, "");
        }

        internal virtual void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            throw new NotImplementedException();
        }

        internal virtual void Remove(DataModelBase p_objObject)
        {
            throw new NotImplementedException();
        }

        protected bool HasProperty(string p_strPropertyName)
        {
            if (m_objXmlNode != null
                && m_objXmlNode.Attributes != null
                && m_objXmlNode.Attributes[p_strPropertyName] != null
                )
                return !string.IsNullOrEmpty(m_objXmlNode.Attributes[p_strPropertyName]!.Value);

            return false;
        }

        protected string Property(string p_strPropertyName)
        {
            if (m_objXmlNode != null
                && m_objXmlNode.Attributes != null
                && m_objXmlNode.Attributes[p_strPropertyName] != null
                )
                return m_objXmlNode.Attributes[p_strPropertyName]!.Value;

            return "";
        }

        protected void Property(string p_strPropertyName, string value)
        {
            if ((m_objXmlNode != null)
                && (m_objXmlNode.Attributes != null)
                )
            {
                if ((m_objXmlNode.Attributes[p_strPropertyName] == null)
                    && (!string.IsNullOrEmpty(value))
                    )
                {
                    XmlAttribute objXmlAttribute = m_objXmlDocument.CreateAttribute(p_strPropertyName);

                    if (objXmlAttribute.Value != value)
                    {
                        objXmlAttribute.Value = value;
                        m_objXmlNode.Attributes.Append(objXmlAttribute);

                        IsDirty = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                        m_objXmlNode.Attributes.RemoveNamedItem(p_strPropertyName);
                    else
                        m_objXmlNode.Attributes[p_strPropertyName]!.Value = value;

                    IsDirty = true;
                }
            }
        }

        public bool IsDirty
        {
            get { return m_blnIsDirty; }
            internal set
            {
                m_blnIsDirty = value;
                OnDataChanged();
            }
        }

        public string Name
        {
            get { return Property(nameof(Name)); }
            set { Property(nameof(Name), value); }
        }

        protected string CData()
        {
            if ((m_objXmlDocument != null)
                && (m_objXmlDocument.ChildNodes.Count > 0)
                && (m_objXmlDocument.ChildNodes[0] is XmlCDataSection)
                )
            {
                XmlCDataSection? objCDataSection = m_objXmlDocument.ChildNodes[0] as XmlCDataSection;
                if (objCDataSection != null)
                {
                    string? strValue = objCDataSection.Value;
                    if (strValue != null)
                        return strValue;
                }
            }

            return "";
        }

        public virtual DataModelBase[] Items
        {
            get { return []; }
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
                        // ignored
                    }
                }

                return objGroups.Contains(p_strSecurityGroup);
            }

            return false;
        }
    }
}
