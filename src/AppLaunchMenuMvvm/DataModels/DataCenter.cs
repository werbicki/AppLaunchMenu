using AppLaunchMenu.DataAccess;
using System;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class DataCenter : DataModelBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        {
            get { return []; }
        }

        public DataCenter(LaunchMenuFile p_objMenuFile, XmlNode p_objDataCenterNode)
            : base(p_objMenuFile, p_objDataCenterNode)
        {
        }

        public DataCenter(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(DataCenter); }
        }
    }
}
