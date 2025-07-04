﻿using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataAccess
{
    public class MenuFile : DataAccessBase
    {
        string? m_strFilename;

        public MenuFile()
            : base(new XmlDocument())
        {
            CreateFile("New LaunchMenu");
        }

        public MenuFile(string p_strFilename)
            : base(new XmlDocument())
        {
            FileInfo objFileInfo = new FileInfo(p_strFilename);

            if (objFileInfo.Exists)
            {
                ReadFile(p_strFilename);
                m_strFilename = objFileInfo.FullName;
            }
            else
            {
                CreateFile(p_strFilename);
                m_strFilename = p_strFilename;
            }
        }

        internal static string ElementName
        {
            get { return "AppLaunchMenu"; }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        public void Load()
        {
            if (m_strFilename != null)
            {
                FileInfo objFileInfo = new FileInfo(m_strFilename);

                if (objFileInfo.Exists)
                    ReadFile(m_strFilename);
            }
        }

        public void Save()
        {
            if (m_strFilename != null)
            {
                try
                {
                    m_objXmlDocument.Save(m_strFilename);
                }
                catch (XmlException e)
                {
                    throw new Exception("Unable to Load Menu file '" + m_strFilename + "'\n\n" + e.Message, e);
                }
            }
        }

        public void Reload()
        {
            if (m_strFilename != null)
            {
                FileInfo objFileInfo = new FileInfo(m_strFilename);

                if (objFileInfo.Exists)
                    ReadFile(m_strFilename);
            }
        }

        protected bool CreateFile(string p_strDocument)
        {
            XmlDocument? objDocument = null;

            try
            {
                objDocument ??= new XmlDocument();

                XmlElement objLaunchMenuElement = objDocument.CreateElement(ElementName);
                XmlElement objMenusElement = objDocument.CreateElement(MenuList.ElementName);
                objLaunchMenuElement.AppendChild(objMenusElement);
                objDocument.AppendChild(objLaunchMenuElement);
            }
            catch (Exception)
            {
            }

            if (objDocument != null)
            {
                m_strFilename = p_strDocument;
                m_objXmlDocument = objDocument;

                XmlNode? objNode = m_objXmlDocument.SelectSingleNode(ElementName);
                if (objNode != null)
                    m_objXmlNode = objNode;
            }
            else
                throw new Exception("Unable to create new Menu file '" + p_strDocument + "'");

            return true;
        }

        protected bool ReadFile(string p_strFilename)
        {
            FileInfo objFileInfo = new(p_strFilename);
            XmlDocument? objDocument = null;

            if (objFileInfo.Exists)
            {
                bool blnLoaded = false;
                int intRetries = 3;

                while (intRetries > 0)
                {
                    try
                    {
                        objDocument ??= new XmlDocument();

                        objDocument.Load(objFileInfo.FullName);

                        blnLoaded = true;
                        intRetries = 0;
                    }
                    catch (XmlException e)
                    {
                        throw new Exception("Unable to Load Menu file '" + p_strFilename + "'\n\n" + e.Message, e);
                    }
                    catch (Exception)
                    {
                        intRetries--;
                        System.Threading.Thread.Sleep(500);
                    }
                }

                if (blnLoaded && objDocument != null)
                {
                    m_objXmlDocument = objDocument;

                    XmlNode? objNode = m_objXmlDocument.SelectSingleNode(ElementName);
                    if (objNode != null)
                        m_objXmlNode = objNode;

                    return true;
                }
                else
                    throw new Exception("Unable to Load Menu file '" + p_strFilename + "'");
            }

            return false;
        }

        internal Menu? AddMenu(string p_strMenuName)
        {
            if (m_objXmlDocument != null)
            {
                XmlNode? objRoot = m_objXmlDocument.DocumentElement;
                XmlNodeList? objMenusNode = null;

                if (objRoot != null)
                    objMenusNode = objRoot.SelectNodes("/" + ElementName + "/" + MenuList.ElementName);

                if (objMenusNode != null)
                {
                    XmlElement objMenuElement = m_objXmlDocument.CreateElement(Menu.ElementName);
                    XmlAttribute objMenuNameAttribute = m_objXmlDocument.CreateAttribute("Name");
                    objMenuNameAttribute.Value = p_strMenuName;
                    objMenuElement.Attributes.Append(objMenuNameAttribute);
                    objMenusNode[0]?.AppendChild(objMenuElement);

                    return new Menu(this, objMenuElement);
                }
            }

            return null;
        }

        internal void RemoveMenu(Menu p_objMenu)
        {
            if (m_objXmlDocument != null)
            {
                p_objMenu.XmlNode?.ParentNode?.RemoveChild(p_objMenu.XmlNode);
            }
        }

        internal override void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            if (p_objObject is MenuList)
            {
                if (p_intIndex >= 0)
                    m_objXmlNode?.InsertBefore(p_objObject.XmlNode, m_objXmlNode?.ChildNodes[p_intIndex]);
                else
                    m_objXmlNode?.AppendChild(p_objObject.XmlNode);
            }
            else
                throw new ArgumentException();
        }

        private MenuList CreateMenuList()
        {
            XmlElement objEnvironmentElement = m_objXmlDocument.CreateElement(MenuList.ElementName);
            return new MenuList(this, objEnvironmentElement);
        }

        private ConfigList CreateConfigList()
        {
            XmlElement objEnvironmentElement = m_objXmlDocument.CreateElement(ConfigList.ElementName);
            return new ConfigList(this, objEnvironmentElement);
        }

        public MenuList MenuList
        {
            get
            {
                XmlNode? objMenuListNode = m_objXmlNode?.SelectSingleNode("/" + ElementName + "/" + MenuList.ElementName);

                if (objMenuListNode == null)
                {
                    MenuList objMenuList = CreateMenuList();

                    Insert(objMenuList, 0);

                    return objMenuList;
                }
                else
                    return new MenuList(this, objMenuListNode);
            }
        }

        public ConfigList ConfigList
        {
            get
            {
                XmlNode? objConfigListNode = m_objXmlNode?.SelectSingleNode("/" + ElementName + "/" + ConfigList.ElementName);

                if (objConfigListNode == null)
                {
                    ConfigList objConfigList = CreateConfigList();

                    Insert(objConfigList, 0);

                    return objConfigList;
                }
                else
                    return new ConfigList(this, objConfigListNode);
            }
        }

        public string SecurityGroup
        {
            get { return Property(nameof(SecurityGroup)); }
            set { Property(nameof(SecurityGroup), value); }
        }

        public bool CanEdit
        {
            get { return MemberOf(SecurityGroup); }
        }
    }
}
