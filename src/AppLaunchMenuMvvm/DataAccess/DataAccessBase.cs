using System;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public abstract class DataAccessBase : DataModelBase, IComparable
    {
        public class DataChangedEventArgs : EventArgs
        {
            public DataChangedEventArgs()
            {
            }
        }

        public delegate void DataChangedEventHandler(object? sender, DataChangedEventArgs e);
        public event DataChangedEventHandler? DataChanged;

        protected virtual void OnDataChanged()
        {
            var eventHandler = DataChanged;
            if (eventHandler != null)
                eventHandler(this, new DataChangedEventArgs());
        }

        private XmlDocument m_objXmlDocument;
        protected bool m_blnIsDirty = false;

        protected DataAccessBase(Type[] p_objXmlChildNodeTypes, XmlDocument p_objXmlDocument)
             : base(p_objXmlChildNodeTypes, p_objXmlDocument)
        {
            m_objXmlDocument = p_objXmlDocument;

            m_objXmlDocument.NodeChanged += XmlDocument_NodeChanged;
            m_objXmlDocument.NodeInserted += XmlDocument_NodeChanged;
            m_objXmlDocument.NodeRemoved += XmlDocument_NodeChanged;
        }

        protected void XmlDocument_NodeChanged(object sender, XmlNodeChangedEventArgs e)
        {
            if ((e.Action == XmlNodeChangedAction.Insert)
                && (e.Node != null)
                )
            {
                if (e.Node.Name != "Environment")
                    IsDirty = true;
            }
            else
                IsDirty = true;
        }

        internal XmlDocument XmlDocument
        {
            get { return m_objXmlDocument; }
        }

        public bool IsDirty
        {
            get { return m_blnIsDirty; }
            internal set
            {
                m_blnIsDirty = value;
                OnDataChanged();
            }
        }
    }
}
