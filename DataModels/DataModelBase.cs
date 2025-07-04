﻿using AppLaunchMenu.DataAccess;
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
    public abstract class DataModelBase : IComparable
    {
        protected MenuFile m_objMenuFile;
        protected XmlNode m_objXmlNode;

        protected DataModelBase(MenuFile p_objMenuFile, XmlNode p_objNode)
        {
            m_objMenuFile = p_objMenuFile;
            m_objXmlNode = p_objNode;
        }

        protected DataModelBase(MenuFile p_objMenuFile, string p_strName)
        {
            m_objMenuFile = p_objMenuFile;

            m_objXmlNode = CreateNode();

            if (!string.IsNullOrEmpty(p_strName))
            {
                XmlAttribute objNameAttribute = m_objMenuFile.XmlDocument.CreateAttribute("Name");
                objNameAttribute.Value = p_strName;
                m_objXmlNode.Attributes?.Append(objNameAttribute);
            }

            Name = p_strName;
        }

        internal XmlNode XmlNode
        {
            get { return m_objXmlNode; }
        }

        protected abstract string _ElementName
        {
            get;
        }

        protected XmlNode CreateNode()
        {
            return m_objMenuFile.XmlDocument.CreateNode("element", _ElementName, "");
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

        public string Name
        {
            get { return Property(nameof(Name)); }
            set { Property(nameof(Name), value); }
        }

        protected string CData()
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
