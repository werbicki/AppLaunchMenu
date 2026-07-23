using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Application : DataModelBase
    {
        public Application(MenuFile p_objMenuFile, XmlNode p_objApplicationNode)
            : base(p_objMenuFile, new Type[] { typeof(Environment) }, p_objApplicationNode)
        {
        }

        public Application(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(Environment) }, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(Application); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        public string ExecutablePath
        {
            get { return GetXmlAttribute(nameof(ExecutablePath)); }
            set { SetXmlAttribute(nameof(ExecutablePath), value); }
        }

        public string WorkingDirectory
        {
            get { return GetXmlAttribute(nameof(WorkingDirectory)); }
            set { SetXmlAttribute(nameof(WorkingDirectory), value); }
        }

        public string Parameters
        {
            get { return GetXmlAttribute(nameof(Parameters)); }
            set { SetXmlAttribute(nameof(Parameters), value); }
        }

        public string ConfigScript
        {
            get { return GetXmlAttribute(nameof(ConfigScript)); }
            set { SetXmlAttribute(nameof(ConfigScript), value); }
        }

        public string ConfigFilePath
        {
            get { return GetXmlAttribute(nameof(ConfigFilePath)); }
            set { SetXmlAttribute(nameof(ConfigFilePath), value); }
        }

        public bool Reservable
        {
            get { return GetXmlAttribute(nameof(Reservable)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(Reservable), value ? "true" : "false"); }
        }

        public string ReservationDescription
        {
            get { return GetXmlAttribute(nameof(ReservationDescription)); }
            set { SetXmlAttribute(nameof(ReservationDescription), value); }
        }

        public DateTimeOffset ReservationDate
        {
            get {
                try
                {
                    return DateTime.Parse(GetXmlAttribute(nameof(ReservationDate)));
                }
                catch
                { }

                return DateTime.Today;
            }
            set { SetXmlAttribute(nameof(ReservationDate), value.ToString("yyyy-MM-dd")); }
        }

        public string ReservationOwner
        {
            get { return GetXmlAttribute(nameof(ReservationOwner)); }
            set { SetXmlAttribute(nameof(ReservationOwner), value); }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = XmlNode.SelectSingleNode("./" + Environment.ElementName);
                if (objEnvironmentNode != null)
                    return new Environment(MenuFile, objEnvironmentNode);
                else
                {
                    Environment objEnvironment = NewItem<Environment>();

                    InsertItem(objEnvironment, 0);

                    return objEnvironment;
                }
            }
        }

        public string GetWorkingDirectory(Environment? p_objEnvironment)
        {
            if (!String.IsNullOrEmpty(WorkingDirectory))
            {
                string strDirectory = WorkingDirectory;
            
                if (p_objEnvironment != null)
                    strDirectory = p_objEnvironment.ExpandVariable(strDirectory);

                if (strDirectory.Length > 0)
                {
                    strDirectory = strDirectory.Replace("\\", "/");
                    if (!strDirectory.EndsWith("/"))
                        strDirectory += "/";
                }
                else
                    strDirectory = Directory.GetCurrentDirectory();

                return strDirectory;
            }

            return string.Empty;
        }

        public string GetExecutablePath(Environment? p_objEnvironment)
        {
            try
            {
                DirectoryInfo objWorkingDirectory = new(GetWorkingDirectory(p_objEnvironment));
                if (!String.IsNullOrEmpty(ExecutablePath))
                {
                    FileInfo objExecutable = new(ExecutablePath);

                    if (p_objEnvironment != null)
                        objExecutable = new(p_objEnvironment.ExpandVariable(ExecutablePath));

                    FileInfo objFullPath;

                    if (objExecutable.Exists)
                        objFullPath = new FileInfo(objExecutable.FullName);
                    else
                        objFullPath = new FileInfo(Path.Combine(objWorkingDirectory.FullName, objExecutable.Name));

                    return objFullPath.FullName;
                }
            }
            catch { }

            return string.Empty;
        }

        public string GetConfigFilePath(Environment? p_objEnvironment)
        {
            if (!String.IsNullOrEmpty(ConfigFilePath))
            {
                string strConfigFilePath = ConfigFilePath;

                if (p_objEnvironment != null)
                    strConfigFilePath = p_objEnvironment.ExpandVariable(strConfigFilePath);

                if (strConfigFilePath.Length > 0)
                    strConfigFilePath = strConfigFilePath.Replace("\\", "/");
                else
                    strConfigFilePath = string.Empty;

                return strConfigFilePath;
            }

            return string.Empty;
        }

        public string GetParameters(Environment? p_objEnvironment)
        {
            string strParameters = Parameters;

            if (p_objEnvironment != null)
                strParameters = p_objEnvironment.ExpandVariable(Parameters);

            return strParameters;
        }

        public void Execute(Environment p_objEnvironment)
        {
            FileInfo objFullPath = new(GetExecutablePath(p_objEnvironment));

            if (objFullPath.Exists)
            {
                DirectoryInfo objWorkingDirectory = new(GetWorkingDirectory(p_objEnvironment));

                if (objWorkingDirectory.Exists)
                {
                    string strConfig = p_objEnvironment.ExpandVariable(ConfigScript);
                    string strConfigFilePath = GetConfigFilePath(p_objEnvironment);

                    if ((!string.IsNullOrEmpty(strConfig))
                        && (!string.IsNullOrEmpty(strConfigFilePath))
                        )
                    {
                        Script? objConfig = MenuFile.ScriptList.GetScriptByName(strConfig);
                        if (objConfig != null)
                        {
                            Variable objConfigFilePathVariable = new(MenuFile, "ConfigFilePath");
                            objConfigFilePathVariable.Value = ConfigFilePath;
                            objConfigFilePathVariable.ExpandedValue = strConfigFilePath;

                            ScriptingHostConfigFile objScriptingHost = new(p_objEnvironment, strConfigFilePath);
                            objConfig.RunScriptVoid(objScriptingHost);
                            objScriptingHost.Close();

                            if (p_objEnvironment.Contains(objConfigFilePathVariable))
                                p_objEnvironment.RemoveItem(objConfigFilePathVariable);

                            p_objEnvironment.InsertItem(objConfigFilePathVariable, 0);
                        }
                    }

                    try
                    {
                        Process objProcess = new();
                        ProcessStartInfo objProcessStartInfo = new ProcessStartInfo(Name);

                        if (p_objEnvironment != null)
                        {
                            foreach (Variable objVariable in p_objEnvironment)
                            {
                                if (objProcessStartInfo.EnvironmentVariables.ContainsKey(objVariable.Name))
                                    objProcessStartInfo.EnvironmentVariables[objVariable.Name] = objVariable.ExpandedValue;
                                else
                                    objProcessStartInfo.EnvironmentVariables.Add(objVariable.Name, objVariable.ExpandedValue);
                            }

                            if (p_objEnvironment.Count > 0)
                                objProcessStartInfo.UseShellExecute = false;
                        }

                        if (WorkingDirectory.Length > 0)
                        {
                            if (objWorkingDirectory.Exists)
                                objProcessStartInfo.WorkingDirectory = objWorkingDirectory.FullName;
                            else
                            {
                                if (objFullPath.Directory != null)
                                    objProcessStartInfo.WorkingDirectory = objFullPath.Directory.FullName;
                            }
                        }

                        objProcessStartInfo.FileName = objFullPath.FullName;
                        objProcessStartInfo.Arguments = GetParameters(p_objEnvironment);

                        objProcess.StartInfo = objProcessStartInfo;
                        //blnResult = objProcess.Start();
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(e.Message);
                    }
                }
                else
                    throw new InvalidOperationException("Directory '" + objWorkingDirectory.FullName + "' does not exist!");
            }
            else
                throw new InvalidOperationException("File '" + objFullPath.FullName + "' does not exist!");
        }
    }
}
