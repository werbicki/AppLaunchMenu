using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class ScriptList : DataModelBase
    {
        ObservableCollection<Menu> m_objMenu = new ObservableCollection<Menu>();

        public ScriptList(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public ScriptList(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(ScriptList); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        public Script[] Configs
        {
            get
            {
                List<Script> objConfigs = [];

                if (m_objMenuFile != null)
                {
                    XmlNode? objRoot = m_objMenuFile.XmlDocument.DocumentElement;
                    XmlNodeList? objConfigListNode = null;

                    if (objRoot != null)
                        objConfigListNode = objRoot.SelectNodes("/" + MenuFile.ElementName + "/" + ScriptList.ElementName);

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
                                XmlNodeList? objNodes = objConfigNode.SelectNodes(Script.ElementName);

                                if (objNodes != null)
                                {
                                    foreach (XmlNode objNode in objNodes)
                                        objConfigs.Add(new Script(m_objMenuFile, objNode));
                                }
                            }
                        }
                    }
                }

                return [.. objConfigs];
            }
        }

        public Script? GetScriptByName(string p_strConfig)
        {
            Script? objConfig = null;

            if (m_objMenuFile != null)
            {
                XmlNode? objRoot = m_objMenuFile.XmlDocument.DocumentElement;
                XmlNode? objConfigListNode = null;

                if (objRoot != null)
                    objConfigListNode = objRoot.SelectSingleNode("/" + MenuFile.ElementName + "/" + ScriptList.ElementName + "/" + Script.ElementName + "[@Name='" + p_strConfig + "']");

                if (objConfigListNode != null)
                    objConfig = new Script(m_objMenuFile, objConfigListNode);
            }

            return objConfig;
        }
    }
}
