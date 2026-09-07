using AppLaunchMenu.DataAccess;
using System;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class DataCenter : DataModelBase, IElementName
    {
        public DataCenter(LaunchMenuFile p_objMenuFile, XmlNode p_objDataCenterNode)
            : base(p_objMenuFile, new Type[] { }, p_objDataCenterNode)
        {
        }

        public DataCenter(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { }, p_strName)
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
