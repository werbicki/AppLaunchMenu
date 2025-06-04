using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class MenuList : DataModelBase
    {
        ObservableCollection<Menu> m_objMenu = new ObservableCollection<Menu>();

        public MenuList(XmlDocument p_objXmlDocument, XmlNode p_objMenuNode)
            : base(p_objXmlDocument, p_objMenuNode)
        {
        }

        public MenuList(XmlDocument p_objXmlDocument, string p_strName)
            : base(p_objXmlDocument, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(MenuList); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        private Environment CreateEnvironment()
        {
            XmlElement objEnvironmentElement = m_objXmlDocument.CreateElement(Environment.ElementName);
            return new Environment(m_objXmlDocument, objEnvironmentElement);
        }

        internal override void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            if (p_objObject is Environment)
            {
                if (p_intIndex >= 0)
                    m_objXmlNode?.InsertBefore(p_objObject.XmlNode, m_objXmlNode?.ChildNodes[p_intIndex]);
                else
                    m_objXmlNode?.AppendChild(p_objObject.XmlNode);
            }
            else
                throw new ArgumentException();
        }

        public Menu[] Menus
        {
            get
            {
                List<Menu> objMenus = [];

                if (m_objXmlDocument != null)
                {
                    XmlNode? objRoot = m_objXmlDocument.DocumentElement;
                    XmlNodeList? objMenusNode = null;

                    if (objRoot != null)
                        objMenusNode = objRoot.SelectNodes("/" + MenuFile.ElementName + "/" + MenuList.ElementName);

                    if (objMenusNode != null)
                    {
                        foreach (XmlNode objMenuNode in objMenusNode)
                        {
                            bool blnInclude = true;

                            /*
                            if ((!DomainMatches(objMenuNode))
                                || (!DataCenterMatches(objMenuNode))
                                || (!HostnameMatches(objMenuNode))
                                )
                                blnInclude = false;
                                */

                            if (blnInclude)
                            {
                                XmlNodeList? objNodes = objMenuNode.SelectNodes(Menu.ElementName);

                                if (objNodes != null)
                                {
                                    foreach (XmlNode objNode in objNodes)
                                        objMenus.Add(new Menu(m_objXmlDocument, objNode));
                                }
                            }
                        }
                    }
                }

                return [.. objMenus];
            }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = m_objXmlNode.SelectSingleNode("/" + MenuFile.ElementName + "/" + _ElementName + "/Environment");

                if (objEnvironmentNode == null)
                {
                    Environment objEnvironment = CreateEnvironment();

                    Insert(objEnvironment, 0);

                    return objEnvironment;
                }
                else
                    return new Environment(m_objXmlDocument, objEnvironmentNode);
            }
        }
    }
}
