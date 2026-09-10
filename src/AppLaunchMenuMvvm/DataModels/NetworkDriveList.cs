using AppLaunchMenu.DataAccess;
using System;
using System.Linq;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class NetworkDriveList : DataModelBase, IElementName
    {
        DataModelCollection<NetworkDrive> m_objNetworkDrives;

        public NetworkDriveList(LaunchMenuFile p_objMenuFile, XmlNode p_objFolderNode)
            : base(p_objMenuFile, new Type[] { typeof(NetworkDrive) }, p_objFolderNode)
        {
            m_objNetworkDrives = new(this, null);

            UpdateItems();
        }

        public NetworkDriveList(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, new Type[] { typeof(NetworkDrive) }, p_strName)
        {
            m_objNetworkDrives = new(this, null);
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(NetworkDriveList); }
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

        public NetworkDrive[] NetworkDrives
        {
            get { return m_objNetworkDrives.ToArray(); }
        }
    }
}
