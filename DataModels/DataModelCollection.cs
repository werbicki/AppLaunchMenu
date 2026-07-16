using AppLaunchMenu.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class DataModelCollection<T> : IEnumerable<T>
    {
        protected DataModelBase m_objObject;
        protected List<T> m_objNodes = [];

        public DataModelCollection(DataModelBase p_objObject, List<XmlNode>? p_objNodes)
        {
            m_objObject = p_objObject;

            if (p_objNodes != null)
                Initialize(p_objNodes);
        }

        protected void Initialize(List<XmlNode> p_objNodes)
        {
            m_objNodes = new List<T>(p_objNodes.Count);

            for (int i = 0; i < p_objNodes.Count; i++)
                Add(p_objNodes[i]);
        }

        public void Clear()
        {
            m_objNodes.Clear();
        }

        public void Add(XmlNode p_objNode)
        {
            T? objItem = (T?)Activator.CreateInstance(typeof(T), m_objObject.MenuFile, p_objNode);

            if (objItem != null)
                m_objNodes.Add(objItem);
        }

        public void Add(T p_objItem)
        {
            m_objNodes.Add(p_objItem);
        }

        public int Count
        {
            get
            {
                if (m_objNodes != null)
                    return m_objNodes.Count;

                return 0;
            }
        }

        public T? Item(int p_intIndex)
        {
            if ((m_objNodes != null) && (p_intIndex >= 0) && (p_intIndex < m_objNodes.Count))
                return m_objNodes[p_intIndex];

            return default;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new NodeEnumerator<T>(m_objNodes);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new NodeEnumerator<T>(m_objNodes);
        }
    }

    public class NodeEnumerator<T>
        : IEnumerator<T>
    {
        protected List<T> m_objNodes;
        protected int m_intIndex;

        public NodeEnumerator(List<T> p_objNodes)
        {
            m_objNodes = p_objNodes;
            m_intIndex = -1;
        }

        public T Current
        {
            get
            {
                return m_objNodes[m_intIndex];
            }
        }

        object? IEnumerator.Current
        {
            get
            {
                return m_objNodes[m_intIndex];
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (++m_intIndex < m_objNodes.Count)
                return true;
            return false;
        }

        public void Reset()
        {
            m_intIndex = (-1);
        }
    }
}
