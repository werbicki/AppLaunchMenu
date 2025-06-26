using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class ConfigList : DataModelBase
    {
        ObservableCollection<Menu> m_objMenu = new ObservableCollection<Menu>();

        public ConfigList(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public ConfigList(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(ConfigList); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        public Config[] Configs
        {
            get
            {
                List<Config> objConfigs = [];

                if (m_objMenuFile != null)
                {
                    XmlNode? objRoot = m_objMenuFile.XmlDocument.DocumentElement;
                    XmlNodeList? objConfigListNode = null;

                    if (objRoot != null)
                        objConfigListNode = objRoot.SelectNodes("/" + MenuFile.ElementName + "/" + ConfigList.ElementName);

                    if (objConfigListNode != null)
                    {
                        foreach (XmlNode objConfigNode in objConfigListNode)
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
                                XmlNodeList? objNodes = objConfigNode.SelectNodes(Config.ElementName);

                                if (objNodes != null)
                                {
                                    foreach (XmlNode objNode in objNodes)
                                        objConfigs.Add(new Config(m_objMenuFile, objNode));
                                }
                            }
                        }
                    }
                }

                return [.. objConfigs];
            }
        }

        public Config? GetConfigByName(string p_strConfig)
        {
            Config? objConfig = null;

            if (m_objMenuFile != null)
            {
                XmlNode? objRoot = m_objMenuFile.XmlDocument.DocumentElement;
                XmlNode? objConfigListNode = null;

                if (objRoot != null)
                    objConfigListNode = objRoot.SelectSingleNode("/" + MenuFile.ElementName + "/" + ConfigList.ElementName + "/" + Config.ElementName + "[@Name='" + p_strConfig + "']");

                if (objConfigListNode != null)
                    objConfig = new Config(m_objMenuFile, objConfigListNode);
            }

            return objConfig;
        }
    }
}
