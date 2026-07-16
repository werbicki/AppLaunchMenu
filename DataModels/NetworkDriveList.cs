using AppLaunchMenu.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class NetworkDriveList : DataModelBase
    {
        DataModelCollection<NetworkDrive> m_objNetworkDrives;

        public NetworkDriveList(MenuFile p_objMenuFile, XmlNode p_objFolderNode)
            : base(p_objMenuFile, new Type[] { typeof(NetworkDrive) }, p_objFolderNode)
        {
            m_objNetworkDrives = new(this, null);

            UpdateItems();
        }

        public NetworkDriveList(MenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(NetworkDrive) }, p_strName)
        {
            m_objNetworkDrives = new(this, null);
        }

        internal static string ElementName
        {
            get { return nameof(NetworkDriveList); }
        }

        protected override string _ElementName
        {
            get { return ElementName; }
        }

        protected override void UpdateItems()
        {
            m_objNetworkDrives.Clear();

            foreach (DataModelBase objObject in Items)
            {
                if (objObject is NetworkDrive)
                    m_objNetworkDrives.Add((NetworkDrive)objObject);
            }
        }

        public NetworkDrive[] Drives
        {
            get { return m_objNetworkDrives.ToArray(); }
        }
    }
}
