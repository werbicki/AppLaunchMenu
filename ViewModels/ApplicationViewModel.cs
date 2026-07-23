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

        protected override void OnLoadChildren()
        {
            foreach (EnvironmentViewModel objEnvironmentViewModel in Collection<EnvironmentViewModel, DataModels.Environment>(this))
                Children.Add(objEnvironmentViewModel);
        }

        public Application Application
        {
            get { return DataModel; }
        }

        public EnvironmentViewModel Environment
        {
            get { return ViewModel<EnvironmentViewModel, DataModels.Environment>(DataModel.Environment); }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public string Executable
        {
            get { return DataModel.ExecutablePath; }
            set
            {
                DataModel.ExecutablePath = value;
                OnPropertyChanged(nameof(Executable));
            }
        }

        public string WorkingDirectory
        {
            get { return DataModel.WorkingDirectory; }
            set
            {
                DataModel.WorkingDirectory = value;
                OnPropertyChanged(nameof(WorkingDirectory));
            }
        }

        public string Parameters
        {
            get { return DataModel.Parameters; }
            set
            {
                DataModel.Parameters = value;
                OnPropertyChanged(nameof(Parameters));
            }
        }

        public bool IsReservable
        {
            get { return DataModel.Reservable; }
            set
            {
                DataModel.Reservable = value;
                OnPropertyChanged(nameof(IsReservable));
            }
        }

        public string ReservationDescription
        {
            get { return DataModel.ReservationDescription; }
            set
            {
                DataModel.ReservationDescription = value;
                OnPropertyChanged(nameof(ReservationDescription));
            }
        }

        public DateTimeOffset ReservationDate
        {
            get { return DataModel.ReservationDate; }
            set
            {
                DataModel.ReservationDate = value;
                OnPropertyChanged(nameof(ReservationDate));
            }
        }

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
