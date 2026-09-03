using AppLaunchMenu.DataAccess;
using AppLaunchMenu.Helper;
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
        public NetworkDrive(LaunchMenuFile p_objMenuFile, XmlNode p_objNetworkDriveNode)
            : base(p_objMenuFile, new Type[] { }, p_objNetworkDriveNode)
        {
        }

        public NetworkDrive(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { }, p_strName)
        {
        }

        internal static string ElementName
        {
            get { return nameof(NetworkDrive); }
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

        public string MapNetworkDrive()
        {
            if (UnmapFirst)
                UnmapNetworkDrive();

            //Environment objEnvironment = m_objNetworkDrives.Folder.Environment;
            //string strRemoveUncPath = objEnvironment.ExpandVariable(RemoteUncPath);
            //string strLocalDriveLetter = objEnvironment.ExpandVariable(LocalDriveLetter);
            string strRemoveUncPath = RemoteUncPath;
            string strLocalDriveLetter = LocalDriveLetter;

            NativeMethods.NETRESOURCE objNetResource = new NativeMethods.NETRESOURCE
            {
                dwType = 1, // RESOURCETYPE_DISK
                lpLocalName = strLocalDriveLetter,
                lpRemoteName = strRemoveUncPath
            };

            int intFlags = Persistent ? 1 : 0; // RESOURCE_REMEMBERED flag value might vary, often 1 or a specific enum

            int intResult = NativeMethods.WNetAddConnection2(ref objNetResource, null, null, intFlags);

            if (intResult == NativeMethods.NO_ERROR)
                return "Mapped";
            //else if (intResult == NativeMethods.ERROR_ACCESS_DENIED)
            //    return "Access denied";
            else if (intResult == NativeMethods.ERROR_BAD_DEVICE)
                return "Invalid device";
            else
                return "Error";
        }

        public bool UnmapNetworkDrive()
        {
            //Environment objEnvironment = m_objNetworkDrives.Folder.Environment;
            //string strLocalDriveLetter = objEnvironment.ExpandVariable(LocalDriveLetter);
            string strLocalDriveLetter = LocalDriveLetter;

            // dwFlags can be 0 or CONNECT_UPDATE_PROFILE (1) to make changes permanent/persistent
            // fForce can be 0 (false) or 1 (true)
            NativeMethods.WNetCancelConnection2(strLocalDriveLetter, 0, ForceUnmap ? 1 : 0);

            return false;
        }
    }
}
