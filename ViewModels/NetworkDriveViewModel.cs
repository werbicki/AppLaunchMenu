using AppLaunchMenu.DataModels;
using AppLaunchMenu.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json.Serialization;

namespace AppLaunchMenu.ViewModels
{
    public class NetworkDriveViewModel : ViewModelTreeBase<NetworkDrive>
    {
        private string m_strStatus = "";

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
            get
            {
                if ((string.IsNullOrEmpty(m_strStatus))
                    && (!string.IsNullOrWhiteSpace(LocalDriveLetter))
                    )
                {
                    // Format the drive letter properly to "X:"
                    string strFormattedDrive = LocalDriveLetter.Trim().Substring(0, 1).ToUpper() + ":";

                    // Initialize StringBuilder with an initial capacity
                    int intCapacity = 512;
                    StringBuilder objStringBuilder = new StringBuilder(intCapacity);

                    // Call the API function
                    int intResult = NativeMethods.WNetGetConnection(strFormattedDrive, objStringBuilder, ref intCapacity);

                    // If the buffer was too small, retry with the updated capacity returned by the API
                    if (intResult == 234) // ERROR_MORE_DATA
                    {
                        objStringBuilder.EnsureCapacity(intCapacity);
                        intResult = NativeMethods.WNetGetConnection(strFormattedDrive, objStringBuilder, ref intCapacity);
                    }

                    if (intResult == NativeMethods.NO_ERROR)
                        m_strStatus = objStringBuilder.ToString();
                    else if (intResult == NativeMethods.ERROR_NOT_CONNECTED)
                        m_strStatus = "Not mapped";
                    else if (intResult == NativeMethods.ERROR_BAD_DEVICE)
                        m_strStatus = "Invalid device";
                    else
                        return "Error";
                }
                else
                    m_strStatus = "Invalid drive letter";

                return m_strStatus;
            }
            set
            {
                m_strStatus = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public bool MapNetworkDrive()
        {
            Status = DataModel.MapNetworkDrive();

            return Status == "Mapped";
        }
    }
}
