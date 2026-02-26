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
    public interface IScriptingHost
    {
        public abstract Environment Environment
        {
            get;
        }

        public abstract void Exception(string p_strMessage);
        public abstract void WriteLine(string p_strLine);
    }

    public class ScriptingHost : IScriptingHost
    {
        private readonly Environment m_objEnvironment;

        internal ScriptingHost(Environment p_objEnvironment)
        {
            m_objEnvironment = p_objEnvironment;
        }

        public Environment Environment
        {
            get { return m_objEnvironment; }
        }

        public void Exception(string p_strMessage)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteLine(string p_strLine)
        {
            throw new NotImplementedException();
        }

        internal virtual void Close()
        {
            throw new NotImplementedException();
        }
    }

    public class ScriptingHostConfigFile : ScriptingHost
    {
        private readonly string m_strConfigFilePath;
        private readonly StreamWriter m_objStreamWriter;

        internal ScriptingHostConfigFile(Environment p_objEnvironment, string p_strConfigFilePath)
            : base(p_objEnvironment)
        {
            m_strConfigFilePath = p_strConfigFilePath;

            if (File.Exists(m_strConfigFilePath))
                File.Delete(m_strConfigFilePath);

            m_objStreamWriter = new StreamWriter(m_strConfigFilePath);
        }

        public override void WriteLine(string p_strLine)
        {
            m_objStreamWriter.WriteLine(p_strLine);
        }

        internal override void Close()
        {
            m_objStreamWriter.Close();
        }
    }

    public class Script : DataModelBase
    {
        public Script(MenuFile p_objMenuFile, XmlNode p_objMenuNode)
            : base(p_objMenuFile, p_objMenuNode)
        {
        }

        public Script(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(DataModels.Script); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        protected string Language        
        {
            get { return GetXmlAttribute(nameof(Language)); }
        }

        protected string Code
        {
            get { return GetXmlCData(); }
        }

        internal void RunScriptVoid(IScriptingHost p_objScriptingHost)
        {
            string strCode = "void RunScript() { " + Code + "; } RunScript();";

            try
            {
                CSharpScript.EvaluateAsync(strCode, globals: p_objScriptingHost);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        internal string RunScriptString(IScriptingHost p_objScriptingHost)
        {
            string strResult = string.Empty;
            string strCode = "string RunScript() { " + Code + "; } return RunScript();";

            try
            {
                object objResult = CSharpScript.EvaluateAsync(strCode, globals: p_objScriptingHost);
                strResult = objResult.ToString()!;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }

            return strResult;
        }
    }
}
