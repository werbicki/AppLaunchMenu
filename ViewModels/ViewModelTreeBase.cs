using AppLaunchMenu.DataModels;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace AppLaunchMenu.ViewModels
{
    public class EmptyViewModel : ViewModelTreeBase<Empty>
    {
        private static ITreeViewItem? m_objEmptyChild;

        public EmptyViewModel(LaunchMenu p_objLaunchMenu)
            : base(new Empty(), p_objLaunchMenu)
        {
        }

        public override string Name
        {
            get { return DataModel.Name; }
        }

        internal static ITreeViewItem? EmptyChild
        {
            get { return m_objEmptyChild; }
            set { m_objEmptyChild = value; }
        }
    }

    public interface ITreeViewItem : INotifyPropertyChanged
    {
        public string Name
        {
            get;
        }

        public ITreeViewItem Item
        {
            get;
        }

        public bool EditMode
        {
            get;
        }

        public ITreeViewItem? Parent
        {
            get;
            set;
        }

        public GridLength TreeViewItemWidth
        {
            get;
            set;
        }

        public bool Expanded
        {
            get;
        }

        public bool IsExpanded
        {
            get;
            set;
        }

        public void LoadChildren();

        public ObservableCollection<ITreeViewItem> Children
        {
            get;
        }
    }

    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public partial class ViewModelTreeBase<T> : ViewModelBase<T>, ITreeViewItem
        where T : DataModelBase
    {
        private ITreeViewItem? m_objParent = null;
        private ObservableCollection<ITreeViewItem> m_objChildren = [];
        private bool m_blnLazyLoadChildren = false;
        private bool m_blnExpanded = false;
        private bool m_blnSelected = false;

        protected ViewModelTreeBase(T p_objDataModel, LaunchMenu p_objLaunchMenu, bool p_blnLazyLoadChildren = false)
            : base(p_objDataModel, p_objLaunchMenu)
        {
            m_blnLazyLoadChildren = p_blnLazyLoadChildren;

            if (EmptyChild != null)
                m_objChildren.Add(EmptyChild);
        }

        protected ViewModelTreeBase(T p_objDataModel, LaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent, bool p_blnLazyLoadChildren = false)
            : base(p_objDataModel, p_objLaunchMenu)
        {
            m_objParent = p_objParent;
            m_blnLazyLoadChildren = p_blnLazyLoadChildren;

            if (EmptyChild != null)
                m_objChildren.Add(EmptyChild);

            ITreeViewItem? objTreeViewItemViewModel = Parent;

            while ((objTreeViewItemViewModel != null) && (objTreeViewItemViewModel.Parent != null))
                objTreeViewItemViewModel = objTreeViewItemViewModel.Parent;

            if (objTreeViewItemViewModel != null)
                objTreeViewItemViewModel.PropertyChanged += MenuViewModel_PropertyChanged;
        }

        private void MenuViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TreeViewItemWidth))
                OnPropertyChanged(nameof(TreeViewItemWidth));
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if ((e.Action == NotifyCollectionChangedAction.Add) && (e.NewItems != null))
            {
                foreach (object objItem in e.NewItems)
                    InsertChild(objItem, e.NewStartingIndex);
            }
            else if ((e.Action == NotifyCollectionChangedAction.Remove) && (e.OldItems != null))
            {
                foreach (object objItem in e.OldItems)
                    RemoveChild(objItem);
            }
        }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        private ITreeViewItem? EmptyChild
        {
            get { return EmptyViewModel.EmptyChild; }
        }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        private bool HasEmptyChild
        {
            get
            {
                if (EmptyChild != null)
                    return m_objChildren.Contains(EmptyChild);
                return false;
            }
        }

        public virtual ITreeViewItem Item
        {
            get { return this; }
        }

        public ITreeViewItem? Parent
        {
            get { return m_objParent; }
            set { m_objParent = value; }
        }

        public virtual GridLength TreeViewItemWidth
        {
            get
            {
                GridLength objTreeViewItemWidth = new(100.0);
                ITreeViewItem? objTreeViewItemViewModel = Parent;

                while ((objTreeViewItemViewModel != null)
                    && (objTreeViewItemViewModel is not MenuViewModel)
                    && (objTreeViewItemViewModel.Parent != null)
                    )
                    objTreeViewItemViewModel = objTreeViewItemViewModel.Parent;

                if (objTreeViewItemViewModel != null)
                    return objTreeViewItemViewModel.TreeViewItemWidth;

                return objTreeViewItemWidth;
            }
            set
            {
                ITreeViewItem? objTreeViewItemViewModel = Parent;

                while ((objTreeViewItemViewModel != null)
                    && (objTreeViewItemViewModel is not MenuViewModel)
                    && (objTreeViewItemViewModel.Parent != null)
                    )
                    objTreeViewItemViewModel = objTreeViewItemViewModel.Parent;

                if (objTreeViewItemViewModel != null)
                    objTreeViewItemViewModel.TreeViewItemWidth = value;
            }
        }

        public virtual bool Expanded
        {
            get { return m_blnExpanded; }
            set { m_blnExpanded = value; }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return Expanded; }
            set
            {
                if (value != Expanded)
                {
                    Expanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }

                // Expand all the way up to the root.
                if (Expanded && m_objParent != null)
                    m_objParent.IsExpanded = true;
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return m_blnSelected; }
            set
            {
                if (value != m_blnSelected)
                {
                    m_blnSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        protected ObservableCollection<TViewModel> Collection<TViewModel, TDataModel>(ITreeViewItem p_objParent)
        {
            if (!m_objCollections.ContainsKey(typeof(TViewModel)))
            {
                ObservableCollection<TViewModel> objColleciton = new();

                foreach (DataModelBase objDataModel in DataModel.Items)
                {
                    if (objDataModel.GetType() == typeof(TDataModel))
                    {
                        object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu, p_objParent };
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

        public TViewModel NewChild<TViewModel, TDataModel>(String p_strItemName, ITreeViewItem p_objParent)
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            TDataModel objDataModel = DataModel.NewItem<TDataModel>(p_strItemName);

            object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu, p_objParent };
            TViewModel? objViewModel = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), arrConstructorArgs);

            if (objViewModel == null)
                throw new AccessViolationException();

            return objViewModel;
        }

        public override void AddChild<TViewModel, TDataModel>(TViewModel p_objViewModel)
        {
            base.AddChild<TViewModel, TDataModel>(p_objViewModel);

            Children.Add((ITreeViewItem)p_objViewModel);
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<ITreeViewItem> Children
        {
            get
            {
                if ((m_objChildren.Count == 1) && (HasEmptyChild))
                    LoadChildren();

                return m_objChildren;
            }
        }

        protected void ReloadChildren()
        {
            m_objChildren.Clear();

            if (EmptyChild != null)
                m_objChildren.Add(EmptyChild);

            LoadChildren();
        }

        public void LoadChildren()
        {
            m_objChildren.CollectionChanged -= Children_CollectionChanged;

            if ((EmptyChild != null) && (HasEmptyChild))
                m_objChildren.Remove(EmptyChild);

            OnLoadChildren();

            if (!m_blnLazyLoadChildren)
            {
                foreach (ITreeViewItem objTreeViewItemViewModel in m_objChildren)
                    objTreeViewItemViewModel.LoadChildren();
            }

            m_objChildren.CollectionChanged += Children_CollectionChanged;

            OnPropertyChanged(nameof(Children));
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void OnLoadChildren()
        {
        }
    }
}