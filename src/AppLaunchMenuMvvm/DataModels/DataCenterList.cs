using AppLaunchMenu.DataAccess;
using System;
using System.Linq;
using System.Xml;

namespace AppLaunchMenu.DataModels
{
    public class DataCenterList : DataModelBase, IElementName
    {
        protected override ChildElementType[] ChildElementTypes
        {
            get
            {
                return
                [
                    new() { Type = typeof(DataCenter), ElementType = ElementTypeEnum.OneOrMore }
                ];
            }
        }

        DataModelCollection<DataCenter> m_objDataCenters;

        public DataCenterList(LaunchMenuFile p_objMenuFile, XmlNode p_objFolderNode)
            : base(p_objMenuFile, p_objFolderNode)
        {
            m_objDataCenters = new(this, null);

            UpdateItems();
        }

        public DataCenterList(LaunchMenuFile p_objMenuFile, string p_strName)
            : base(p_objMenuFile, p_strName)
        {
            m_objDataCenters = new(this, null);
        }

        internal override string _ElementName
        {
            get { return ElementName; }
        }

        public static string ElementName
        {
            get { return nameof(DataCenterList); }
        }

        protected override void UpdateItems()
        {
            m_objDataCenters.Clear();

            foreach (DataModelBase objObject in Items)
            {
                if (objObject is DataCenter)
                    m_objDataCenters.Add((DataCenter)objObject);
            }
        }

        public DataCenter[] DataCenters
        {
            get { return m_objDataCenters.ToArray(); }
        }
    }
}
