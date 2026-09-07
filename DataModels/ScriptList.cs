using AppLaunchMenu.DataAccess;
using System;
using System.Linq;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class ScriptList : DataModelBase, IElementName
    {
        DataModelCollection<Script> m_objScripts;

        public ScriptList(LaunchMenuFile p_objMenuFile, XmlNode p_objScriptListNode)
            : base(p_objMenuFile, new Type[] { typeof(Script) }, p_objScriptListNode)
        {
            m_objScripts = new(this, null);

            UpdateItems();
        }

        public ScriptList(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(Script) }, p_strName)
        {
            m_objScripts = new(this, null);
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(ScriptList); }
        }

        protected override void UpdateItems()
        {
            m_objScripts.Clear();

            foreach (DataModelBase objObject in Items)
            {
                if (objObject is Script)
                    m_objScripts.Add((Script)objObject);
            }
        }

        public Script[] Scripts
        {
            get { return m_objScripts.ToArray(); }
        }

        public Script? GetScriptByName(string p_strConfig)
        {
            Script? objConfig = null;

            if (MenuFile != null)
            {
                XmlNode? objRoot = MenuFile.XmlDocument.DocumentElement;
                XmlNode? objConfigListNode = null;

                if (objRoot != null)
                    objConfigListNode = objRoot.SelectSingleNode("/" + LaunchMenuFile.ElementName + "/" + ScriptList.ElementName + "/" + Script.ElementName + "[@Name='" + p_strConfig + "']");

                if (objConfigListNode != null)
                    objConfig = new Script(MenuFile, objConfigListNode);
            }

            return objConfig;
        }

    }
}
