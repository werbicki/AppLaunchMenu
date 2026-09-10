using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System.Collections.ObjectModel;

namespace AppLaunchMenu.ViewModels
{
    public partial class DataCenterListViewModel : ViewModelTreeBase<DataCenterList>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get
            {
                return
                [
                    new() { DataModelType = typeof(DataCenter), ViewModelType = typeof(DataCenterViewModel) },
                ];
            }
        }

        protected ObservableCollection<DataCenterViewModel> m_objAllDataCenters = new ObservableCollection<DataCenterViewModel>();

        public DataCenterListViewModel(DataCenterList p_objDataCenterList, ILaunchMenu p_objLaunchMenu)
            : base(p_objDataCenterList, p_objLaunchMenu)
        {
            foreach (DataCenter objDataCenter in DataModel.DataCenters)
                m_objAllDataCenters.Add(new DataCenterViewModel(objDataCenter, p_objLaunchMenu, this));
        }

        protected override void OnLoadChildren()
        {
            foreach (DataCenterViewModel objDataCenterViewModel in Collection<DataCenterViewModel, DataCenter>(this))
                Children.Add(objDataCenterViewModel);
        }

        [DialogContent("DataC enter List Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<DataCenterViewModel> DataCenters
        {
            get { return Collection<DataCenterViewModel, DataCenter>(); }
        }

        public ObservableCollection<DataCenterViewModel> AllDataCenters
        {
            get { return m_objAllDataCenters; }
        }
    }
}
