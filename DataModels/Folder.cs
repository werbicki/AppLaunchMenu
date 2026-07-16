using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Folder : DataModelBase
    {
        public Folder(MenuFile p_objMenuFile, XmlNode p_objFolderNode)
            : base(p_objMenuFile, new Type[] { typeof(Folder), typeof(Environment), typeof(Application) }, p_objFolderNode)
        {
        }

        public Folder(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(Folder), typeof(Environment), typeof(Application) }, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(Folder); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        public bool Expanded
        {
            get { return GetXmlAttribute(nameof(Expanded)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(Expanded), value ? "true" : "false"); }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = XmlNode.SelectSingleNode("./" + Environment.ElementName);

                if (objEnvironmentNode == null)
                {
                    Environment objEnvironment = CreateItem<Environment>();

                    InsertItem(objEnvironment, 0);

                    return objEnvironment;
                }
                else
                    return new Environment(MenuFile, objEnvironmentNode);
            }
        }
    }
}
