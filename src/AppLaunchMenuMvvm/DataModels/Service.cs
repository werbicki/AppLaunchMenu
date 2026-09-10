using AppLaunchMenu.DataAccess;
using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Service : DataModelBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        {
            get
            {
                return
                [
                    new() { Type = typeof(Server), ElementType = ElementTypeEnum.OneOrMore },
                ];
            }
        }

        public Service(LaunchMenuFile p_objMenuFile, XmlNode p_objServiceNode)
            : base(p_objMenuFile, p_objServiceNode)
        {
        }

        public Service(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(Service); }
        }

        public Collection<Server> Servers
        {
            get { return GetItems<Server>(); }
        }

        public string ExecutablePath
        {
            get { return GetXmlAttribute(nameof(ExecutablePath)); }
            set { SetXmlAttribute(nameof(ExecutablePath), value); }
        }

        public string WorkingDirectory
        {
            get { return GetXmlAttribute(nameof(WorkingDirectory)); }
            set { SetXmlAttribute(nameof(WorkingDirectory), value); }
        }

        public string ConfigScript
        {
            get { return GetXmlAttribute(nameof(ConfigScript)); }
            set { SetXmlAttribute(nameof(ConfigScript), value); }
        }

        public string ConfigFilePath
        {
            get { return GetXmlAttribute(nameof(ConfigFilePath)); }
            set { SetXmlAttribute(nameof(ConfigFilePath), value); }
        }

    }
}
