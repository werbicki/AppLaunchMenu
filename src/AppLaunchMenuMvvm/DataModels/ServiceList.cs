using AppLaunchMenu.DataAccess;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class ServiceList : DataModelBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        {
            get
            {
                return
                [
                    new() { Type = typeof(NetworkDrive), ElementType = ElementTypeEnum.OneOrMore }
                ];
            }
        }

        public ServiceList(LaunchMenuFile p_objMenuFile, XmlNode p_objFolderNode)
            : base(p_objMenuFile, p_objFolderNode)
        {
        }

        public ServiceList(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(ServiceList); }
        }

        public Collection<Service> Services
        {
            get { return GetItems<Service>(); }
        }
    }
}
