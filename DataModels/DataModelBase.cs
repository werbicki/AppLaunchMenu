using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Windows.Media.Audio;

namespace AppLaunchMenu.DataModels
{
    public abstract class DataModelBase : IComparable
    {
        protected XmlDocument m_objXmlDocument;
        protected XmlNode m_objXmlNode;

        protected DataModelBase(XmlDocument p_objXmlDocument, XmlNode p_objNode)
        {
            m_objXmlDocument = p_objXmlDocument;
            m_objXmlNode = p_objNode;
        }

        protected DataModelBase(XmlDocument p_objXmlDocument, string p_strName)
        {
            m_objXmlDocument = p_objXmlDocument;

            m_objXmlNode = CreateNode();

            if (!string.IsNullOrEmpty(p_strName))
            {
                XmlAttribute objNameAttribute = m_objXmlDocument.CreateAttribute("Name");
                objNameAttribute.Value = p_strName;
                m_objXmlNode.Attributes?.Append(objNameAttribute);
            }

            Name = p_strName;
        }

        protected abstract string ElementName
        {
            get;
        }

        protected XmlNode CreateNode()
        {
            return m_objXmlDocument.CreateNode("element", ElementName, "");
        }

        internal virtual void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            throw new NotImplementedException();
        }

        internal virtual void Remove(DataModelBase p_objObject)
        {
            throw new NotImplementedException();
        }

        internal XmlNode XmlNode
        {
            get { return m_objXmlNode; }
        }

        public string Name
        {
            get
            {
                if (m_objXmlNode != null
                    && m_objXmlNode.Attributes != null
                    && m_objXmlNode.Attributes["Name"] != null
                    )
                    return m_objXmlNode.Attributes["Name"]!.Value;

                return "";
            }
            set
            {
                if ((m_objXmlDocument != null)
                    && (m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    )
                {
                    if (m_objXmlNode.Attributes["Name"] == null)
                    {
                        XmlAttribute objXmlAttribute = m_objXmlDocument.CreateAttribute("Name");
                        objXmlAttribute.Value = value;
                        m_objXmlNode.Attributes.Append(objXmlAttribute);
                    }
                    else
                        m_objXmlNode.Attributes["Name"]!.Value = value;
                }
            }
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
    }
}
