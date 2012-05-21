using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Base;

namespace DataObjects_Framework.Base
{
    public class ClsBaseRowDetail
    {
        #region _Variables

        string mHeaderName;
        string mTableName;
        string mViewName;
        List<string> mList_Key = new List<string>();
        List<Do_Constants.Str_ForeignKeyRelation> mList_ForeignKey = new List<Common.Do_Constants.Str_ForeignKeyRelation>();
        ClsBase mObj_Base;
        bool mIsCustomKeys = false;

        string mOtherLoadCondition;
        DataRow mDr;

        Interface_DataAccess mDa;

        #endregion

        #region _Constructor

        private ClsBaseRowDetail()
        { }

        public ClsBaseRowDetail(
            ClsBase pObj_Base
            , string pHeaderName
            , string pTableName
            , string pViewName = ""
            , string pOtherLoadCondition = ""
            , List<string> CustomKeys = null
            , List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys = null)
        {
            if (pViewName == "") pViewName = pTableName;

            this.mHeaderName = pHeaderName;
            this.mTableName = pTableName;
            this.mViewName = pViewName;
            this.mOtherLoadCondition = pOtherLoadCondition;
            this.mList_ForeignKey = ForeignKeys;
            this.mObj_Base = pObj_Base;
            this.mDa = pObj_Base.pDa;
            
            this.mIsCustomKeys = CustomKeys != null;
            if (CustomKeys == null)
            {
                DataTable Dt_Def = this.mDa.GetTableDef(this.mTableName);
                DataRow[] ArrDr = Dt_Def.Select("IsPk = 1");
                foreach (DataRow Dr in ArrDr)
                { this.mList_Key.Add((string)Dr["ColumnName"]); }
            }
            else
            { this.mList_Key = CustomKeys; }
        }
        
        #endregion

        #region _Methods
        
        public void Load(Interface_DataAccess Da, ClsKeys Keys)
        { this.mDr = Da.Load_RowDetails(this.mViewName, Keys, this.mOtherLoadCondition, this.mList_ForeignKey); }

        public void Save(Interface_DataAccess Da)
        {
            /*
            foreach (string Header_Key in this.mObj_Base.pHeader_Key)
            {
                Int64 Inner_ID = Do_Methods.Convert_Int64(this.mObj_Base.pDr[Header_Key]);
                this.mDr[Header_Key] = Inner_ID;
            }
            */

            if (!this.mIsCustomKeys)
            {
                foreach (string Header_Key in this.mObj_Base.pHeader_Key)
                {
                    Int64 Inner_ID = Do_Methods.Convert_Int64(this.mObj_Base.pDr[Header_Key]);
                    this.mDr[Header_Key] = Inner_ID;
                }
            }
            else
            {
                foreach (Do_Constants.Str_ForeignKeyRelation Inner_Keys in this.mList_ForeignKey)
                {
                    Int64 Inner_ID = Do_Methods.Convert_Int64(this.mObj_Base.pDr[Inner_Keys.Parent_Key]);
                    this.mDr[Inner_Keys.Child_Key] = Inner_ID;
                }
            }

            Da.SaveDataRow(this.mDr, this.mTableName, "", false, this.mIsCustomKeys ? this.mList_Key : null);
        }

        #endregion

        #region _Properties

        public string pTableName
        {
            get
            { return this.mTableName; }
        }

        public DataRow pDr
        {
            get
            { return this.mDr; }
        }

        #endregion

    }
}
