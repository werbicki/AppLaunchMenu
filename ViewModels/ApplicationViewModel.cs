using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class ApplicationViewModel : TreeViewItemViewModel
    {
        Application m_objApplication;

        public ApplicationViewModel(LaunchMenu p_objLaunchMenu, Application p_objApplication, TreeViewItemViewModel p_objParent)
            : base(p_objLaunchMenu, p_objApplication, p_objParent)
        {
            m_objApplication = p_objApplication;
        }

        protected override void Insert(object p_objItem, int p_intIndex)
        {
            //if (p_objItem is EnvironmentViewModel objEnvironmentViewModel)
            //    m_objApplication.Insert(objEnvironmentViewModel.Environment, p_intIndex);
        }

        protected override void Remove(object p_objItem, int p_intIndex)
        {
            //if (p_objItem is EnvironmentViewModel objEnvironmentViewModel)
            //    m_objApplication.Remove(objEnvironmentViewModel.Environment);
        }

        public Application Application
        {
            get { return m_objApplication; }
            set { m_objApplication = value; }
        }

        public override string Name
        {
            get { return m_objApplication.Name; }
            set
            {
                m_objApplication.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public string Executable
        {
            get { return m_objApplication.ExecutablePath; }
            set
            {
                m_objApplication.ExecutablePath = value;
                OnPropertyChanged(nameof(Executable));
            }
        }

        public string WorkingDirectory
        {
            get { return m_objApplication.WorkingDirectory; }
            set
            {
                m_objApplication.WorkingDirectory = value;
                OnPropertyChanged(nameof(WorkingDirectory));
            }
        }

        public string Parameters
        {
            get { return m_objApplication.Parameters; }
            set
            {
                m_objApplication.Parameters = value;
                OnPropertyChanged(nameof(Parameters));
            }
        }

        public EnvironmentViewModel Environment
        {
            get { return new EnvironmentViewModel(LaunchMenu, m_objApplication.Environment); }
        }

        public bool IsReservable
        {
            get { return m_objApplication.Reservable; }
            set
            {
                m_objApplication.Reservable = value;
                OnPropertyChanged(nameof(IsReservable));
            }
        }

        public string ReservationDescription
        {
            get { return m_objApplication.ReservationDescription; }
            set
            {
                m_objApplication.ReservationDescription = value;
                OnPropertyChanged(nameof(ReservationDescription));
            }
        }

        public DateTimeOffset ReservationDate
        {
            get { return m_objApplication.ReservationDate; }
            set
            {
                m_objApplication.ReservationDate = value;
                OnPropertyChanged(nameof(ReservationDate));
            }
        }

        public string ReservationOwner
        {
            get { return m_objApplication.ReservationOwner; }
            set
            {
                m_objApplication.ReservationOwner = value;
                OnPropertyChanged(nameof(ReservationOwner));
            }
        }

        protected override void OnLoadChildren()
        {
            foreach (DataModelBase objItem in m_objApplication.Items)
            {
                if (objItem is DataModels.Environment objEnvironment)
                    Children.Add(new EnvironmentViewModel(LaunchMenu, objEnvironment, this));
            }
        }
    }
}
