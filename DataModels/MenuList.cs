using AppLaunchMenu.DataAccess;
using System;
using System.Linq;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class MenuList : DataModelBase, IElementName
    {
        DataModelCollection<Menu> m_objMenus;

        public MenuList(LaunchMenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, new Type[] { typeof(Menu), typeof(Environment) }, p_objMenuNode)
        {
            m_objMenus = new(this, null);

            UpdateItems();
        }

        public MenuList(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(Menu), typeof(Environment) }, p_strName)
        {
            m_objMenus = new(this, null);
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(MenuList); }
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
            get { return GetItem<Environment>(); }
        }
    }
}
