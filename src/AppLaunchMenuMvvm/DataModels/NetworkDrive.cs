using AppLaunchMenu.DataAccess;
using System;
using System.Runtime.InteropServices;
using System.Xml;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.NetworkManagement.WNet;

namespace AppLaunchMenu.DataModels
{
    public class NetworkDrive : DataModelBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        {
            get { return []; }
        }

        public NetworkDrive(LaunchMenuFile p_objMenuFile, XmlNode p_objNetworkDriveNode)
            : base(p_objMenuFile, p_objNetworkDriveNode)
        {
        }

        public NetworkDrive(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(NetworkDrive); }
        }

        public string RemoteUncPath
        {
            get { return GetXmlAttribute(nameof(RemoteUncPath)); }
            set { SetXmlAttribute(nameof(RemoteUncPath), value); }
        }

        public string LocalDriveLetter
        {
            get { return GetXmlAttribute(nameof(LocalDriveLetter)); }
            set { SetXmlAttribute(nameof(LocalDriveLetter), value); }
        }

        public bool Persistent
        {
            get { return GetXmlAttributeBool(nameof(Persistent)); }
            set { SetXmlAttributeBool(nameof(Persistent), value); }
        }

        public bool UnmapFirst
        {
            get { return GetXmlAttributeBool(nameof(UnmapFirst)); }
            set { SetXmlAttributeBool(nameof(UnmapFirst), value); }
        }

        public bool ForceUnmap
        {
            get { return GetXmlAttributeBool(nameof(ForceUnmap)); }
            set { SetXmlAttributeBool(nameof(ForceUnmap), value); }
        }

        public string GetDriveMapping()
        {
            string strStatus = "";

            if (!string.IsNullOrWhiteSpace(LocalDriveLetter))
            {
                string strLocalDriveLetter = LocalDriveLetter.Trim().Substring(0, 1).ToUpper() + ":";

                try
                {
                    unsafe
                    {
                        uint intBufferSize = 512;
                        Span<char> pBuffer = stackalloc char[(int)intBufferSize];

                        WIN32_ERROR enumResult = PInvoke.WNetGetConnection(strLocalDriveLetter, pBuffer, ref intBufferSize);

                        // If the buffer was too small, retry with the updated capacity returned by the API
                        if (enumResult == WIN32_ERROR.ERROR_MORE_DATA)
                        {
                            Span<char> pLargerBuffer = stackalloc char[(int)intBufferSize];

                            enumResult = PInvoke.WNetGetConnection(strLocalDriveLetter, pLargerBuffer, ref intBufferSize);
                            if (enumResult == WIN32_ERROR.ERROR_SUCCESS)
                                strStatus = new string(pLargerBuffer);
                        }

                        if (enumResult == WIN32_ERROR.NO_ERROR)
                            strStatus = new string(pBuffer);
                        else if (enumResult == WIN32_ERROR.ERROR_NOT_CONNECTED)
                            strStatus = "Not mapped";
                        else if (enumResult == WIN32_ERROR.ERROR_BAD_DEVICE)
                            strStatus = "Invalid device";
                        else
                            return "Error";
                    }
                }
                catch
                {
                    strStatus = "Exception";
                }
            }
            else
                strStatus = "No drive assigned";

            return strStatus;
        }

        public string MapNetworkDrive()
        {
            string strStatus = "Unknown";

            if (UnmapFirst)
                UnmapNetworkDrive();

            //Environment objEnvironment = m_objNetworkDrives.Folder.Environment;
            //string strRemoveUncPath = objEnvironment.ExpandVariable(RemoteUncPath);
            //string strLocalDriveLetter = objEnvironment.ExpandVariable(LocalDriveLetter);

            string strRemoveUncPath = RemoteUncPath;
            string strLocalDriveLetter = LocalDriveLetter;

            // Allocate unmanaged memory for the strings
            IntPtr pRemote = Marshal.StringToHGlobalUni(strRemoveUncPath);
            IntPtr pLocal = Marshal.StringToHGlobalUni(strLocalDriveLetter);

            try
            {
                unsafe
                {
                    // Initialize the struct
                    NETRESOURCEW objNetResource = new NETRESOURCEW
                    {
                        //dwScope = NETRESOURCE_SCOPE.RESOURCE_GLOBALNET, // Or other scope flags
                        dwType = NET_RESOURCE_TYPE.RESOURCETYPE_DISK,
                        //dwDisplayType = NETRESOURCE_DISPLAY_TYPE.RESOURCEDISPLAYTYPE_SHARE,
                        //dwUsage = NETRESOURCE_USAGE.RESOURCEUSAGE_CONNECTABLE,
                        lpLocalName = (PWSTR)(void*)pLocal,
                        lpRemoteName = (PWSTR)(void*)pRemote,
                        lpComment = null,
                        lpProvider = null
                    };

                    NET_CONNECT_FLAGS enumNetConnectFlags = Persistent ? NET_CONNECT_FLAGS.CONNECT_UPDATE_PROFILE : 0;
                    WIN32_ERROR enumResult = PInvoke.WNetAddConnection2W(objNetResource, null, null, enumNetConnectFlags);

                    if (enumResult == WIN32_ERROR.NO_ERROR)
                        strStatus = GetDriveMapping();
                    else if (enumResult == WIN32_ERROR.ERROR_ACCESS_DENIED)
                        strStatus = "Access denied";
                    else if (enumResult == WIN32_ERROR.ERROR_BAD_DEVICE)
                        strStatus = "Invalid device";
                    else
                        strStatus = "Error";
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pLocal);
                Marshal.FreeHGlobal(pRemote);
            }

            return strStatus;
        }

        public bool UnmapNetworkDrive()
        {
            //Environment objEnvironment = m_objNetworkDrives.Folder.Environment;
            //string strLocalDriveLetter = objEnvironment.ExpandVariable(LocalDriveLetter);
            string strLocalDriveLetter = LocalDriveLetter;

            PInvoke.WNetCancelConnection2W(strLocalDriveLetter, 0, ForceUnmap ? true : false);

            return false;
        }
    }
}
