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
    public class ServerViewModel : ViewModelTreeBase<Server>
    {
        public ServerViewModel(Server p_objServer, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objServer, p_objLaunchMenu, p_objParent)
        {
        }

        internal Server Server
        {
            get { return DataModel; }
        }

        [DialogContent("Service Username")]
        public string ServiceUsername
        {
            get { return DataModel.ServiceUsername; }
            set
            {
                DataModel.ServiceUsername = value;
                OnPropertyChanged(nameof(ServiceUsername));
            }
        }
    }
}
