using AppLaunchMenu.DataModels;
using Microsoft.UI.Xaml;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Windows.UI.Text;

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

        public Type[] ChildNodeTypes
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

        public bool IsVisible
        {
            get;
        }

        public bool ShowItem
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

        public ViewModelNotifyBase NewChild(Type p_objType, String p_strItemName, ITreeViewItem p_objParent);

        public void AddChild(ViewModelNotifyBase p_objViewModel);

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
            else if (e.PropertyName == nameof(TreeViewItemMinWidth))
                OnPropertyChanged(nameof(TreeViewItemMinWidth));
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

        [DialogContent("Enabled")]
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                OnPropertyChanged(nameof(IsVisible));
                OnPropertyChanged(nameof(ShowItem));
            }
        }

        public ITreeViewItem? Parent
        {
            get { return m_objParent; }
            set { m_objParent = value; }
        }

        private MenuViewModel? MenuViewModel
        {
            get
            {
                ITreeViewItem? objTreeViewItemViewModel = this;

                while ((objTreeViewItemViewModel != null)
                    && (objTreeViewItemViewModel.GetType() != typeof(MenuViewModel))
                    && (objTreeViewItemViewModel.Parent != null)
                    )
                    objTreeViewItemViewModel = objTreeViewItemViewModel.Parent;

                if ((objTreeViewItemViewModel != null)
                    && (objTreeViewItemViewModel is MenuViewModel)
                    )
                    return ((MenuViewModel)objTreeViewItemViewModel);

                return null;
            }
        }

        public virtual GridLength TreeViewItemWidth
        {
            get
            {
                GridLength objTreeViewItemWidth = new GridLength(200.0, GridUnitType.Auto);
                ITreeViewItem? objTreeViewItemViewModel = MenuViewModel;

                if (objTreeViewItemViewModel != null)
                    objTreeViewItemWidth = objTreeViewItemViewModel.TreeViewItemWidth;

                return objTreeViewItemWidth;
            }
            set
            {
                ITreeViewItem? objTreeViewItemViewModel = MenuViewModel;

                if (objTreeViewItemViewModel != null)
                {
                    objTreeViewItemViewModel.TreeViewItemWidth = value;
                    OnPropertyChanged(nameof(TreeViewItemWidth));
                }
            }
        }

        public virtual double TreeViewItemMinWidth
        {
            get
            {
                double dblTreeViewItemMinWidth = 200.0;
                ITreeViewItem? objTreeViewItemViewModel = MenuViewModel;

                if (objTreeViewItemViewModel != null)
                    dblTreeViewItemMinWidth = ((MenuViewModel)objTreeViewItemViewModel).TreeViewItemMinWidth;

                return dblTreeViewItemMinWidth;
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

        protected TViewModel ViewModel<TViewModel, TDataModel>(TDataModel p_objDataModel, ITreeViewItem p_objParent)
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
        {
            if (!m_objViewModels.ContainsKey(typeof(TViewModel)))
            {
                object[] arrConstructorArgs = new object[] { p_objDataModel, LaunchMenu, p_objParent };
                TViewModel? objViewModel = (TViewModel?)Activator.CreateInstance(typeof(TViewModel), arrConstructorArgs);

                if (objViewModel == null)
                    throw new AccessViolationException();

                m_objViewModels.Add(typeof(TViewModel), objViewModel);
            }

            return (TViewModel)m_objViewModels[typeof(TViewModel)];
        }

        protected ObservableCollection<TViewModel> Collection<TViewModel, TDataModel>(ITreeViewItem p_objParent)
            where TViewModel : ViewModelBase<TDataModel>
            where TDataModel : DataModelBase
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

        protected IList Collection(Type p_objViewModelType, Type p_objDataModelType)
        {
            if (!m_objCollections.ContainsKey(p_objViewModelType))
            {
                Type objOpenTemplateType = typeof(ObservableCollection<>);
                Type[] arrTypeArguments = { p_objViewModelType };
                Type objCollectionType = objOpenTemplateType.MakeGenericType(arrTypeArguments);
                IList? objCollection = (IList?)Activator.CreateInstance(objCollectionType);

                if (objCollection == null)
                    throw new AccessViolationException();

                foreach (DataModelBase objDataModel in DataModel.Items)
                {
                    if (objDataModel.GetType() == p_objDataModelType)
                    {
                        object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu };
                        ViewModelNotifyBase? objViewModel = (ViewModelNotifyBase?)Activator.CreateInstance(p_objViewModelType, arrConstructorArgs);

                        if (objViewModel == null)
                            throw new AccessViolationException();

                        objCollection.Add(objViewModel);
                    }
                }

                ((INotifyCollectionChanged)objCollection).CollectionChanged += ViewModelBase_OnCollectionChanged;

                m_objCollections.Add(p_objViewModelType, objCollection);
            }

            return (IList)m_objCollections[p_objViewModelType];
        }

        public ViewModelNotifyBase NewChild(Type p_objDataModelType, String p_strItemName, ITreeViewItem p_objParent)
        {
            DataModelBase objDataModel = (DataModelBase)DataModel.NewItem(p_objDataModelType, p_strItemName);
            Type objViewModelType = ViewModelForDataModel(p_objDataModelType);

            object[] arrConstructorArgs = new object[] { objDataModel, LaunchMenu, p_objParent };
            ViewModelNotifyBase? objViewModel = (ViewModelNotifyBase?)Activator.CreateInstance(objViewModelType, arrConstructorArgs);

            if (objViewModel == null)
                throw new AccessViolationException();

            return objViewModel;
        }

        /*
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
        */

        public virtual void AddChild(ViewModelNotifyBase p_objViewModel)
        {
            var m_objCollection = Collection(p_objViewModel.GetType(), p_objViewModel.DataModelBase.GetType());

            m_objCollection.Add(p_objViewModel);
            OnPropertyChanged(p_objViewModel.DataModelBase.GetType().Name + "s");
        }

        /*
        public override void AddChild<TViewModel, TDataModel>(TViewModel p_objViewModel)
        {
            base.AddChild<TViewModel, TDataModel>(p_objViewModel);

            Children.Add((ITreeViewItem)p_objViewModel);
        }
        */

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
                ObservableCollection<ITreeViewItem> Children;

                // Children property will decide if LoadChildren() needs to be called.
                foreach (ITreeViewItem objTreeViewItemViewModel in m_objChildren)
                    Children = objTreeViewItemViewModel.Children;
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