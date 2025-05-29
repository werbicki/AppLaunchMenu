using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Variable : DataModelBase
    {
        protected string m_strExpandedValue = "";

        public Variable(XmlDocument p_objXmlDocument, XmlNode p_objVariableNode)
            : base(p_objXmlDocument, p_objVariableNode)
        {
        }

        public Variable(XmlDocument p_objXmlDocument, string p_strName)
            : base(p_objXmlDocument, p_strName)
        {
        }

        protected override string ElementName
        {
            get { return nameof(Variable); }
        }

        public string Description
        {
            get
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Description"] != null)
                    )
                    return m_objXmlNode.Attributes["Description"]!.Value;

                return "";
            }
            set
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Description"] != null)
                    )
                    m_objXmlNode.Attributes["Description"]!.Value = value;
            }
        }

        public string Group
        {
            get
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null) 
                    && (m_objXmlNode.Attributes["Group"] != null)
                    )
                    return m_objXmlNode.Attributes["Group"]!.Value;

                return "";
            }
            set
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Group"] != null)
                    )
                    m_objXmlNode.Attributes["Group"]!.Value = value;
            }
        }

        public string Value
        {
            get
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Value"] != null)
                    )
                    return m_objXmlNode.Attributes["Value"]!.Value;

                return "";
            }
            set
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Value"] != null)
                    )
                    m_objXmlNode.Attributes["Value"]!.Value = value;
            }
        }

        public string ExpandedValue
        {
            get
            {
                return m_strExpandedValue;
            }
            set
            {
                m_strExpandedValue = value;
            }
        }

        public string Validation
        {
            get
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Validation"] != null)
                    )
                    return m_objXmlNode.Attributes["Validation"]!.Value;

                return "";
            }
            set
            {
                if ((m_objXmlNode != null)
                    && (m_objXmlNode.Attributes != null)
                    && (m_objXmlNode.Attributes["Validation"] != null)
                    )
                    m_objXmlNode.Attributes["Validation"]!.Value = value;
            }
        }
    }
}