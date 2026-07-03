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
    public partial class ScriptListViewModel : TreeViewItemViewModel
    {
        LaunchMenu m_objLaunchMenu;
        ScriptList m_objScriptList;
        ObservableCollection<ScriptViewModel> m_objScripts = new();

        public ScriptListViewModel(LaunchMenu p_objLaunchMenu, MenuFile p_objMenuFile)
            : base(p_objLaunchMenu)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objScriptList = new ScriptList(p_objMenuFile, "Scripts");
        }

        public ScriptListViewModel(LaunchMenu p_objLaunchMenu, ScriptList p_objScriptList)
            : base(p_objLaunchMenu)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objScriptList = p_objScriptList;
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_objScriptList.Name))
                    return "ScriptList";
                return m_objScriptList.Name;
            }
            set
            {
                m_objScriptList.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<ScriptViewModel> Scripts
        {
            get { return m_objScripts; }
        }

        public ScriptViewModel? AddScript(LaunchMenu p_objLaunchMenu, String p_strScript)
        {
            ScriptViewModel? objScriptViewModel = null;
            Script? objScript = m_objScriptList.MenuFile.AddScript(p_strScript);
            if (objScript != null)
            {
                objScriptViewModel = new ScriptViewModel(p_objLaunchMenu, this, objScript);
                m_objScripts.Add(objScriptViewModel);

                this.OnPropertyChanged(nameof(Scripts));
            }

            return objScriptViewModel;
        }

        public void RemoveScript(ScriptViewModel p_objScriptViewModel)
        {
            if (m_objScripts.Remove(p_objScriptViewModel))
            {
                m_objScriptList.MenuFile.RemoveScript(p_objScriptViewModel.Script);

                this.OnPropertyChanged(nameof(Scripts));
            }
        }

        protected override void OnLoadChildren()
        {
            foreach (DataModelBase objItem in m_objScriptList.Items)
            {
                if (objItem is Script objScript)
                    Children.Add(new ScriptViewModel(LaunchMenu, this, objScript));
            }
        }
    }
}
