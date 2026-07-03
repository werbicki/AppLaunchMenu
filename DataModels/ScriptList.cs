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
        DataModelCollection<Script> m_objScripts;

        public ScriptList(MenuFile p_objMenuFile, XmlNode p_objScriptListNode)
            : base(p_objMenuFile, p_objScriptListNode)
        {
            m_objScripts = new(p_objMenuFile, null);

            Initialize(m_objMenuFile.XmlDocument, m_objMenuFile.XmlDocument, XmlNode.ParentNode);
        }

        public ScriptList(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
            m_objScripts = new(p_objMenuFile, null);
        }

        private void Initialize(XmlDocument p_objDocument, XmlNode? p_objReferenceNode, XmlNode? p_objParent)
        {
            m_objScripts.Clear();

            List<XmlNode> objIncludedNodes = [];
            XmlNodeList? objNodeList = XmlNode.SelectNodes("./" + Script.ElementName);
            Initialize(objIncludedNodes, objNodeList);

            m_objScripts = new DataModelCollection<Script>(m_objMenuFile, objIncludedNodes);
        }

        internal static string ElementName
        {
            get { return nameof(ScriptList); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        internal override void Insert(DataModelBase p_objObject, int p_intIndex)
        {
            if (p_objObject is Script)
            {
                if (p_intIndex >= 0)
                    m_objXmlNode?.InsertBefore(p_objObject.XmlNode, m_objXmlNode?.ChildNodes[p_intIndex]);
                else
                    m_objXmlNode?.AppendChild(p_objObject.XmlNode);

                Initialize(m_objMenuFile.XmlDocument, m_objMenuFile.XmlDocument, XmlNode.ParentNode);
            }
            else
                throw new ArgumentException();
        }

        internal override void Remove(DataModelBase p_objObject)
        {
            if (p_objObject is Script)
                p_objObject.XmlNode?.ParentNode?.RemoveChild(p_objObject.XmlNode);
            else
                throw new ArgumentException();
        }

        public override DataModelBase[] Items
        {
            get
            {
                List<DataModelBase> objItems = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(Script.ElementName);
                    if (objNodes != null)
                    {
                        foreach (XmlNode objItemNode in objNodes)
                        {
                            bool blnInclude = true;

                            /*
                            if (!HostnameMatches(objItemNode))
                                    blnInclude = false;
                            */

                            if (blnInclude)
                                objItems.Add(new Script(m_objMenuFile, objItemNode));
                        }
                    }
                }

                return [.. objItems];
            }
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
