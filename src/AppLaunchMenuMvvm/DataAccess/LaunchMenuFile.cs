using AppLaunchMenu.DataModels;
using Microsoft.UI.Dispatching;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using Environment = AppLaunchMenu.DataModels.Environment;

namespace AppLaunchMenu.DataAccess
{
    public class LaunchMenuFile : DataAccessBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        { 
            get
            { 
                return
                [
                    new() { Type = typeof(DataCenterList), ElementType = ElementTypeEnum.ZeroOrOne },
                    new() { Type = typeof(NetworkDriveList), ElementType = ElementTypeEnum.ZeroOrOne },
                    new() { Type = typeof(ScriptList), ElementType = ElementTypeEnum.ZeroOrOne },
                    new() { Type = typeof(MenuList), ElementType = ElementTypeEnum.ZeroOrOne },
                    new() { Type = typeof(Environment), ElementType = ElementTypeEnum.ZeroOrOne }
                ];
            }
        }

        private readonly DispatcherQueue m_objDispatcherQueue = DispatcherQueue.GetForCurrentThread();
        public delegate void FileChangedEventHandler(object? sender, DataChangedEventArgs e);
        public event FileChangedEventHandler? FileChanged;
        private bool m_blnEditMode = false;
        private string m_strFilename = "";
        private FileSystemWatcher m_objFileSystemWatcher = new FileSystemWatcher();

        public LaunchMenuFile()
            : base(new XmlDocument())
        {
            CreateFile("New AppLaunchMenu");
        }

        public LaunchMenuFile(string p_strFilename)
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

        protected virtual void OnFileChanged()
        {
            m_objDispatcherQueue.TryEnqueue(() =>
            {
                var eventHandler = FileChanged;
                if (eventHandler != null)
                    eventHandler(this, new DataChangedEventArgs());
            });
        }

        internal override LaunchMenuFile MenuFile
        {
            get { return this; }
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return "AppLaunchMenu"; }
        }


        public DataCenterList DataCenterList
        {
            get { return GetItem<DataCenterList>(); }
        }

        public NetworkDriveList NetworkDriveList
        {
            get { return GetItem<NetworkDriveList>(); }
        }

        public ScriptList ScriptList
        {
            get { return GetItem<ScriptList>(); }
        }

        public MenuList MenuList
        {
            get { return GetItem<MenuList>(); }
        }

        public Environment Environment
        {
            get { return GetItem<Environment>(); }
        }

        public String Directory
        {
            get
            {
                if (m_strFilename != null)
                {
                    FileInfo objFileInfo = new FileInfo(m_strFilename);
                    if (objFileInfo.DirectoryName != null)
                        return objFileInfo.DirectoryName;
                }

                if (System.Environment.ProcessPath != null)
                    return System.Environment.ProcessPath;

                return AppDomain.CurrentDomain.BaseDirectory;
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

        public new bool HasEditAccess
        {
            get { return MemberOf(SecurityGroup); }
        }

        public bool EditMode
        {
            get
            {
                if (HasEditAccess)
                    return m_blnEditMode;
                return
                    false;
            }
            set
            {
                if (HasEditAccess)
                    m_blnEditMode = value;
            }
        }

        public string LocalDomainName
        {
            get { return System.Environment.UserDomainName; }
        }

        public string LocalUsername
        {
            get { return System.Environment.UserName; }
        }

        public string LocalHostname
        {
            get
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    return Dns.GetHostName();

                return "localhost";
            }
        }

        public IPAddress LocalIpAddress
        {
            get
            {
                //System.Environment.MachineName

                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    var objIPHostEntry = Dns.GetHostEntry(Dns.GetHostName());

                    foreach (var objIPAddress in objIPHostEntry.AddressList)
                    {
                        if (objIPAddress.AddressFamily == AddressFamily.InterNetwork)
                            return objIPAddress;
                    }
                }

                return IPAddress.Parse("127.0.0.1");
            }
        }

        public string LocalDataCenter
        {
            get
            {
                foreach (DataCenter objDataCenter in DataCenterList.DataCenters)
                {
                    if ((!string.IsNullOrWhiteSpace(objDataCenter.Username))
                        || (!string.IsNullOrWhiteSpace(objDataCenter.Hostname))
                        || (!string.IsNullOrWhiteSpace(objDataCenter.Subnet))
                        )
                    {
                        // Omit LocalDataCenter to prevent stack overflow.
                        if (Matches(objDataCenter.Username, MenuFile.LocalUsername)
                            && Matches(objDataCenter.Hostname, MenuFile.LocalHostname)
                            && Matches(objDataCenter.Subnet, MenuFile.LocalIpAddress.ToString())
                            )
                        {
                            return objDataCenter.Name;
                        }
                    }
                }

                return "None";
            }
        }

        public bool MapNetworkDrives
        {
            get { return GetXmlAttributeBool(nameof(MapNetworkDrives), false); }
            set { SetXmlAttributeBool(nameof(MapNetworkDrives), value, false); }
        }

        public string LogoImage
        {
            get { return GetXmlAttribute(nameof(LogoImage)); }
            set
            {
                FileInfo objLogoFileInfo = new FileInfo(value);

                SetXmlAttribute(nameof(LogoImage), objLogoFileInfo.Name);
            }
        }

        public string LogoImagePath
        {
            get
            {
                FileInfo objFileInfo = new FileInfo(m_strFilename);
                FileInfo objLogoFileInfo = new FileInfo(GetXmlAttribute(nameof(LogoImage)));

                return objFileInfo.DirectoryName + Path.DirectorySeparatorChar + objLogoFileInfo.Name;
            }
        }

        public void Load()
        {
            if (m_strFilename != null)
            {
                FileInfo objFileInfo = new FileInfo(m_strFilename);

                if (objFileInfo.Exists)
                {
                    ReadFile(m_strFilename);

                    OnFileChanged();
                }
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
                    throw new Exception("Unable to Save file '" + m_strFilename + "'\n\n" + e.Message, e);
                }
            }
        }

        public void SaveAs(string p_strFilename)
        {
            if (!string.IsNullOrEmpty(p_strFilename))
            {
                try
                {
                    XmlDocument.Save(p_strFilename);

                    m_strFilename = p_strFilename;
                    IsDirty = false;
                }
                catch (XmlException e)
                {
                    throw new Exception("Unable to Save file as '" + p_strFilename + "'\n\n" + e.Message, e);
                }
            }
        }

        public void Reload()
        {
            if (m_strFilename != null)
            {
                FileInfo objFileInfo = new FileInfo(m_strFilename);

                if (objFileInfo.Exists)
                {
                    ReadFile(m_strFilename);

                    OnFileChanged();
                }
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

        internal override void InsertItem(DataModelBase p_objObject, int p_intIndex)
        {
            if (IsValidChildNodeType(p_objObject.GetType()))
            {
                if (p_intIndex >= 0)
                    XmlNode?.InsertBefore(p_objObject.XmlNode, XmlNode?.ChildNodes[p_intIndex]);
                else
                    XmlNode?.AppendChild(p_objObject.XmlNode);
            }
            else
                throw new ArgumentException();
        }

        private DataCenterList CreateDataCenterList()
        {
            XmlElement objElement = XmlDocument.CreateElement(DataModels.DataCenterList.ElementName);
            return new DataCenterList(this, objElement);
        }

        private NetworkDriveList CreateNetworkDriveList()
        {
            XmlElement objElement = XmlDocument.CreateElement(DataModels.NetworkDriveList.ElementName);
            return new NetworkDriveList(this, objElement);
        }

        private ScriptList CreateScriptList()
        {
            XmlElement objElement = XmlDocument.CreateElement(ScriptList.ElementName);
            return new ScriptList(this, objElement);
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
    }
}
