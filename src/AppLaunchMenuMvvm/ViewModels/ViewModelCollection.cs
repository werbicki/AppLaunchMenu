using AppLaunchMenu.DataModels;
using System.Collections.Specialized;

namespace AppLaunchMenu.ViewModels
{
    public abstract class ViewModelCollection<DataModelT, ViewModelT> : INotifyCollectionChanged
    {
        /// <summary>
        /// Multicast event for collection change notifications.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        DataModelCollection<DataModelT> m_objItems;

        public ViewModelCollection(DataModelCollection<DataModelT> p_objItems)
            : base()
        {
            m_objItems = p_objItems;
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var eventHandler = CollectionChanged;
            if (eventHandler != null)
                eventHandler(this, e);
        }
    }
}
