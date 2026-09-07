using AppLaunchMenu.DataAccess;
using System;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Folder : DataModelBase, IElementName
    {
        public Folder(LaunchMenuFile p_objMenuFile, XmlNode p_objFolderNode)
            : base(p_objMenuFile, new Type[] { typeof(Folder), typeof(Environment), typeof(Application) }, p_objFolderNode)
        {
        }

        public Folder(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(Folder), typeof(Environment), typeof(Application) }, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(Folder); }
        }

        public Environment Environment
        {
            get { return GetItem<Environment>(); }
        }

        public bool Expanded
        {
            get { return GetXmlAttribute(nameof(Expanded)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(Expanded), value ? "true" : "false"); }
        }
    }
}
