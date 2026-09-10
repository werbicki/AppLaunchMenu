using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public class DataCenterViewModel : ViewModelTreeBase<DataCenter>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get { return []; }
        }

        public DataCenterViewModel(DataCenter p_objDataCenter, ILaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objDataCenter, p_objLaunchMenu, p_objParent)
        {
        }

        internal DataCenter DataCenterDataModel
        {
            get { return DataModel; }
        }
    }
}
