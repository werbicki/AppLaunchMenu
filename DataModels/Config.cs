using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using AppLaunchMenu.DataAccess;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace AppLaunchMenu.DataModels
{
    public class ScriptingHost
    {
        private Environment m_objEnvironment;
        private string m_strConfigFilePath;
        private StreamWriter m_objStreamWriter;

        internal ScriptingHost(Environment p_objEnvironment, string p_strConfigFilePath)
        {
            m_objEnvironment = p_objEnvironment;
            m_strConfigFilePath = p_strConfigFilePath;

            if (File.Exists(m_strConfigFilePath))
                File.Delete(m_strConfigFilePath);

            m_objStreamWriter = new StreamWriter(m_strConfigFilePath);
        }

        internal void Close()
        {
            m_objStreamWriter.Close();
        }

        public Environment Environment
        {
            get { return m_objEnvironment; }
        }

        public void WriteLine(string p_strLine)
        {
            m_objStreamWriter.WriteLine(p_strLine);
        }
    }

    public class Config : DataModelBase
    {
        public Config(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public Config(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(Config); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        protected string Language        
        {
            get { return Property(nameof(Language)); }
        }

        protected string Script
        {
            get { return CData(); }
        }

        public bool WriteConfig(Environment p_objEnvironment, string p_strConfigFilePath)
        {
            ScriptingHost objScriptingHost = new(p_objEnvironment, p_strConfigFilePath);

            string strCode = "void WriteConfig() { " + Script + "; } WriteConfig();";

            try
            {
                CSharpScript.EvaluateAsync(strCode, globals: objScriptingHost);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }

            objScriptingHost.Close();

            return false;
        }
    }
}
