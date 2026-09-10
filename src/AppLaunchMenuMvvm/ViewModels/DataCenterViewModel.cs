using AppLaunchMenu.DataModels;
using AppLaunchMenu.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.Json.Serialization;

namespace AppLaunchMenu.ViewModels
{
    public class DataCenterViewModel : ViewModelTreeBase<DataCenter>
    {
        public DataCenterViewModel(DataCenter p_objDataCenter, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objDataCenter, p_objLaunchMenu, p_objParent)
        {
        }

        internal DataCenter DataCenterDataModel
        {
            get { return DataModel; }
        }
    }
}
