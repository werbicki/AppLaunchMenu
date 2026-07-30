using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class EnvironmentViewModel : ViewModelTreeBase<DataModels.Environment>
    {
        protected ObservableCollection<VariableViewModel> m_objAllVariables = new ObservableCollection<VariableViewModel>();

        public EnvironmentViewModel(DataModels.Environment p_objEnvironment, LaunchMenu p_objLaunchMenu)
            : base(p_objEnvironment, p_objLaunchMenu)
        {
            foreach (Variable objVariable in p_objEnvironment.AllVariables)
                m_objAllVariables.Add(new VariableViewModel(objVariable, p_objLaunchMenu, this));
        }

        public EnvironmentViewModel(DataModels.Environment p_objEnvironment, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objEnvironment, p_objLaunchMenu, p_objParent)
        {
            foreach (Variable objVariable in p_objEnvironment.AllVariables)
                m_objAllVariables.Add(new VariableViewModel(objVariable, p_objLaunchMenu, this));
        }

        protected override void OnLoadChildren()
        {
            foreach (VariableViewModel objVariableViewModel in Collection<VariableViewModel, Variable>(this))
                Children.Add(objVariableViewModel);
        }

        internal DataModels.Environment Environment
        {
            get { return DataModel; }
        }

        [DialogContent("Environment Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<VariableViewModel> Variables
        {
            get { return Collection<VariableViewModel, Variable>(this); }
        }

        public ObservableCollection<VariableViewModel> AllVariables
        {
            get { return m_objAllVariables; }
        }
    }
}
