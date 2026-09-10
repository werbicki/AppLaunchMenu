using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public partial class ServiceListViewModel : ViewModelTreeBase<ServiceList>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get
            {
                return
                [
                    new() { DataModelType = typeof(Service), ViewModelType = typeof(ServiceViewModel) },
                ];
            }
        }

        public ServiceListViewModel(ServiceList p_objServiceList, ILaunchMenu p_objLaunchMenu)
            : base(p_objServiceList, p_objLaunchMenu)
        {
        }

        protected override void OnLoadChildren()
        {
            foreach (ServiceViewModel objServiceViewModel in Collection<ServiceViewModel, Service>(this))
                Children.Add(objServiceViewModel);
        }

        [DialogContent("Service List Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override bool Expanded
        {
            get { return false; }
        }
    }
}
