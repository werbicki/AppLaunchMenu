﻿using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class Application : DataModelBase
    {
        public Application(MenuFile p_objMenuFile, XmlNode p_objApplicationNode)
            : base(p_objMenuFile, p_objApplicationNode)
        {
        }

        public Application(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
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

        private Environment CreateEnvironment()
        {
            XmlElement objEnvironmentElement = m_objMenuFile.XmlDocument.CreateElement(Environment.ElementName);
            return new Environment(m_objMenuFile, objEnvironmentElement);
        }

        internal Variable? CreateVariable(String p_strVariableName)
        {
            XmlElement? objVariableElement = m_objMenuFile.XmlDocument.CreateElement(Variable.ElementName);
            XmlAttribute? objVariableNameAttribute = m_objMenuFile.XmlDocument.CreateAttribute("Name");
            if ((objVariableElement != null) && (objVariableNameAttribute != null))
            {
                objVariableNameAttribute.Value = p_strVariableName;
                objVariableElement.Attributes.Append(objVariableNameAttribute);

                return new Variable(m_objMenuFile, objVariableElement);
            }

            return null;
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

        public string SecurityGroup
        {
            get { return Property(nameof(SecurityGroup)); }
            set { Property(nameof(SecurityGroup), value); }
        }

        public bool CanAccess
        {
            get
            {
                //TODO: Checked for CanEdit and override to make visible
                return MemberOf(SecurityGroup);
            }
        }

        public string Executable
        {
            get { return Property(nameof(Executable)); }
            set { Property(nameof(Executable), value); }
        }

        public string WorkingDirectory
        {
            get { return Property(nameof(WorkingDirectory)); }
            set { Property(nameof(WorkingDirectory), value); }
        }

        public string Parameters
        {
            get { return Property(nameof(Parameters)); }
            set { Property(nameof(Parameters), value); }
        }

        public string Config
        {
            get { return Property(nameof(Config)); }
            set { Property(nameof(Config), value); }
        }

        public string ConfigFilePath
        {
            get { return Property(nameof(ConfigFilePath)); }
            set { Property(nameof(ConfigFilePath), value); }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = m_objXmlNode.SelectSingleNode("./" + Environment.ElementName);

                if (objEnvironmentNode == null)
                {
                    Environment objEnvironment = CreateEnvironment();

                    Insert(objEnvironment, 0);

                    return objEnvironment;
                }
                else
                    return new Environment(m_objMenuFile, objEnvironmentNode);
            }
        }

        public bool IsReservable
        {
            get { return Property(nameof(IsReservable)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { Property(nameof(IsReservable), value ? "true" : "false"); }
        }

        public string ReservationDescription
        {
            get { return Property(nameof(ReservationDescription)); }
            set { Property(nameof(ReservationDescription), value); }
        }

        public DateTimeOffset ReservationDate
        {
            get {
                try
                {
                    return DateTime.Parse(Property(nameof(ReservationDate)));
                }
                catch
                { }

                return DateTime.Today;
            }
            set { Property(nameof(ReservationDate), value.ToString("yyyy-MM-dd")); }
        }

        public string ReservationOwner
        {
            get { return Property(nameof(ReservationOwner)); }
            set { Property(nameof(ReservationOwner), value); }
        }

        public override DataModelBase[] Items
        {
            get
            {
                List<DataModelBase> objItems = [];

                if (m_objXmlNode != null)
                {
                    XmlNodeList? objNodes = m_objXmlNode.SelectNodes(Environment.ElementName);
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
                            {
                                if (objItemNode.Name == Environment.ElementName)
                                    objItems.Add(new Environment(m_objMenuFile, objItemNode));
                            }
                        }
                    }
                }

                return [.. objItems];
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
                if (!String.IsNullOrEmpty(Executable))
                {
                    FileInfo objExecutable = new(Executable);

                    if (p_objEnvironment != null)
                        objExecutable = new(p_objEnvironment.ExpandVariable(Executable));

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

        public bool Execute(Environment p_objEnvironment)
        {
            bool blnResult;

            FileInfo objFullPath = new(GetExecutablePath(p_objEnvironment));

            if (objFullPath.Exists)
            {
                DirectoryInfo objWorkingDirectory = new(GetWorkingDirectory(p_objEnvironment));

                if (objWorkingDirectory.Exists)
                {
                    string strConfig = p_objEnvironment.ExpandVariable(Config);
                    string strConfigFilePath = GetConfigFilePath(p_objEnvironment);

                    if ((!string.IsNullOrEmpty(strConfig))
                        && (!string.IsNullOrEmpty(strConfigFilePath))
                        )
                    {
                        Config? objConfig = m_objMenuFile.ConfigList.GetConfigByName(strConfig);
                        if (objConfig != null)
                        {
                            Variable objConfigFilePathVariable = new(m_objMenuFile, "ConfigFilePath");
                            objConfigFilePathVariable.Value = ConfigFilePath;
                            objConfigFilePathVariable.ExpandedValue = strConfigFilePath;

                            objConfig.WriteConfig(p_objEnvironment, strConfigFilePath);

                            if (p_objEnvironment.Contains(objConfigFilePathVariable))
                                p_objEnvironment.Remove(objConfigFilePathVariable);

                            p_objEnvironment.Insert(objConfigFilePathVariable, 0);
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
                        blnResult = objProcess.Start();
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

            return blnResult;
        }
    }
}
