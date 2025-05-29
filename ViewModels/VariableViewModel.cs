using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class VariableViewModel : TreeViewItemViewModel
    {
        protected DataModels.Environment m_objEnvironment;
        protected Variable m_objVariable;

        public VariableViewModel(LaunchMenu? p_objLaunchMenu, Variable p_objVariable, EnvironmentViewModel p_objEnvironment)
            : base(p_objLaunchMenu, p_objEnvironment, false)
        {
            m_objVariable = p_objVariable;
            m_objEnvironment = p_objEnvironment.Environment;
        }

        internal Variable Variable
        {
            get { return m_objVariable; }
        }

        public override string Name
        {
            get { return m_objVariable.Name; }
            set
            {
                m_objVariable.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Environment
        {
            get { return m_objEnvironment.Name; }
        }

        public string Description
        {
            get { return m_objVariable.Description; }
            set
            {
                m_objVariable.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Group
        {
            get { return m_objVariable.Group; }
            set
            {
                m_objVariable.Group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        public string Value
        {
            get { return m_objVariable.Value; }
            set
            {
                m_objVariable.Value = value;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(ExpandedValue));
            }
        }

        public string ExpandedValue
        {
            get { return m_objEnvironment.ExpandVariable(m_objVariable.Value); }
        }

        protected override void LoadChildren()
        {
            base.LoadChildren();
        }
    }
}
