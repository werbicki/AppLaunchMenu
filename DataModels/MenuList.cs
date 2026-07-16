using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class MenuList : DataModelBase
    {
        DataModelCollection<Menu> m_objMenus;

        public MenuList(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, new Type[] { typeof(Menu), typeof(Environment) }, p_objMenuNode)
        {
            m_objMenus = new(this, null);

            UpdateItems();
        }

        public MenuList(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(Menu), typeof(Environment) }, p_strName)
        {
            m_objMenus = new(this, null);
        }

        internal static string ElementName
        {
            get { return nameof(MenuList); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        protected override void UpdateItems()
        {
            m_objMenus.Clear();

            foreach (DataModelBase objObject in Items)
            {
                if (objObject is Menu)
                    m_objMenus.Add((Menu)objObject);
            }
        }

        public Menu[] Menus
        {
            get { return m_objMenus.ToArray(); }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = XmlNode.SelectSingleNode("./" + Environment.ElementName);

                if (objEnvironmentNode == null)
                {
                    Environment objEnvironment = (Environment)CreateChildNode(typeof(Environment));

                    InsertItem(objEnvironment, 0);

                    return objEnvironment;
                }
                else
                    return new Environment(MenuFile, objEnvironmentNode);
            }
        }
    }
}
