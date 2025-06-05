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

        internal static string ElementName
        {
            get { return nameof(Variable); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        public string Description
        {
            get { return Property(nameof(Description)); }
            set { Property(nameof(Description), value); }
        }

        public string Group
        {
            get { return Property(nameof(Group)); }
            set { Property(nameof(Group), value); }
        }

        public string Value
        {
            get { return Property(nameof(Value)); }
            set { Property(nameof(Value), value); }
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
            get { return Property(nameof(Validation)); }
            set { Property(nameof(Validation), value); }
        }
    }
}