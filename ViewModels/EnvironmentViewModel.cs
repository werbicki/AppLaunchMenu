using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class EnvironmentViewModel : TreeViewItemViewModel
    {
        DataModels.Environment m_objEnvironment;
        protected ObservableCollection<VariableViewModel> m_objVariables = new ObservableCollection<VariableViewModel>();

        public EnvironmentViewModel(LaunchMenu? p_objLaunchMenu, DataModels.Environment p_objEnvironment)
        {
            m_objEnvironment = p_objEnvironment;

            foreach (Variable objVariable in p_objEnvironment)
                m_objVariables.Add(new VariableViewModel(p_objLaunchMenu, objVariable, this));

            m_objVariables.CollectionChanged += Variables_CollectionChanged;
        }

        public EnvironmentViewModel(LaunchMenu? p_objLaunchMenu, DataModels.Environment p_objEnvironment, TreeViewItemViewModel p_objTreeViewItemViewModel)
            : base(p_objLaunchMenu, p_objTreeViewItemViewModel)
        {
            m_objEnvironment = p_objEnvironment;
        }

        private void Variables_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == NotifyCollectionChangedAction.Add) && (e.NewItems != null))
            {
                foreach (object objItem in e.NewItems)
                    Insert(objItem, e.NewStartingIndex);
            }
            else if ((e.Action == NotifyCollectionChangedAction.Remove) && (e.OldItems != null))
            {
                foreach (object objItem in e.OldItems)
                    Remove(objItem, e.NewStartingIndex);
            }
        }

        protected override void Insert(object p_objItem, int p_intIndex)
        {
            if (p_objItem is VariableViewModel objVariableViewModel)
                m_objEnvironment.Insert(objVariableViewModel.Variable, p_intIndex);
        }

        protected override void Remove(object p_objItem, int p_intIndex)
        {
            if (p_objItem is VariableViewModel objVariableViewModel)
                m_objEnvironment.Remove(objVariableViewModel.Variable);
        }

        internal DataModels.Environment Environment
        {
            get { return m_objEnvironment; }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_objEnvironment.Name))
                    return "Environment";
                return m_objEnvironment.Name;
            }
            set
            {
                m_objEnvironment.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<VariableViewModel> Variables
        {
            get { return m_objVariables; }
        }

        protected override void OnLoadChildren()
        {
            foreach (DataModelBase objItem in m_objEnvironment.Items)
            {
                if (objItem is Variable objVariable)
                    Children.Add(new VariableViewModel(LaunchMenu, objVariable, this));
            }
        }
    }
}
