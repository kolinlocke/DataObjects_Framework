using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.Objects
{
    [Serializable()]
    public class ClsBindGridColumn
    {
        #region _Variables

        public string mFieldName;
        public string mFieldDesc;
        public string mColumnName;
        public Int32 mWidth;
        public Constants.eSystem_Lookup_FieldType mFieldType;
        public string mDataFormat;
        public bool mIsVisible;
        public bool mEnabled;
        public bool mIsFilter;

        #endregion

        #region _Constructor

        public ClsBindGridColumn() { }

        public ClsBindGridColumn(string FieldName)
        { this.Setup(FieldName, FieldName); }

        public ClsBindGridColumn(string FieldName
            , string FieldDesc
            , Int32 Width = 100
            , string DataFormat = ""
            , Constants.eSystem_Lookup_FieldType FieldType = Constants.eSystem_Lookup_FieldType.FieldType_Static
            , bool IsVisible = true
            , bool Enabled = true
            , bool IsFilter = true)
        {
            this.Setup(
                FieldName
                , FieldDesc
                , Width
                , DataFormat
                , FieldType
                , IsVisible
                , Enabled
                , IsFilter);
        }

        #endregion

        #region _Methods

        protected virtual void Setup(
          string FieldName
          , string FieldDesc
          , Int32 Width = 100
          , string DataFormat = ""
          , Constants.eSystem_Lookup_FieldType FieldType = Constants.eSystem_Lookup_FieldType.FieldType_Static
          , bool IsVisible = true
          , bool Enabled = true
          , bool IsFilter = true)
        {
            this.mFieldName = FieldName;
            this.mFieldDesc = FieldDesc;
            this.mColumnName = FieldName;
            this.mWidth = Width;
            this.mDataFormat = DataFormat;
            this.mFieldType = FieldType;
            this.mIsVisible = IsVisible;
            this.mEnabled = Enabled;
            this.mIsFilter = IsFilter;
        }

        #endregion
    }
}
