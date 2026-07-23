using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public abstract class ViewModelBase<T> : ViewModelNotifyBase
        where T : DataModelBase
    {
        private readonly T m_objDataModel;
        private readonly LaunchMenu m_objLaunchMenu;
        protected Dictionary<Type, object> m_objViewModels = new();
        protected Dictionary<Type, object> m_objCollections = new();

        protected ViewModelBase(T p_objDataModel, LaunchMenu objLaunchMenu)
            : base(p_objDataModel)
        {
            m_objDataModel = p_objDataModel;
            m_objLaunchMenu = objLaunchMenu;

            m_objLaunchMenu.PropertyChanged += LaunchMenu_PropertyChanged;
        }

        private void LaunchMenu_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditMode))
                OnPropertyChanged(nameof(EditMode));
        }

        protected T DataModel
        {
            get { return m_objDataModel; }
        }

        protected LaunchMenu LaunchMenu
        {
            get { return m_objLaunchMenu; }
        }

        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(DataModel.Name))
                    return DataModel.GetType().Name;
                return DataModel.Name;
            }
            set
            {
                DataModel.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public virtual bool EditMode
        {
            get
            {
                if (LaunchMenu != null)
                    return LaunchMenu.EditMode;
                else
                    return false;
            }
        }

        protected TViewModel ViewModel<TViewModel, TDataModel>(TDataModel p_objDataModel)
        {
            if (!m_objViewModels.ContainsKey(typeof(TViewModel)))
            {
                object[] arrConstructorArgs = new object[] { p_objDataModel, LaunchMenu };
                TViewModel? objViewModel = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), arrConstructorArgs);

                if (objViewModel == null)
                    throw new AccessViolationException();

                m_objViewModels.Add(typeof(TViewModel), objViewModel);
            }

            return (TViewModel)m_objViewModels[typeof(TViewModel)];
        }

        protected ObservableCollection<TViewModel> Collection<TViewModel, TDataModel>()
        {
            if (!m_objCollections.ContainsKey(typeof(TViewModel)))
            {
                ObservableCollection<TViewModel> objColleciton = new();

                foreach (DataModelBase objDataModel in DataModel.Items)
                {
                    if (objDataModel.GetType() == typeof(TDataModel))
                    {
                        object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu };
                        TViewModel? objViewModel = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), arrConstructorArgs);

                        if (objViewModel == null)
                            throw new AccessViolationException();

                        objColleciton.Add(objViewModel);
                    }
                }

                objColleciton.CollectionChanged += ViewModelBase_OnCollectionChanged;

                m_objCollections.Add(typeof(TViewModel), objColleciton);
            }

            return (ObservableCollection<TViewModel>)m_objCollections[typeof(TViewModel)];
        }

        protected void ViewModelBase_OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == NotifyCollectionChangedAction.Add) && (e.NewItems != null))
            {
                foreach (ViewModelNotifyBase objViewModel in e.NewItems)
                    DataModel.InsertItem(objViewModel.DataModelBase);

            }
            else if ((e.Action == NotifyCollectionChangedAction.Remove) && (e.OldItems != null))
            {
                foreach (ViewModelNotifyBase objViewModel in e.OldItems)
                    DataModel.RemoveItem(objViewModel.DataModelBase);

            }
        }

        public TViewModel NewChild<TViewModel, TDataModel>(String p_strItemName = "")
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            TDataModel objDataModel = DataModel.NewItem<TDataModel>(p_strItemName);

            object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu };
            TViewModel? objViewModel = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), arrConstructorArgs);

            if (objViewModel == null)
                throw new AccessViolationException();

            return objViewModel;
        }

        public TViewModel AddChild<TViewModel, TDataModel>(String p_strItemName = "")
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            TDataModel objDataModel = DataModel.NewItem<TDataModel>(p_strItemName);

            object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu };
            TViewModel? objViewModel = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), arrConstructorArgs);

            if (objViewModel == null)
                throw new AccessViolationException();

            ObservableCollection<TViewModel> m_objCollection = Collection<TViewModel, TDataModel>();

            m_objCollection.Add(objViewModel);
            OnPropertyChanged(typeof(TDataModel).Name + "s");

            return objViewModel;
        }

        public virtual void AddChild<TViewModel, TDataModel>(TViewModel p_objViewModel)
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            ObservableCollection<TViewModel> m_objCollection = Collection<TViewModel, TDataModel>();

            m_objCollection.Add(p_objViewModel);
            OnPropertyChanged(typeof(TDataModel).Name + "s");
        }

        protected virtual void InsertChild<TViewModel, TDataModel>(TViewModel p_objViewModel, int p_intIndex)
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            ObservableCollection<TViewModel> m_objCollection = Collection<TViewModel, TDataModel>();

            if ((p_intIndex >= 0) && (p_intIndex < m_objCollection.Count))
                m_objCollection.Insert(p_intIndex, p_objViewModel);
            else
                m_objCollection.Add(p_objViewModel);

            OnPropertyChanged(typeof(TDataModel).Name + "s");
        }

        protected void InsertChild(object p_objViewModel, int p_intIndex)
        {
            if (m_objCollections.ContainsKey(p_objViewModel.GetType()))
            {
                IList objList = (IList)m_objCollections[p_objViewModel.GetType()];

                objList.Insert(p_intIndex, p_objViewModel);
            }
        }

        public void RemoveChild<TViewModel, TDataModel>(TViewModel p_objViewModel)
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            ObservableCollection<TViewModel> m_objCollection = Collection<TViewModel, TDataModel>();

            if (m_objCollection.Remove(p_objViewModel))
            {
                DataModel.DeleteItem<TDataModel>(p_objViewModel.DataModel);

                OnPropertyChanged(typeof(TDataModel).Name + "s");
            }
        }

        protected void RemoveChild(object p_objViewModel)
        {
            if (m_objCollections.ContainsKey(p_objViewModel.GetType()))
            {
                IList objList = (IList)m_objCollections[p_objViewModel.GetType()];

                objList.Remove(p_objViewModel);
            }
        }
    }
}
