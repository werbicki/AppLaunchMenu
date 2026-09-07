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
    public class ServiceViewModel : ViewModelTreeBase<Service>
    {
        public ServiceViewModel(Service p_objService, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objService, p_objLaunchMenu, p_objParent)
        {
        }

        protected override void OnLoadChildren()
        {
            foreach (ServerViewModel objServerViewModel in Collection<ServerViewModel, Server>(this))
                Children.Add(objServerViewModel);
        }

        [DialogContent("Executable Path")]
        public string ExecutablePath
        {
            get { return DataModel.ExecutablePath; }
            set
            {
                DataModel.ExecutablePath = value;
                OnPropertyChanged(nameof(ExecutablePath));
            }
        }
    }
}
