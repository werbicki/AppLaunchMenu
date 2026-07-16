using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Menu : Folder
    {
        public Menu(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public Menu(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal static new string ElementName
        {
            get { return nameof(Menu); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }
    }
}
