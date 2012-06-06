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

		protected List<ClsBaseListObject> mBase_ListObject = new List<ClsBaseListObject>();

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
        /// /// <param name="Qc_LoadCondition">
        /// Additional conditions when fetching the data object
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        public virtual void Setup(
            string TableName
            , string ViewName = ""
            , ClsQueryCondition Qc_LoadCondition = null
            , List<string> CustomKeys = null
            )
        {
            base.Setup(TableName, ViewName, CustomKeys);
            this.mQc_LoadCondition = Qc_LoadCondition;
        }

		/// <summary>
		/// Not Implemented. Don't use.
		/// </summary>
		/// <param name="TableName"></param>
		/// <param name="ViewName"></param>
		/// <param name="LoadCondition"></param>
		/// <param name="CustomKeys"></param>
		/// <param name="CustomForeignKeys"></param>
		protected override void Setup_AddTableDetail(string TableName, string ViewName = "", string LoadCondition = "", List<string> CustomKeys = null, List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
		{ throw new NotImplementedException(); }

		/// <summary>
		/// Not Implemented. Don't use.
		/// </summary>
		/// <param name="TableName"></param>
		/// <param name="ViewName"></param>
		/// <param name="LoadCondition"></param>
		/// <param name="CustomKeys"></param>
		/// <param name="CustomForeignKeys"></param>
		protected override void Setup_AddRowDetails(string TableName, string ViewName = "", string LoadCondition = "", List<string> CustomKeys = null, List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
		{ throw new NotImplementedException(); }

		protected virtual void Setup_AddListObject(
			string Name
			, ClsBase Template_Obj
			, List<Object> Template_Obj_Constructors
			, string Template_ViewName
            , List<Do_Constants.Str_ForeignKeyRelation> Template_FetchKeys
			, List<Do_Constants.Str_ForeignKeyRelation> Template_ForeignKeys
			, ClsQueryCondition Template_LoadCondition = null)
		{
            this.mBase_ListObject.Add(
                new ClsBaseListObject(
                    this
                    , Name
                    , Template_Obj
                    , Template_Obj_Constructors
                    , Template_ViewName
                    , Template_FetchKeys
                    , Template_ForeignKeys
                    , Template_LoadCondition));
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
        /// Loads the List with the supplied Key
        /// </summary>
        /// <param name="Keys">
        /// Key object to use, if null, it implies to create a new data object.
        /// </param>
        /// <param name="Obj_Parent">
        /// The Parent Data Object.
        /// </param>
        public override void Load(ClsKeys Keys, ClsBase Obj_Parent = null)
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
                    foreach (ClsQueryCondition.Str_QueryCondition Str_Qc in this.mQc_LoadCondition.pList)
                    { Qc.pList.Add(Str_Qc); }
                }

                this.Load(Qc);
            }

            if (this.mBase_ListObject != null)
            {
                foreach (ClsBaseListObject Lo in this.mBase_ListObject)
                { Lo.Load(this.mDa, Keys); }
            }

            this.mObj_Parent = Obj_Parent;
        }

        /// <summary>
        /// Loads the Data Object from the key of the specified data row.
        /// </summary>
        /// <param name="Dr">
        /// Source data row to use
        /// </param>
        public override void Load(DataRow Dr)
        {
            ClsKeys Key = this.GetKeys(Dr);
            this.Load(Key);
        }

        /// <summary>
        /// Loads the List with the supplied condition string
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
        /// Loads the List with the supplied QueryCondition object
        /// </summary>
        /// <param name="Condition">
        /// QueryCondition object to use
        /// </param>
        public virtual void Load(ClsQueryCondition Condition)
        {
            this.mDt_List = this.mDa.List(this.mHeader_ViewName, Condition);
            this.AddRequired(this.mDt_List);
        }

        /// <summary>
        /// Saves changes to the List
        /// </summary>
        /// <param name="Da">
        /// An open Data_Access Objects that is reused from the calling method
        /// </param>
        /// <returns></returns>
        public sealed override bool Save(DataAccess.Interface_DataAccess Da = null)
        {
            this.Save_ListObjects(Da);
            this.Save_Add();
            return this.Save_Ex(Da);
        }

        /// <summary>
        /// Overide this method to add additional methods before saving.
        /// </summary>
        protected virtual void Save_Add() { }

        void Save_ListObjects(DataAccess.Interface_DataAccess Da = null)
        {
            foreach (ClsBaseListObject Lo in this.mBase_ListObject)
            { Lo.Save(Da); }
        }

        bool Save_Ex(DataAccess.Interface_DataAccess Da = null)
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

                //[-]

                ArrDr = this.mDt_List.Select("", "", DataViewRowState.Deleted);
                foreach (DataRow Dr in ArrDr)
                {
                    DataRow Nr = Dr.Table.NewRow();
                    foreach (DataColumn Dc in Dr.Table.Columns)
                    { Nr[Dc.ColumnName] = Dr[Dc.ColumnName, DataRowVersion.Original]; }

                    Da.SaveDataRow(Nr, this.mHeader_TableName, "", true);
                }

                //[-]

                if (IsDa) { Da.CommitTransaction(); }
                IsSave = true;
            }
            catch (Exception Ex)
            {
                if (IsDa) { Da.RollbackTransaction(); }
                throw Ex;
            }
            finally
            {
                if (IsDa) { Da.Close(); }
            }

            return IsSave;
        }

        void New()
        {
            this.mDt_List = this.mDa.List_Empty(this.mHeader_ViewName);
            this.AddRequired(this.mDt_List);
        }

        /// <summary>
        /// Adds a new data row to the collection.
        /// </summary>
		public virtual DataRow Add_Item()
		{
			DataRow Dr = this.mDt_List.NewRow();
			Dr["TmpKey"] = ClsBase.GetNewTmpKey(this.mDt_List);
			this.mDt_List.Rows.Add(Dr);

			foreach (ClsBaseListObject Obj in this.mBase_ListObject)
			{ Obj.Add_Object(Do_Methods.Convert_Int64(Dr["TmpKey"])); }

			return Dr;
		}

        /// <summary>
        /// Removes the data row and its corresponding List_Obj objects
        /// </summary>
        /// <param name="TmpKey">
        /// The key of the data row to be removed
        /// </param>
        public virtual void Delete_Item(Int64 TmpKey)
        {
            DataRow[] ArrDr = this.mDt_List.Select("TmpKey = " + TmpKey.ToString());
            if (ArrDr.Length > 0)
            {
                ArrDr[0].Delete();
                foreach (ClsBaseListObject Lo in this.mBase_ListObject)
                {
                    ClsBaseListObject.Str_Obj Lo_Obj = Lo.pList_Obj.FirstOrDefault(Item => Item.Name == TmpKey.ToString());
                    if (Lo_Obj.Obj != null) { Lo.pList_Obj.Remove(Lo_Obj); }
                }
            }
        }

        public void Refresh_Desc(string Name, List<ClsBaseListObject.Str_Desc> List_Desc)
        {
            ClsBaseListObject Obj = this.mBase_ListObject.FirstOrDefault(Item => Item.pName == Name);
            if (Obj != null)
            { Obj.Refresh_Desc(List_Desc); }
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Get Property, gets the List datatable for the Data Object, Me.Load() required
        /// </summary>
        public DataTable pDt_List
        {
            get { return this.mDt_List; }
        }

		public DataTable pDt_ListObject_Get(string Name)
		{ return this.mBase_ListObject.FirstOrDefault(Item => Item.pName == Name).pDt_Obj; }

		public ClsBase pObj_ListObject_Get(string Name, Int64 TmpKey)
		{
			ClsBaseListObject Obj = this.mBase_ListObject.FirstOrDefault(Item => Item.pName == Name);
			if (Obj != null) { return Obj.pList_Obj.FirstOrDefault(Item => Item.Name == TmpKey.ToString()).Obj; }
			else { return null; }
		}

        #endregion
    }
}
