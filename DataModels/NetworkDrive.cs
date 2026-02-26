using AppLaunchMenu.DataAccess;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class NetworkDrive : DataModelBase
    {
        protected NetworkDriveListing m_objNetworkDrives;

        public NetworkDrive(MenuFile p_objMenuFile, NetworkDriveListing p_objNetworkDrives, XmlNode p_objApplicationNode)
            : base(p_objMenuFile, p_objApplicationNode)
        {
            m_objNetworkDrives = p_objNetworkDrives;
        }

        public NetworkDrive(MenuFile p_objMenuFile, NetworkDriveListing p_objNetworkDrives, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
            m_objNetworkDrives = p_objNetworkDrives;
        }

        internal static string ElementName
        {
            get { return nameof(Application); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
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
            get { return GetXmlAttribute(nameof(Persistent)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(Persistent), value ? "true" : "false"); }
        }

        public bool UnmapFirst
        {
            get { return GetXmlAttribute(nameof(UnmapFirst)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(UnmapFirst), value ? "true" : "false"); }
        }

        public bool ForceUnmap
        {
            get { return GetXmlAttribute(nameof(ForceUnmap)).Equals("true", StringComparison.CurrentCultureIgnoreCase); }
            set { SetXmlAttribute(nameof(ForceUnmap), value ? "true" : "false"); }
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(ref NETRESOURCE lpNetResource, string? lpPassword, string? lpUsername, int dwFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string lpName, uint dwFlags, int fForce);

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string? lpLocalName;
            public string? lpRemoteName;
            public string? lpComment;
            public string? lpProvider;
        }

        public bool MapNetworkDrive()
        {
            if (UnmapFirst)
                UnmapNetworkDrive();

            Environment objEnvironment = m_objNetworkDrives.Folder.Environment;
            string strRemoveUncPath = objEnvironment.ExpandVariable(RemoteUncPath);
            string strLocalDriveLetter = objEnvironment.ExpandVariable(LocalDriveLetter);

            NETRESOURCE objNetResource = new NETRESOURCE
            {
                dwType = 1, // RESOURCETYPE_DISK
                lpLocalName = strLocalDriveLetter,
                lpRemoteName = strRemoveUncPath
            };

            int intFlags = Persistent ? 1 : 0; // RESOURCE_REMEMBERED flag value might vary, often 1 or a specific enum

            int intResult = WNetAddConnection2(ref objNetResource, null, null, intFlags);

            return (intResult != 0);
        }

        public bool UnmapNetworkDrive()
        {
            Environment objEnvironment = m_objNetworkDrives.Folder.Environment;
            string strLocalDriveLetter = objEnvironment.ExpandVariable(LocalDriveLetter);

            // dwFlags can be 0 or CONNECT_UPDATE_PROFILE (1) to make changes permanent/persistent
            // fForce can be 0 (false) or 1 (true)
            WNetCancelConnection2(strLocalDriveLetter, 0, ForceUnmap ? 1 : 0);

            return false;
        }
    }
}
