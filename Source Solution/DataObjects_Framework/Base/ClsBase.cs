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
    /// Base Class for Data Objects
    /// to be inherited in order to use
    /// </summary>
    [Serializable()]
    public class ClsBase
    {
        #region _Variables

        /// <summary>
        /// The declared TableName of the Data Object
        /// </summary>
        protected string mHeader_TableName;

        /// <summary>
        /// The declared ViewName of the Data Object
        /// </summary>
        protected string mHeader_ViewName;

        /// <summary>
        /// DataRow storage of the Data Object (this.Load() Required)
        /// </summary>
        protected DataRow mHeader_Dr;

        /// <summary>
        /// Default/Custom Table Key Definition of the Data Object
        /// </summary>
        protected List<string> mHeader_Key = new List<string>();

        bool mIsCustomKeys = false;

        /// <summary>
        /// List of Table Detail objecs of the Data Object
        /// </summary>
        protected List<ClsBaseTableDetail> mBase_TableDetail = new List<ClsBaseTableDetail>();

        /// <summary>
        /// List of Row Detail objecs of the Data Object
        /// </summary>
        protected List<ClsBaseRowDetail> mBase_RowDetail = new List<ClsBaseRowDetail>();

        /// <summary>
        /// Current Data Access object
        /// </summary>
        protected Interface_DataAccess mDa = new ClsDataAccess_SqlServer();

        #endregion

        #region _Constructor

        /// <summary>
        /// Default Constructor
        /// Mostly used to access Instance Methods such the DataAccess Object
        /// </summary>
        public ClsBase() { }

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
		/// this will be used in Me.List() and Me.Load() if supplied
		/// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        protected virtual void Setup(string TableName, string ViewName = "", List<string> CustomKeys = null)
        {
            this.mHeader_TableName = TableName;

            if (ViewName == "")
            { ViewName = TableName; }

            this.mHeader_ViewName = ViewName;

            this.mIsCustomKeys = CustomKeys != null;
            if (CustomKeys == null)
            {
                DataTable Dt_Def = this.mDa.GetTableDef(this.mHeader_TableName);
                DataRow[] ArrDr = Dt_Def.Select("IsPk = 1");
                foreach (DataRow Dr in ArrDr)
                { this.mHeader_Key.Add((string)Dr["ColumnName"]); }
            }
            else
            { this.mHeader_Key = CustomKeys; }
        }

        /// <summary>
        /// Adds a Detail Table
        /// </summary>
        /// <param name="TableName">
        /// Table Name of the detail table
        /// </param>
        /// <param name="ViewName">
        /// View Name of the detail table, 
        /// will be used in Load()
        /// </param>
        /// <param name="LoadCondition">
        /// Additional conditions to be used in fetching the data
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        /// <param name="CustomForeignKeys">
        /// Custom Foreign Key definition
        /// </param>
        protected void Add_TableDetail(
            string TableName
            , string ViewName = ""
            , string LoadCondition = ""
            , List<string> CustomKeys = null
            , List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
        {
            this.mBase_TableDetail.Add(
                new ClsBaseTableDetail(
                    this
                    , this.mHeader_TableName
                    , TableName
                    , ViewName
                    , LoadCondition
                    , CustomKeys
                    , CustomForeignKeys));
        }

        /// <summary>
        /// Adds a Row Table, a table detail that is expected to have only one row 
        /// (one is to one relationship)
        /// </summary>
        /// <param name="TableName">
        /// Table Name of the row detail
        /// </param>
        /// <param name="ViewName">
        /// View Name of the row detail
        /// </param>
        /// <param name="LoadCondition">
        /// Additional conditions to be used in fetching the data
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        /// <param name="CustomForeignKeys">
        /// Custom Foreign Key definition
        /// </param>
        protected void Add_RowDetails(
            string TableName
            , string ViewName = ""
            , string LoadCondition = ""
            , List<string> CustomKeys = null
            , List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
        { 
            this.mBase_RowDetail.Add(
                new ClsBaseRowDetail(
                    this
                    , this.mHeader_TableName
                    , TableName
                    , ViewName
                    , LoadCondition
                    , CustomKeys
                    , CustomForeignKeys)); 
        }

        //[-]

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="Condition">
        /// Additional conditions to be used in fetching the data
        /// </param>
        /// <param name="Sort">
        /// Additional sorting to be used in fetching the data
        /// </param>
        /// <returns></returns>
        public virtual DataTable List(string Condition = "", string Sort = "")
        { return this.mDa.List(this.mHeader_ViewName, Condition, Sort); }

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="Condition">
        /// ClsQueryCondition Object to be used in fetching the data
        /// </param>
        /// <param name="Sort">
        /// Additional sorting to be used in fetching the data
        /// </param>
        /// <param name="Top">
        /// Limits the result set, mainly used for pagination
        /// </param>
        /// <param name="Page">
        /// Fetch a section of the result set based on the supplied Top, mainly used for pagination
        /// </param>
        /// <returns></returns>
        public virtual DataTable List(ClsQueryCondition Condition, string Sort = "", Int32 Top = 0, Int32 Page = 0)
        { return this.mDa.List(this.mHeader_ViewName, Condition, Sort, Top, Page); }

        /// <summary>
        /// Returns a Empy List based on the supplied Table/View Name
        /// Used for getting the definition of the table
        /// </summary>
        /// <returns></returns>
        public virtual DataTable List_Empty()
        { return this.mDa.List_Empty(this.mHeader_ViewName); }

        /// <summary>
        /// Returns the Result Set Count with out actually fetching the result set,
        /// mainly used for pagination
        /// </summary>
        /// <param name="Condition">
        /// ClsQueryCondition Object to be used in fetching the data
        /// </param>
        /// <returns></returns>
        public virtual Int64 List_Count(ClsQueryCondition Condition = null)
        { return this.mDa.List_Count(this.mHeader_ViewName, Condition); }

        //[-]

        /// <summary>
        /// (Overridable) Loads the Data Object with the supplied Key,
        /// when loading table details, the framework assumes the foreign key field of the table detail is the same the parent table
        /// if not supplied by an explicit foreign key definition
        /// </summary>
        /// <param name="Keys">
        /// Key object to use
        /// </param>
        public virtual void Load(ClsKeys Keys = null)
        {
            try
            {
                this.mDa.Connect();

                //[-]

                this.mHeader_Dr = this.mDa.Load(this.mHeader_ViewName, this.mHeader_Key, Keys);

                //[-]

                if (this.mBase_TableDetail != null)
                {
                    foreach (ClsBaseTableDetail Inner_Obj in this.mBase_TableDetail)
                    { Inner_Obj.Load(this.mDa, Keys); }
                }

                //[-]

                if (this.mBase_RowDetail != null)
                {
                    foreach (ClsBaseRowDetail Inner_Obj in this.mBase_RowDetail)
                    { Inner_Obj.Load(this.mDa, Keys); }
                }

                //[-]

                this.AddRequired();
            }

            catch (Exception Ex)
            { throw Ex; }
            finally
            { this.mDa.Close(); }
        }
        
        /// <summary>
        /// (Overridable) Saves changes to the Data Object
        /// </summary>
        /// <param name="Da">
        /// Optional, an open Data_Access Objects that is reused from the calling method
        /// </param>
        /// <returns></returns>
        public virtual bool Save(Interface_DataAccess Da = null)
        {
            bool IsSave = false;
            bool IsDa = false;
            
            try
            {
                if (Da == null)
                {
                    Da = this.mDa;
                    Da.Connect();
                    Da.BeginTransaction();
                    IsDa = true;
                }

                Da.SaveDataRow(
                    this.mHeader_Dr
                    , this.mHeader_TableName
                    , ""
                    , false
                    , this.mIsCustomKeys ? this.mHeader_Key : null);

                //[-]

                if (this.mBase_TableDetail != null)
                {
                    foreach (ClsBaseTableDetail Inner_Obj in this.mBase_TableDetail)
                    { Inner_Obj.Save(Da); }
                }

                //[-]

                if (this.mBase_RowDetail != null)
                {
                    foreach (ClsBaseRowDetail Inner_Obj in this.mBase_RowDetail)
                    { Inner_Obj.Save(Da); }
                }

                //[-]

                if (IsDa)
                { Da.CommitTransaction(); }
                IsSave = true;
            }            
            catch (Exception ex)
            {
                if (IsDa)
                { Da.RollbackTransaction(); }
                throw ex;
            }            
            finally
            {
                if (IsDa)
                { Da.Close(); }
            }

            return IsSave;
        }

        /// <summary>
        /// (Overridable) Deletes the Data Object
        /// </summary>
        public virtual void Delete()
        {
            try
            {
                this.mDa.Connect();
                this.mDa.BeginTransaction();
                this.mDa.SaveDataRow(this.mHeader_Dr, this.mHeader_TableName, "", true);
                this.mDa.CommitTransaction();
            }

            catch (Exception ex)
            {
                this.mDa.RollbackTransaction();
                throw ex;
            }

            finally
            { 
                this.mDa.Close(); 
            }
        }

        //[-]

        /// <summary>
        /// Gets the current Keys of the Data Object
        /// </summary>
        /// <returns></returns>
        public ClsKeys GetKeys()
        { 
            ClsKeys Obj = new ClsKeys();

            foreach (string Key in this.mHeader_Key)
            {
                Int64 ID = Do_Methods.Convert_Int64(this.mHeader_Dr[Key], 0);
                Obj.Add(Key, ID);
            }

            return Obj;
        }

        /// <summary>
        /// Gets the Keys of the supplied row using the Key Definition of the Data Object
        /// </summary>
        /// <param name="Dr">
        /// Source datarow, mostly the same definition as from Me.List()
        /// </param>
        /// <returns></returns>
        public ClsKeys GetKeys(DataRow Dr)
        {
            ClsKeys Obj = new ClsKeys();

            foreach (string Key in this.mHeader_Key)
            {
                Int64 ID = Do_Methods.Convert_Int64(Dr[Key]);
                Obj.Add(Key, ID);
            }

            return Obj;
        }

        /// <summary>
        /// Gets the Keys of the supplier datarow using the supplier Key Definition
        /// </summary>
        /// <param name="Dr">
        /// Source datarow
        /// </param>
        /// <param name="KeyNames">
        /// Key definition
        /// </param>
        /// <returns></returns>
        public ClsKeys GetKeys(DataRow Dr, List<string> KeyNames)
        {
            bool IsFound = false;
            ClsKeys Key = new ClsKeys();

            foreach (string Inner_Key in KeyNames)
            {
                if (!Information.IsDBNull(Dr[Inner_Key]))
                { Key.Add(Inner_Key, Do_Methods.Convert_Int64(Dr[Inner_Key], 0)); }
                else
                {
                    IsFound = true;
                    break;
                }
            }

            if (IsFound)
            { Key = null; }

            return Key;
        }

        /// <summary>
        /// (Overridable) Add required columns (e.g. validation flags) to supplier datatable,
        /// mostly used in detail tables
        /// </summary>
        /// <param name="Dt"></param>
        public virtual void AddRequired(DataTable Dt)
        {
            Int64 Ct = 0;

            try
            {
                Dt.Columns.Add("TmpKey", typeof(Int64));
                Dt.Columns.Add("IsError", typeof(bool));
                Dt.Columns.Add("Item_Style", typeof(string));
            }
            catch { }

            foreach (DataRow Dr in Dt.Rows)
            {
                Ct++;
                Dr["TmpKey"] = Ct;
                Dr["Item_Style"] = "";
            }

            Dt.AcceptChanges();
        }

        /// <summary>
        /// (Overridable) Calls AddRequired to all defined Table Details
        /// </summary>
        protected virtual void AddRequired()
        {
            if (this.mBase_TableDetail == null)
            { return; }

            foreach (ClsBaseTableDetail Obj in this.mBase_TableDetail)
            { this.AddRequired(Obj.pDt); }
        }

        /// <summary>
        /// (Overridable) Clears Validation Flags to supplied datatable
        /// </summary>
        /// <param name="Dt"></param>
        public virtual void Check_Clear(DataTable Dt)
        {
            DataRow[] Arr_Dr = Dt.Select("", "", DataViewRowState.CurrentRows);
            foreach (DataRow Dr in Arr_Dr)
            {
                Dr["IsError"] = false;
                Dr["Item_Style"] = "";
            }
        }

        /// <summary>
        /// (Overridable) Clears Validation flags to all defined Table Details
        /// </summary>
        public virtual void Check_Clear()
        {
            if (this.mBase_TableDetail == null)
            { return; }

            foreach (ClsBaseTableDetail Obj in this.mBase_TableDetail)
            { this.Check_Clear(Obj.pDt); }
        }

        /// <summary>
        /// (Static) Gets a new TmpKey Value from the supplied datatable with a TmpKey column
        /// </summary>
        /// <param name="Dt_Source"></param>
        /// <returns></returns>
        public static Int64 GetNewTmpKey(DataTable Dt_Source)
        {
            Int64 Rv = 0;
            DataRow[] ArrDr = Dt_Source.Select("", "TmpKey Desc");
            
            if (ArrDr.Length > 0)
            { 
                Rv = (Int64)ArrDr[0]["TmpKey"]; 
            }

            Rv++;
            return Rv;
        }

        /// <summary>
        /// Checks if the Data Object's Deleted flag is raised
        /// </summary>
        protected void CheckIfDeleted()
        {
            DataRow Dr = this.mHeader_Dr;

            bool IsDeleted = false;
            try
            { IsDeleted = (bool)Do_Methods.IsNull(Dr["IsDeleted"], false); }
            catch { }

            if (IsDeleted)
            { throw new ClsCustomException("This record is deleted."); }
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Get Property, gets the Data Object ID (or primary key)
        /// </summary>
        public Int64 pID
        {
            get 
			{
				if (this.mHeader_Key.Count > 1)
				{ throw new Exception("This can only be used when the table key is only one."); }

				return Do_Methods.Convert_Int64(this.mHeader_Dr[this.pHeader_TableKey]);
			}
        }

        /// <summary>
        /// Get Property, gets the current Keys of the Data Object 
        /// </summary>
        public ClsKeys pKey 
        {
            get
            {
                ClsKeys Obj = new ClsKeys();
                try
                {
                    foreach (string Key in this.mHeader_Key)
                    {
                        Int64 ID = Do_Methods.Convert_Int64(this.mHeader_Dr[Key]);
                        Obj.Add(Key, ID);
                    }
                }
                catch
                { Obj = null; }
                return Obj;                
            }
        }

        /// <summary>
        /// Get Property, gets the datarow for the Data Object (Me.Load() Required)
        /// </summary>
        public virtual DataRow pDr
        {
            get
            { return this.mHeader_Dr; }
        }

        /// <summary>
        /// Get Property, gets the Key Definition
        /// </summary>
        public List<string> pHeader_Key
        {
            get { return this.mHeader_Key; }
        }

        /// <summary>
        /// Get Property, gets the defined Table Name 
        /// </summary>
        public string pHeader_TableName
        {
            get { return this.mHeader_TableName; }
        }

        /// <summary>
        /// Get Property, gets the defined View Name 
        /// </summary>
        public string pHeader_ViewName
        {
            get { return this.mHeader_ViewName; }
        }

        /// <summary>
        /// Get Property, gets the defined Table Key Name
        /// </summary>
        public string pHeader_TableKey
        {
            get 
			{
				if (this.mHeader_Key.Count > 1)
				{ throw new Exception("This can only be used when the table key is only one."); }

				return this.mHeader_Key[0];
			}
        }

        /// <summary>
        /// Gets the table detail by Name
        /// </summary>
        /// <param name="Name">
        /// The name of the table detail to get
        /// </param>
        /// <returns></returns>
        public DataTable pTableDetail_Get(string Name)
        {
            /*
            ClsBaseTableDetail Obj = null;
            foreach (ClsBaseTableDetail Inner_Obj in this.mBase_TableDetail)
            {
                if (Inner_Obj.pTableName == Name)
                {
                    Obj = Inner_Obj;
                    break;
                }
            }

            DataTable Dt = null;
            if (Obj != null) Dt = Obj.pDt;

            return Dt;
            */

            DataTable Dt = null;
            try { Dt =  this.mBase_TableDetail.Find(item => item.pTableName == Name).pDt; }
            catch { }

            return Dt;
        }

        /// <summary>
        /// Sets a new value for a table detail searched by name
        /// </summary>
        /// <param name="Name">
        /// The name of the table detail to search
        /// </param>
        /// <param name="Value">
        /// New datatable object to set
        /// </param>
        public void pTableDetail_Set(string Name, DataTable Value)
        {
            /*
            ClsBaseTableDetail Obj = null;
            foreach (ClsBaseTableDetail Inner_Obj in this.mBase_TableDetail)
            {
                if (Inner_Obj.pTableName == Name)
                {
                    Obj = Inner_Obj;
                    break;
                }
            }
            Obj.pDt = Value;
            */

            try { this.mBase_TableDetail.Find(item => item.pTableName == Name).pDt = Value; }
            catch { }
        }

        /// <summary>
        /// Gets the row detail by Name
        /// </summary>
        /// <param name="Name">
        /// Name of the row detail to get
        /// </param>
        /// <returns></returns>
        public DataRow pRowDetail_Get(string Name)
        {
            ClsBaseRowDetail Obj = null;
            foreach (ClsBaseRowDetail Inner_Obj in this.mBase_RowDetail)
            {
                if (Inner_Obj.pTableName == Name)
                {
                    Obj = Inner_Obj;
                    break;
                }
            }

            DataRow Dr = null;
            if (Obj != null) Dr = Obj.pDr;
            return Dr;
        }

        /// <summary>
        /// Current DataAccess Object
        /// </summary>
        public Interface_DataAccess pDa
        {
            get { return this.mDa; } 
        }

        #endregion
    }
}
