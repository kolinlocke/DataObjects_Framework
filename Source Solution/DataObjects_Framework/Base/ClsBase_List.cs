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
using DataObjects_Framework.Base;

namespace DataObjects_Framework.Base
{
    /// <summary>
    /// Base Class for Data Objects, a List will be loaded instead
    /// The List can be edited and saved to the defined Table Name
    /// </summary>
    public abstract class ClsBase_List : ClsBase
    {
        #region _Variables

        /// <summary>
        /// The additional fetching conditions is stored here.
        /// </summary>
        protected ClsQueryCondition mQc_LoadCondition = null;

        /// <summary>
        /// The data table storage for the loaded Data Object, Me.Load() required
        /// </summary>
        protected DataTable mDt_List;
        
        #endregion

        #region _Methods

        /// <summary>
        /// Sets the data object definition, 
        /// must be set preferably in the constructor of the derived object
        /// </summary>
        /// <param name="TableName">
        /// Table Name of the data object will be using
        /// </param>
        /// <param name="ViewName">
        /// View Name of the data object 
        /// this will be used Me.Load() if supplied
        /// </param>
        /// <param name="Qc_LoadCondition">
        /// Additional conditions when fetching the data object
        /// </param>
        public virtual void Setup(string TableName, string ViewName = "", ClsQueryCondition Qc_LoadCondition = null)
        {
            base.Setup(TableName, ViewName);
            this.mQc_LoadCondition = Qc_LoadCondition;
        }

        /// <summary>
        /// Not Implemented, use Me.Load()
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="Sort"></param>
        /// <returns></returns>
        public override DataTable List(string Condition, string Sort = "")
        { throw new NotImplementedException(); }

        /// <summary>
        /// Not Implemented, use Me.Load()
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="Sort"></param>
        /// <param name="Top"></param>
        /// <param name="Page"></param>
        /// <returns></returns>
        public override DataTable List(ClsQueryCondition Condition = null, string Sort = "", int Top = 0, int Page = 0)
        { throw new NotImplementedException(); }

        /// <summary>
        /// (Overrided) Loads the List with the supplied Key
        /// </summary>
        /// <param name="Keys">
        /// Key object to use
        /// </param>
        public override void Load(ClsKeys Keys = null)
        {
            if (Keys == null)
            { this.New(); }
            else
            {
                ClsQueryCondition Qc = this.mDa.CreateQueryCondition();
                foreach (string KeyName in Keys.pName)
                { Qc.Add(KeyName, Keys[KeyName].ToString(), typeof(Int64).ToString(), "0"); }

                if (this.mQc_LoadCondition != null)
                {
                    foreach (DataObjects_Framework.Objects.ClsQueryCondition.Str_QueryCondition Str_Qc in this.mQc_LoadCondition.pList)
                    { Qc.pList.Add(Str_Qc); }
                }

                this.Load(Qc);
            }
        }

        /// <summary>
        /// (Overridable) Loads the List with the supplied condition string
        /// </summary>
        /// <param name="Condition">
        /// String condition to use
        /// </param>
        public virtual void Load(string Condition)
        {
            this.mDt_List = this.mDa.List(this.mHeader_ViewName, Condition);
            this.AddRequired(this.mDt_List);
        }

        /// <summary>
        /// (Overridable) Loads the List with the supplied QueryCondition object
        /// </summary>
        /// <param name="Condition">
        /// QueryCondition object to use
        /// </param>
        public virtual void Load(DataObjects_Framework.Objects.ClsQueryCondition Condition)
        {
            this.mDt_List = this.mDa.List(this.mHeader_ViewName, Condition);
            this.AddRequired(this.mDt_List);
        }

        void New()
        {
            this.mDt_List = this.mDa.List_Empty(this.mHeader_ViewName);
            this.AddRequired(this.mDt_List);
        }

        /// <summary>
        /// (Overrided) Saves changes to the List
        /// </summary>
        /// <param name="Da">
        /// Optional, an open Data_Access Objects that is reused from the calling method
        /// </param>
        /// <returns></returns>
        public override bool Save(DataAccess.Interface_DataAccess Da = null)
        {
            bool IsSave = false;
            bool IsDa = false;

            if (Da == null)
            {
                Da = this.mDa;
                Da.Connect();
                Da.BeginTransaction();
                IsDa = true;
            }

            try
            {
                DataRow[] ArrDr = this.mDt_List.Select("", "", DataViewRowState.CurrentRows);
                foreach (DataRow Dr in ArrDr)
                {
                    if (Dr.RowState == DataRowState.Added || Dr.RowState == DataRowState.Modified)
                    { Da.SaveDataRow(Dr, this.mHeader_TableName); }
                }

                ArrDr = this.mDt_List.Select("", "", DataViewRowState.Deleted);
                foreach (DataRow Dr in ArrDr)
                {
                    DataRow Nr = Dr.Table.NewRow();
                    foreach (DataColumn Dc in Dr.Table.Columns)
                    { Nr[Dc.ColumnName] = Dr[Dc.ColumnName, DataRowVersion.Original]; }

                    Da.SaveDataRow(Nr, this.mHeader_TableName, "", true);
                }

                if (IsDa)
                { Da.CommitTransaction(); }
                IsSave = true;
            }
            catch (Exception Ex)
            {
                if (IsDa)
                { Da.RollbackTransaction(); }
                throw Ex;
            }
            finally
            {
                if (IsDa)
                { Da.Close(); }
            }

            return IsSave;
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Get Property, gets the List datatable for the Data Object, Me.Load() required
        /// </summary>
        public DataTable pDt_List
        {
            get
            { return this.mDt_List; }
        }

        #endregion
    }
}
