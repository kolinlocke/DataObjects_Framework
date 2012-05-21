using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using DataObjects_Framework;
using DataObjects_Framework._System;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Modules_Base;
using DataObjects_Framework.Modules_Base.Abstract;
using DataObjects_Framework.Modules_Base.Objects;

namespace DataObjects_Framework.Modules_Base.Objects
{
    public class ClsBaseRowDetail
    {
        #region _Variables

        string mHeaderName;
        string mTableName;
        string mViewName;
        List<string> mList_Key = new List<string>();
        ClsBase mObj_Base;

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
            , string pOtherLoadCondition = "")
        {
            if (pViewName == "") pViewName = pTableName;

            this.mHeaderName = pHeaderName;
            this.mTableName = pTableName;
            this.mViewName = pViewName;
            this.mOtherLoadCondition = pOtherLoadCondition;
            this.mObj_Base = pObj_Base;
			this.mDa = pObj_Base.pDa;

            DataTable Dt_Def = this.mDa.GetTableDef(this.mTableName);
            DataRow[] ArrDr = Dt_Def.Select("IsPk = 1");
            foreach (DataRow Dr in ArrDr)
            { this.mList_Key.Add((string)Dr["ColumnName"]); }            
        }
        
        #endregion

        #region _Methods

        /*
        public void Load(Interface_DataAccess Da, string Condition = "")
        {
            string OtherCondition = "";
            if (this.mOtherLoadCondition != "") OtherCondition = " And " + this.mOtherLoadCondition;

            DataTable Dt;
            DataRow Dr;
            if (Condition == "")
            {
                Dt = this.mDa.GetQuery(Da.Connection, this.mViewName, "*", "1 = 0");
                Dr = Dt.NewRow();
            }
            else 
            { 
                Dt = this.mDa.GetQuery(Da.Connection, this.mViewName, "*", Condition + OtherCondition);
                if (Dt.Rows.Count > 0) Dr = Dt.Rows[0];
                else Dr = Dt.NewRow();                
            }

            this.mDr = Dr;
        }
        */

        /*
        public void Load(string Condition = "")
        {
            try
            {
                this.mDa.Connect();
                this.Load(this.mDa, Condition);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { this.mDa.Close(); }
        }
        */

        public void Load(Interface_DataAccess Da, ClsKeys Keys)
        { this.mDr = Da.Load_RowDetails(this.mViewName, Keys, this.mOtherLoadCondition); }

        public void Save(Interface_DataAccess Da)
        {
            foreach (string Header_Key in this.mObj_Base.pHeader_Key)
            {
                Int64 Inner_ID = (Int64)Layer01_Methods.IsNull(this.mObj_Base.pDr[Header_Key], 0);
                this.mDr[Header_Key] = Inner_ID;
            }

            Da.SaveDataRow(this.mDr, this.mTableName);
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
