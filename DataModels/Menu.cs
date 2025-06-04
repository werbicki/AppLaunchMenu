using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Menu : Folder
    {
        public Menu(XmlDocument p_objXmlDocument, XmlNode p_objMenuNode)
            : base(p_objXmlDocument, p_objMenuNode)
        {
        }

        public Menu(XmlDocument p_objXmlDocument, string p_strName)
            : base(p_objXmlDocument, p_strName)
        {
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }
    }
}
