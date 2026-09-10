using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public class VariableViewModel : ViewModelTreeBase<Variable>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get { return []; }
        }

        protected DataModels.Environment m_objEnvironment;

        public VariableViewModel(Variable p_objVariable, ILaunchMenu p_objLaunchMenu, EnvironmentViewModel p_objEnvironmentViewModel)
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

        [DialogContent("Variable Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        [DialogContent("Description")]
        public string Description
        {
            get { return DataModel.Description; }
            set
            {
                DataModel.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        [DialogContent("Group")]
        public string Group
        {
            get { return DataModel.Group; }
            set
            {
                DataModel.Group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        [DialogContent("Value")]
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
