using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class VariableViewModel : ViewModelTreeBase<Variable>
    {
        protected DataModels.Environment m_objEnvironment;

        public VariableViewModel(Variable p_objVariable, LaunchMenu p_objLaunchMenu, EnvironmentViewModel p_objEnvironmentViewModel)
            : base(p_objVariable, p_objLaunchMenu, p_objEnvironmentViewModel)
        {
            m_objEnvironment = p_objEnvironmentViewModel.Environment;
        }

        internal Variable Variable
        {
            get { return DataModel; }
        }

        public string Environment
        {
            get { return m_objEnvironment.Name; }
        }

        public string Description
        {
            get { return DataModel.Description; }
            set
            {
                DataModel.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Group
        {
            get { return DataModel.Group; }
            set
            {
                DataModel.Group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        public string Value
        {
            get { return DataModel.Value; }
            set
            {
                DataModel.Value = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(ExpandedValue));
            }
        }

        public string ExpandedValue
        {
            get { return m_objEnvironment.ExpandVariable(DataModel.Value); }
        }
    }
}
