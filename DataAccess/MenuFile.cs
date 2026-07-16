using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Environment = AppLaunchMenu.DataModels.Environment;

namespace AppLaunchMenu.DataAccess
{
    public class MenuFile : DataAccessBase
    {
        public delegate void FileChangedEventHandler(object? sender, DataChangedEventArgs e);
        public event FileChangedEventHandler? FileChanged;

        protected virtual void OnFileChanged()
        {
            var eventHandler = FileChanged;
            if (eventHandler != null)
                eventHandler(this, new DataChangedEventArgs());
        }

        private string? m_strFilename;
        private FileSystemWatcher m_objFileSystemWatcher = new FileSystemWatcher();

        public MenuFile()
            : base(new XmlDocument())
        {
            CreateFile("New AppLaunchMenu");
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

        public String Filename
        {
            get
            {
                if (m_strFilename != null)
                {
                    FileInfo objFileInfo = new FileInfo(m_strFilename);
                    return objFileInfo.Name;
                }
                return "Menu";
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
                    XmlDocument.Save(m_strFilename);
                    IsDirty = false;
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
            try
            {
                XmlElement objLaunchMenuElement = XmlDocument.CreateElement(ElementName);
                XmlElement objMenusElement = XmlDocument.CreateElement(MenuList.ElementName);
                objLaunchMenuElement.AppendChild(objMenusElement);
                XmlDocument.AppendChild(objLaunchMenuElement);
            }
            catch (Exception)
            {
            }

            if (XmlDocument != null)
            {
                m_strFilename = p_strDocument;

                XmlNode? objXmlNode = XmlDocument.SelectSingleNode(ElementName);
                if (objXmlNode != null)
                    SetXmlNode(this, objXmlNode);
            }
            else
                throw new Exception("Unable to create new Menu file '" + p_strDocument + "'");

            return true;
        }

        protected bool ReadFile(string p_strFilename)
        {
            FileInfo objFileInfo = new(p_strFilename);

            if (objFileInfo.Exists)
            {
                bool blnLoaded = false;
                int intRetries = 3;

                XmlDocument.NodeChanged -= XmlDocument_NodeChanged;
                XmlDocument.NodeInserted -= XmlDocument_NodeChanged;
                XmlDocument.NodeRemoved -= XmlDocument_NodeChanged;

                while (intRetries > 0)
                {
                    try
                    {
                        XmlDocument.Load(objFileInfo.FullName);

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

                if (blnLoaded)
                {
                    XmlDocument.NodeChanged += XmlDocument_NodeChanged;
                    XmlDocument.NodeInserted += XmlDocument_NodeChanged;
                    XmlDocument.NodeRemoved += XmlDocument_NodeChanged;

                    XmlNode? objXmlNode = XmlDocument.SelectSingleNode(ElementName);
                    if (objXmlNode != null)
                        SetXmlNode(this, objXmlNode);

                    String? strPath = Path.GetDirectoryName(objFileInfo.FullName);
                    if (strPath != null)
                        m_objFileSystemWatcher.Path = strPath;

                    m_objFileSystemWatcher.Filter = Path.GetFileName(objFileInfo.FullName);

                    m_objFileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
                    m_objFileSystemWatcher.Changed += FileSystemWatcher_Changed;
                    m_objFileSystemWatcher.EnableRaisingEvents = true;

                    IsDirty = false;

                    return true;
                }
                else
                    throw new Exception("Unable to Load Menu file '" + p_strFilename + "'");
            }

            return false;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            OnFileChanged();
        }

        internal NetworkDrive? AddNetworkDrive(string p_strNetworkDriveName)
        {
            if (XmlDocument != null)
            {
                XmlNode? objRoot = XmlDocument.DocumentElement;
                XmlNodeList? objMenusNode = null;

                if (objRoot != null)
                    objMenusNode = objRoot.SelectNodes("/" + ElementName + "/" + NetworkDriveList.ElementName);

                if (objMenusNode != null)
                {
                    XmlElement objNetworkDriveElement = XmlDocument.CreateElement(NetworkDrive.ElementName);
                    XmlAttribute objNetworkDriveNameAttribute = XmlDocument.CreateAttribute("Name");
                    objNetworkDriveNameAttribute.Value = p_strNetworkDriveName;
                    objNetworkDriveElement.Attributes.Append(objNetworkDriveNameAttribute);
                    objMenusNode[0]?.AppendChild(objNetworkDriveElement);

                    return new NetworkDrive(this, objNetworkDriveElement);
                }
            }

            return null;
        }

        internal void RemoveNetworkDrive(NetworkDrive p_objNetworkDrive)
        {
            if (XmlDocument != null)
            {
                p_objNetworkDrive.XmlNode?.ParentNode?.RemoveChild(p_objNetworkDrive.XmlNode);
            }
        }

        internal Script? AddScript(string p_strScriptName)
        {
            if (XmlDocument != null)
            {
                XmlNode? objRoot = XmlDocument.DocumentElement;
                XmlNodeList? objMenusNode = null;

                if (objRoot != null)
                    objMenusNode = objRoot.SelectNodes("/" + ElementName + "/" + ScriptList.ElementName);

                if (objMenusNode != null)
                {
                    XmlElement objScriptElement = XmlDocument.CreateElement(Script.ElementName);
                    XmlAttribute objScriptNameAttribute = XmlDocument.CreateAttribute("Name");
                    objScriptNameAttribute.Value = p_strScriptName;
                    objScriptElement.Attributes.Append(objScriptNameAttribute);
                    objMenusNode[0]?.AppendChild(objScriptElement);

                    return new Script(this, objScriptElement);
                }
            }

            return null;
        }

        internal void RemoveScript(Script p_objScript)
        {
            if (XmlDocument != null)
            {
                p_objScript.XmlNode?.ParentNode?.RemoveChild(p_objScript.XmlNode);
            }
        }

        internal Menu? AddMenu(string p_strMenuName)
        {
            if (XmlDocument != null)
            {
                XmlNode? objRoot = XmlDocument.DocumentElement;
                XmlNodeList? objMenusNode = null;

                if (objRoot != null)
                    objMenusNode = objRoot.SelectNodes("/" + ElementName + "/" + MenuList.ElementName);

                if (objMenusNode != null)
                {
                    XmlElement objMenuElement = XmlDocument.CreateElement(Menu.ElementName);
                    XmlAttribute objMenuNameAttribute = XmlDocument.CreateAttribute("Name");
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
            if (XmlDocument != null)
            {
                p_objMenu.XmlNode?.ParentNode?.RemoveChild(p_objMenu.XmlNode);
            }
        }

        internal override void InsertItem(DataModelBase p_objObject, int p_intIndex)
        {
            if ((p_objObject is NetworkDriveList)
                || (p_objObject is ScriptList)
                || (p_objObject is MenuList)
                || (p_objObject is Environment)
                )
            {
                if (p_intIndex >= 0)
                    XmlNode?.InsertBefore(p_objObject.XmlNode, XmlNode?.ChildNodes[p_intIndex]);
                else
                    XmlNode?.AppendChild(p_objObject.XmlNode);
            }
            else
                throw new ArgumentException();
        }

        private ScriptList CreateScriptList()
        {
            XmlElement objElement = XmlDocument.CreateElement(ScriptList.ElementName);
            return new ScriptList(this, objElement);
        }

        private NetworkDriveList CreateNetworkDriveList()
        {
            XmlElement objElement = XmlDocument.CreateElement(DataModels.NetworkDriveList.ElementName);
            return new NetworkDriveList(this, objElement);
        }

        private MenuList CreateMenuList()
        {
            XmlElement objElement = XmlDocument.CreateElement(MenuList.ElementName);
            return new MenuList(this, objElement);
        }

        private Environment CreateEnvironment()
        {
            XmlElement objEnvironmentElement = XmlDocument.CreateElement(Environment.ElementName);
            return new Environment(this, objEnvironmentElement);
        }

        public ScriptList ScriptList
        {
            get
            {
                XmlNode? objScriptListNode = XmlNode?.SelectSingleNode("/" + MenuFile.ElementName + "/" + ScriptList.ElementName);

                if (objScriptListNode == null)
                {
                    ScriptList objConfigList = CreateScriptList();

                    InsertItem(objConfigList, 0);

                    return objConfigList;
                }
                else
                    return new ScriptList(this, objScriptListNode);
            }
        }

        public NetworkDriveList NetworkDriveList
        {
            get
            {
                XmlNode? objNetworkDriveListNode = XmlNode?.SelectSingleNode("/" + MenuFile.ElementName + "/" + NetworkDriveList.ElementName);

                if (objNetworkDriveListNode == null)
                {
                    NetworkDriveList objNetworkDriveList = CreateNetworkDriveList();

                    InsertItem(objNetworkDriveList, 0);

                    return objNetworkDriveList;
                }
                else
                    return new NetworkDriveList(this, objNetworkDriveListNode);
            }
        }

        public MenuList MenuList
        {
            get
            {
                XmlNode? objMenuListNode = XmlNode?.SelectSingleNode("/" + MenuFile.ElementName + "/" + MenuList.ElementName);

                if (objMenuListNode == null)
                {
                    MenuList objMenuList = CreateMenuList();

                    InsertItem(objMenuList, 0);

                    return objMenuList;
                }
                else
                    return new MenuList(this, objMenuListNode);
            }
        }

        public Environment Environment
        {
            get
            {
                XmlNode? objEnvironmentNode = XmlNode?.SelectSingleNode("/" + MenuFile.ElementName + "/" + MenuList.ElementName + "/" + Environment.ElementName);

                if (objEnvironmentNode == null)
                {
                    Environment objEnvironment = CreateEnvironment();

                    InsertItem(objEnvironment, 0);

                    return objEnvironment;
                }
                else
                    return new Environment(this, objEnvironmentNode);
            }
        }

        public new bool CanEdit
        {
            get { return MemberOf(SecurityGroup); }
        }
    }
}
