using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Variable : DataModelBase
    {
        protected string m_strExpandedValue = "";

        public Variable(MenuFile p_objMenuFile, XmlNode p_objVariableNode)
            : base(p_objMenuFile, p_objVariableNode)
        {
        }

        public Variable(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
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
            get { return GetXmlAttribute(nameof(Description)); }
            set { SetXmlAttribute(nameof(Description), value); }
        }

        public string Group
        {
            get { return GetXmlAttribute(nameof(Group)); }
            set { SetXmlAttribute(nameof(Group), value); }
        }

        public string Value
        {
            get { return GetXmlAttribute(nameof(Value)); }
            set { SetXmlAttribute(nameof(Value), value); }
        }

        public bool Prompt
        {
            get { return GetXmlAttribute(nameof(Prompt)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(Prompt), value ? "true" : "false"); }
        }

        public string Validation
        {
            get { return GetXmlAttribute(nameof(Validation)); }
            set { SetXmlAttribute(nameof(Validation), value); }
        }

        public string ExpandedValue
        {
            get { return m_strExpandedValue; }
            set { m_strExpandedValue = value; }
        }
    }
}