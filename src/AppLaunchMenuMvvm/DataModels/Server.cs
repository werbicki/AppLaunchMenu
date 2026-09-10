using AppLaunchMenu.DataAccess;
using System;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Server : DataModelBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        {
            get { return []; }
        }

        public Server(LaunchMenuFile p_objMenuFile, XmlNode p_objServerNode)
            : base(p_objMenuFile, p_objServerNode)
        {
        }

        public Server(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(Server); }
        }

        public string ServiceUsername
        {
            get { return GetXmlAttribute(nameof(ServiceUsername)); }
            set { SetXmlAttribute(nameof(ServiceUsername), value); }
        }
    }
}
