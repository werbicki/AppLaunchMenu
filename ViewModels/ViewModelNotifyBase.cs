using AppLaunchMenu.DataModels;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public abstract class ViewModelNotifyBase : INotifyPropertyChanged
    {
        private readonly DispatcherQueue m_objDispatcherQueue = DispatcherQueue.GetForCurrentThread();
        protected static Dictionary<Type, Type> m_objDataModelViewModelMappings = new();
        private readonly DataModelBase m_objDataModelBase;

        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        protected ViewModelNotifyBase(DataModelBase p_objDataModelBase)
        {
            m_objDataModelBase = p_objDataModelBase;
        }

        public DataModelBase DataModelBase
        {
            get { return m_objDataModelBase; }
        }

        [DialogContent("Name")]
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(DataModelBase.Name))
                    return DataModelBase.GetType().Name;
                return DataModelBase.Name;
            }
            set
            {
                DataModelBase.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [DialogContent("SecurityGroup")]
        public virtual string SecurityGroup
        {
            get { return DataModelBase.SecurityGroup; }
            set
            {
                DataModelBase.SecurityGroup = value;
                OnPropertyChanged(nameof(SecurityGroup));
            }
        }

        [DialogContent("Enabled")]
        public virtual bool Enabled
        {
            get { return DataModelBase.Enabled; }
            set
            {
                DataModelBase.Enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        [DialogContent("Username")]
        public virtual string Username
        {
            get { return DataModelBase.Username; }
            set
            {
                DataModelBase.Username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        [DialogContent("Hostname")]
        public virtual string Hostname
        {
            get { return DataModelBase.Hostname; }
            set
            {
                DataModelBase.Hostname = value;
                OnPropertyChanged(nameof(Hostname));
            }
        }

        [DialogContent("Subnet")]
        public virtual string Subnet
        {
            get { return DataModelBase.Subnet; }
            set
            {
                DataModelBase.Subnet = value;
                OnPropertyChanged(nameof(Subnet));
            }
        }

        [DialogContent("DataCenter")]
        public virtual string DataCenter
        {
            get { return DataModelBase.DataCenter; }
            set
            {
                DataModelBase.DataCenter = value;
                OnPropertyChanged(nameof(DataCenter));
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            // Log.DebugFormat("{0}.{1} = {2}", this.GetType().Name, propertyName, storage);
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                if (m_objDispatcherQueue.HasThreadAccess)
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                else
                {
                    m_objDispatcherQueue.TryEnqueue(() =>
                    {
                        eventHandler(this, new PropertyChangedEventArgs(propertyName));
                    });
                }
            }
        }
    }
}
