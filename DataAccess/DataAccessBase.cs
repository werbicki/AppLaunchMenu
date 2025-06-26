using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.Media.Audio;

namespace AppLaunchMenu.DataModels
{
    public abstract class DataAccessBase : IComparable
    {
        protected XmlDocument m_objXmlDocument;
        protected XmlNode? m_objXmlNode;

        protected DataAccessBase(XmlDocument p_objXmlDocument)
        {
            m_objXmlDocument = p_objXmlDocument;
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
            if (m_objXmlDocument != null
                && m_objXmlDocument.Attributes != null
                && m_objXmlDocument.Attributes[p_strPropertyName] != null
                )
                return !string.IsNullOrEmpty(m_objXmlDocument.Attributes[p_strPropertyName]!.Value);

            return false;
        }

        protected string Property(string p_strPropertyName)
        {
            if (m_objXmlDocument != null
                && m_objXmlDocument.Attributes != null
                && m_objXmlDocument.Attributes[p_strPropertyName] != null
                )
                return m_objXmlDocument.Attributes[p_strPropertyName]!.Value;

            return "";
        }

        protected void Property(string p_strPropertyName, string value)
        {
            if ((m_objXmlDocument != null)
                && (m_objXmlDocument.Attributes != null)
                )
            {
                if ((m_objXmlDocument.Attributes[p_strPropertyName] == null)
                    && (!string.IsNullOrEmpty(value))
                    )
                {
                    XmlAttribute objXmlAttribute = m_objXmlDocument.CreateAttribute(p_strPropertyName);
                    objXmlAttribute.Value = value;
                    m_objXmlDocument.Attributes.Append(objXmlAttribute);
                }
                else
                {
                    if (string.IsNullOrEmpty(value))
                        m_objXmlDocument.DocumentElement?.RemoveAttribute(p_strPropertyName);
                    else
                        m_objXmlDocument.Attributes[p_strPropertyName]!.Value = value;
                }
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
