using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public partial class ServiceListViewModel : ViewModelTreeBase<ServiceList>
    {
        public ServiceListViewModel(ServiceList p_objServiceList, LaunchMenu p_objLaunchMenu)
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
