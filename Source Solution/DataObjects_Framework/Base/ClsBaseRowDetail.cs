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
    /// <summary>
    /// Internal, manages the defined row detail
    /// </summary>
    internal class ClsBaseRowDetail
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

        private ClsBaseRowDetail() { }

        /// <summary>
        /// Constructor for this class
        /// </summary>
        /// <param name="pObj_Base">
        /// Parent Object Base
        /// </param>
        /// <param name="pHeaderName">
        /// Parent Table Name, (apparently not used at all)
        /// </param>
        /// <param name="pTableName">
        /// Row Detail Table Name
        /// </param>
        /// <param name="pViewName">
        /// Row Detail View Name
        /// </param>
        /// <param name="pOtherLoadCondition">
        /// Additional conditions for fetching
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        /// <param name="ForeignKeys">
        /// Custom Foreign Key definition
        /// </param>
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
        
        /// <summary>
        /// Loads the row detail data
        /// </summary>
        /// <param name="Da">
        /// An open DataAccess object from calling method
        /// </param>
        /// <param name="Keys">
        /// Key Object to use
        /// </param>
        public void Load(Interface_DataAccess Da, ClsKeys Keys)
        { this.mDr = Da.Load_RowDetails(this.mViewName, Keys, this.mOtherLoadCondition, this.mList_ForeignKey); }

        /// <summary>
        /// Saves the changes to the detail table
        /// </summary>
        /// <param name="Da">
        /// An open DataAccess from calling method
        /// </param>
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

        /// <summary>
        /// Gets the defined table name for this row detail
        /// </summary>
        public string pTableName
        {
            get { return this.mTableName; }
        }

        /// <summary>
        /// Gets the DataRow object for this row detail
        /// </summary>
        public DataRow pDr
        {
            get { return this.mDr; }
        }

        #endregion

    }
}
