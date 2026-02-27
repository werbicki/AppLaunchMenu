using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public partial class FolderViewModel : TreeViewItemViewModel
    {
        private readonly Folder m_objFolder;

        public FolderViewModel(LaunchMenu? p_objLaunchMenu, Folder p_objFolder)
            : base(p_objLaunchMenu)
        {
            m_objFolder = p_objFolder;
        }

        public FolderViewModel(LaunchMenu? p_objLaunchMenu, Folder p_objFolder, FolderViewModel p_objFolderViewModel)
            : base(p_objLaunchMenu, p_objFolderViewModel)
        {
            m_objFolder = p_objFolder;
        }

        protected override void Insert(object p_objItem, int p_intIndex)
        {
            if (p_objItem is FolderViewModel objFolderViewModel)
            {
                InsertFolder(objFolderViewModel, p_intIndex);
                objFolderViewModel.Parent = this;
            }
        }

        protected override void Remove(object p_objItem, int p_intIndex)
        {
            if (p_objItem is FolderViewModel objFolderViewModel)
            {
                RemoveFolder(objFolderViewModel);
                objFolderViewModel.Parent = null;
            }
        }

        internal FolderViewModel? CreateFolder(String p_strFolderName)
        {
            FolderViewModel? objFolderViewModel = null;
            Folder? objFolder = m_objFolder.CreateFolder(p_strFolderName);
            if (objFolder != null)
            {
                objFolderViewModel = new FolderViewModel(LaunchMenu, objFolder, this);
                //Children.Add(new FolderViewModel(objFolder, this));
            }

            return objFolderViewModel;
        }

        internal FolderViewModel InsertFolder(FolderViewModel p_objFolderViewModel, int p_intIndex)
        {
            m_objFolder.Insert(p_objFolderViewModel.Folder, p_intIndex);

            return p_objFolderViewModel;
        }

        internal void RemoveFolder(FolderViewModel p_objFolderViewModel)
        {
            if (Children.Remove(p_objFolderViewModel))
                m_objFolder.Remove(p_objFolderViewModel.Folder);
        }

        internal ApplicationViewModel? CreateApplication(String p_strApplicationName)
        {
            ApplicationViewModel? objApplicationViewModel = null;
            Application? objApplication = m_objFolder.CreateApplication(p_strApplicationName);
            if (objApplication != null)
                objApplicationViewModel = new ApplicationViewModel(LaunchMenu, objApplication, this);

            return objApplicationViewModel;
        }

        internal VariableViewModel? CreateVariable(String p_strVariableName)
        {
            VariableViewModel? objVariableViewModel = null;
            Variable? objVariable = m_objFolder.CreateVariable(p_strVariableName);
            if (objVariable != null)
                objVariableViewModel = new VariableViewModel(LaunchMenu, objVariable, new EnvironmentViewModel(LaunchMenu, m_objFolder.Environment));

            return objVariableViewModel;
        }

        internal Folder Folder
        {
            get { return m_objFolder; }
        }

        public override string Name
        {
            get { return m_objFolder.Name; }
            set
            {
                m_objFolder.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override bool Expanded
        {
            get { return m_objFolder.Expanded; }
            set
            {
                m_objFolder.Expanded = value;
                OnPropertyChanged(nameof(Expanded));
            }
        }

        public EnvironmentViewModel Environment
        {
            get { return new EnvironmentViewModel(LaunchMenu, m_objFolder.Environment); }
        }

        protected override void OnLoadChildren()
        {
            foreach (DataModelBase objItem in m_objFolder.Items)
            {
                if (objItem is DataModels.Environment objEnvironment)
                    Children.Add(new EnvironmentViewModel(LaunchMenu, objEnvironment, this));
            }

            foreach (DataModelBase objItem in m_objFolder.Items)
            {
                if (objItem is Folder objFolder)
                    Children.Add(new FolderViewModel(LaunchMenu, objFolder, this));
            }

            foreach (DataModelBase objItem in m_objFolder.Items)
            {
                if (objItem is Application objApplication)
                    Children.Add(new ApplicationViewModel(LaunchMenu, objApplication, this));
            }
        }
    }
}
