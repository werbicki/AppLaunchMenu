using AppLaunchMenu.DataAccess;
using System;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Server : DataModelBase, IElementName
    {
        public Server(LaunchMenuFile p_objMenuFile, XmlNode p_objServerNode)
            : base(p_objMenuFile, new Type[] { }, p_objServerNode)
        {
        }

        public Server(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { }, p_strName)
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
