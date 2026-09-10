using AppLaunchMenu.DataModels;
using AppLaunchMenu.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinUIEditor;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace AppLaunchMenu.ViewModels
{
    public class NetworkDriveViewModel : ViewModelTreeBase<NetworkDrive>
    {
        public NetworkDriveViewModel(NetworkDrive p_objNetworkDrive, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objNetworkDrive, p_objLaunchMenu, p_objParent)
        {
        }

        internal NetworkDrive NetworkDrive
        {
            get { return DataModel; }
        }

        [DialogContent("Remote UNC Path")]
        public string RemoteUncPath
        {
            get { return DataModel.RemoteUncPath; }
            set
            {
                DataModel.RemoteUncPath = value;
                OnPropertyChanged(nameof(RemoteUncPath));
            }
        }

        [DialogContent("Local Drive Letter")]
        public string LocalDriveLetter
        {
            get { return DataModel.LocalDriveLetter; }
            set
            {
                DataModel.LocalDriveLetter = value;
                OnPropertyChanged(nameof(LocalDriveLetter));
            }
        }

        [DialogContent("Persistent")]
        public bool Persistent
        {
            get { return DataModel.Persistent; }
            set
            {
                DataModel.Persistent = value;
                OnPropertyChanged(nameof(Persistent));
            }
        }

        [DialogContent("Unmap First")]
        public bool UnmapFirst
        {
            get { return DataModel.UnmapFirst; }
            set
            {
                DataModel.UnmapFirst = value;
                OnPropertyChanged(nameof(UnmapFirst));
            }
        }

        [DialogContent("Force Unmap")]
        public bool ForceUnmap
        {
            get { return DataModel.ForceUnmap; }
            set
            {
                DataModel.ForceUnmap = value;
                OnPropertyChanged(nameof(ForceUnmap));
            }
        }

        public string Status
        {
            get { return DataModel.GetDriveMapping(); }
        }

        public bool MapNetworkDrive()
        {
            string strStatus = DataModel.MapNetworkDrive();
            OnPropertyChanged(nameof(Status));

            return strStatus.StartsWith("\\\\");
        }
    }
}
