using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class ApplicationViewModel : ViewModelTreeBase<Application>
    {
        public ApplicationViewModel(Application p_objApplication, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objApplication, p_objLaunchMenu, p_objParent)
        {
        }

        public Application Application
        {
            get { return DataModel; }
        }

        protected override void OnLoadChildren()
        {
            Children.Add(Environment);
            Children.Add(ServiceList);
        }

        public EnvironmentViewModel Environment
        {
            get { return ViewModel<EnvironmentViewModel, DataModels.Environment>(DataModel.Environment); }
        }

        public ServiceListViewModel ServiceList
        {
            get { return ViewModel<ServiceListViewModel, DataModels.ServiceList>(DataModel.ServiceList); }
        }

        [DialogContent("Application Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        [DialogContent("Executable")]
        public string Executable
        {
            get { return DataModel.ExecutablePath; }
            set
            {
                DataModel.ExecutablePath = value;
                OnPropertyChanged(nameof(Executable));
            }
        }

        [DialogContent("Working Directory")]
        public string WorkingDirectory
        {
            get { return DataModel.WorkingDirectory; }
            set
            {
                DataModel.WorkingDirectory = value;
                OnPropertyChanged(nameof(WorkingDirectory));
            }
        }

        [DialogContent("Parameters")]
        public string Parameters
        {
            get { return DataModel.Parameters; }
            set
            {
                DataModel.Parameters = value;
                OnPropertyChanged(nameof(Parameters));
            }
        }

        [DialogContent("Is Reservable")]
        public bool IsReservable
        {
            get { return DataModel.Reservable; }
            set
            {
                DataModel.Reservable = value;
                OnPropertyChanged(nameof(IsReservable));
            }
        }

        [DialogContent("Reservation Description")]
        public string ReservationDescription
        {
            get { return DataModel.ReservationDescription; }
            set
            {
                DataModel.ReservationDescription = value;
                OnPropertyChanged(nameof(ReservationDescription));
            }
        }

        [DialogContent("Reservation Date")]
        public DateTimeOffset ReservationDate
        {
            get { return DataModel.ReservationDate; }
            set
            {
                DataModel.ReservationDate = value;
                OnPropertyChanged(nameof(ReservationDate));
            }
        }

        [DialogContent("Reservation Owner")]
        public string ReservationOwner
        {
            get { return DataModel.ReservationOwner; }
            set
            {
                DataModel.ReservationOwner = value;
                OnPropertyChanged(nameof(ReservationOwner));
            }
        }
    }
}
