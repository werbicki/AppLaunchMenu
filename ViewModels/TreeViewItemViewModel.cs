using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AppLaunchMenu.ViewModels
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public partial class TreeViewItemViewModel : ViewModelBase
    {
        private static readonly TreeViewItemViewModel DummyChild = new();

        private LaunchMenu? m_objLaunchMenu;
        private TreeViewItemViewModel? m_objParent = null;
        private ObservableCollection<TreeViewItemViewModel> m_objChildren = [];
        private string m_strName = "";
        private bool m_blnLazyLoadChildren = false;
        private bool m_blnExpanded = false;
        private bool m_blnSelected = false;

        protected TreeViewItemViewModel(LaunchMenu? p_objLaunchMenu, bool p_blnLazyLoadChildren)
        {
            m_objLaunchMenu = p_objLaunchMenu;

            if (p_blnLazyLoadChildren)
            {
                m_blnLazyLoadChildren = p_blnLazyLoadChildren;
                m_objChildren.Add(DummyChild);
            }

            if (m_objLaunchMenu != null)
                m_objLaunchMenu.PropertyChanged += LaunchMenu_PropertyChanged;
        }

        protected TreeViewItemViewModel(LaunchMenu? p_objLaunchMenu, TreeViewItemViewModel p_objParent)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objParent = p_objParent;

            if (m_objLaunchMenu != null)
                m_objLaunchMenu.PropertyChanged += LaunchMenu_PropertyChanged;
        }

        protected TreeViewItemViewModel(LaunchMenu? p_objLaunchMenu, TreeViewItemViewModel p_objParent, bool p_blnLazyLoadChildren)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objParent = p_objParent;

            if (p_blnLazyLoadChildren)
            {
                m_blnLazyLoadChildren = p_blnLazyLoadChildren;
                m_objChildren.Add(DummyChild);
            }

            if (m_objLaunchMenu != null)
                m_objLaunchMenu.PropertyChanged += LaunchMenu_PropertyChanged;
        }

        // This is used to create the DummyChild instance.
        internal TreeViewItemViewModel()
        {
        }

        private void LaunchMenu_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditMode))
            {
                OnPropertyChanged(nameof(EditMode));
                ReloadChildren();
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        private void Children_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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

        virtual protected void Insert(object p_objItem, int p_intIndex)
        {
            throw new NotImplementedException();
        }

        virtual protected void Remove(object p_objItem, int p_intIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        private bool HasDummyChild
        {
            get { return m_objChildren.Contains(DummyChild); }
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public virtual string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// Returns a reference to the object itself.
        /// </summary>
        public virtual object Item
        {
            get { return this; }
        }

        protected LaunchMenu? LaunchMenu
        {
            get { return m_objLaunchMenu; }
        }

        internal TreeViewItemViewModel? Parent
        {
            get { return m_objParent; }
            set { m_objParent = value; }
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get
            {
                if ((m_blnLazyLoadChildren) || ((m_objChildren.Count == 1) && (HasDummyChild)))
                {
                    m_blnLazyLoadChildren = false;
                    if (HasDummyChild)
                        m_objChildren.Clear();

                    LoadChildren();
                }

                return m_objChildren;
            }
        }

        public virtual bool EditMode
        {
            get
            {
                bool blnEditMode = false;
                TreeViewItemViewModel? objTreeViewItemViewModel = Parent;

                while ((objTreeViewItemViewModel != null) && (objTreeViewItemViewModel.Parent != null))
                    objTreeViewItemViewModel = objTreeViewItemViewModel.Parent;

                if (objTreeViewItemViewModel != null)
                    return objTreeViewItemViewModel.EditMode;

                return blnEditMode;
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

                // Lazy load the child items, if necessary.
                if (m_blnLazyLoadChildren)
                    LoadChildren();
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

        protected void ReloadChildren()
        {
            m_objChildren.CollectionChanged -= Children_CollectionChanged;
            m_blnLazyLoadChildren = true;
            Children.Clear();

            LoadChildren();
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
            m_blnLazyLoadChildren = false;
            if (HasDummyChild)
                Children.Remove(DummyChild);

            m_objChildren.CollectionChanged += Children_CollectionChanged;
        }
    }
}