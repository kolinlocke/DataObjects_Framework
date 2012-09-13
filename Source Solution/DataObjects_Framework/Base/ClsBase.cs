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
        /// The Parent Data Object for this instance.
        /// </summary>
        protected ClsBase mObj_Parent;

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
        internal List<ClsBaseTableDetail> mBase_TableDetail = new List<ClsBaseTableDetail>();

        /// <summary>
        /// List of Row Detail objecs of the Data Object
        /// </summary>
        internal List<ClsBaseRowDetail> mBase_RowDetail = new List<ClsBaseRowDetail>();

        internal List<ClsBaseListDetail> mBase_ListDetail = new List<ClsBaseListDetail>();

        /// <summary>
        /// Current Data Access object
        /// </summary>
        protected Interface_DataAccess mDa;

        #endregion

        #region _Constructor

        /// <summary>
        /// Default Constructor
        /// Mostly used to access Instance Methods such the DataAccess Object
        /// </summary>
        public ClsBase() 
        {
            //this.mDa = new ClsDataAccess_SqlServer();

            switch (Do_Globals.gSettings.pDataAccessType)
            {
                case Do_Constants.eDataAccessType.DataAccess_SqlServer:
                    {
                        this.mDa = new ClsDataAccess_SqlServer();
                        break;
                    }
                case Do_Constants.eDataAccessType.DataAccess_WCF:
                    { 
                        this.mDa = new ClsDataAccess_Wcf();
                        break;
                    }
            }

        }

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
        protected virtual void Setup(
            string TableName
            , string ViewName = ""
            , List<string> CustomKeys = null)
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
        protected virtual void Setup_AddTableDetail(
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
        protected virtual void Setup_AddRowDetails(
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

        /// <summary>
        /// Adds a List Detail.
        /// </summary>
        /// <param name="Name">
        /// The name of the List Detail.
        /// </param>
        /// <param name="Obj_List">
        /// The ClsBase_List object to be used.
        /// </param>
        /// <param name="CustomForeignKeys">
        /// Custom Foreign Key definition.
        /// </param>
        protected virtual void Setup_AddListDetail(
            string Name
            , ClsBase_List Obj_List
            , List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
        { this.mBase_ListDetail.Add(new ClsBaseListDetail(this, Name, Obj_List, CustomForeignKeys)); }

        //[-]

        public DataTable List()
        { return this.List("", ""); }

        public DataTable List(String Condition)
        { return this.List(Condition, ""); }

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
        public virtual DataTable List(String Condition, String Sort)
        {
            try
            {
                this.mDa.Connect();
                return this.List(this.mDa, Condition, Sort);
            }
            catch (Exception Ex) { throw Ex; }
            finally { this.mDa.Close(); }
        }

        public DataTable List(Interface_DataAccess Da)
        { return this.List(Da, "", ""); }

        public DataTable List(Interface_DataAccess Da, String Condition)
        { return this.List(Da, Condition, ""); }

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="Da"></param>
        /// <param name="Condition"></param>
        /// <param name="Sort"></param>
        /// <returns></returns>
        public virtual DataTable List(Interface_DataAccess Da, String Condition , String Sort)
        {
            DataTable Dt = null;
            Dt = Da.List(this.mHeader_ViewName, Condition, Sort); 
            return Dt;
        }

        public DataTable List(ClsQueryCondition Condition)
        { return this.List(Condition, "", 0, 0); }

        public DataTable List(ClsQueryCondition Condition, String Sort)
        { return this.List(Condition, Sort, 0, 0); }

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
        public virtual DataTable List(ClsQueryCondition Condition, String Sort, Int32 Top, Int32 Page)
        {
            try
            {
                this.mDa.Connect();
                return this.List(this.mDa, Condition, Sort, Top, Page);
            }
            catch (Exception Ex) { throw Ex; }
            finally { this.mDa.Close(); }
        }

        public DataTable List(Interface_DataAccess Da, ClsQueryCondition Condition)
        { return this.List(Da, Condition, "", 0, 0); }

        public DataTable List(Interface_DataAccess Da, ClsQueryCondition Condition, String Sort)
        { return this.List(Da, Condition, Sort, 0, 0); }

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="Da"></param>
        /// <param name="Condition"></param>
        /// <param name="Sort"></param>
        /// <param name="Top"></param>
        /// <param name="Page"></param>
        /// <returns></returns>
        public virtual DataTable List(Interface_DataAccess Da, ClsQueryCondition Condition, String Sort, Int32 Top, Int32 Page)
        {
            DataTable Dt = null;
            Dt = Da.List(this.mHeader_ViewName, Condition, Sort, Top, Page);
            return Dt;
        }

        /// <summary>
        /// Returns a Empy List based on the supplied Table/View Name
        /// Used for getting the definition of the table
        /// </summary>
        /// <returns></returns>
        public virtual DataTable List_Empty()
        {
            DataTable Dt = null;
            this.mDa.Connect();
            try { Dt = this.mDa.List_Empty(this.mHeader_ViewName); }
            catch (Exception Ex) { throw Ex; }
            finally { this.mDa.Close(); }
            return Dt;            
        }

        /// <summary>
        /// Returns the Result Set Count with out actually fetching the result set,
        /// mainly used for pagination
        /// </summary>
        /// <param name="Condition">
        /// ClsQueryCondition Object to be used in fetching the data
        /// </param>
        /// <returns></returns>
        public virtual Int64 List_Count(ClsQueryCondition Condition = null)
        {
            Int64 Rv = 0;
            this.mDa.Connect();
            try { Rv = this.mDa.List_Count(this.mHeader_ViewName, Condition); }
            catch (Exception Ex) { throw Ex; }
            finally { this.mDa.Close(); }
            return Rv;
        }

        //[-]

        /// <summary>
        /// Loads an empty Data Object.
        /// </summary>
        public virtual void Load()
        { this.Load((ClsKeys)null); }

        /// <summary>
        /// Loads the Data Object with the supplied Key,
        /// when loading table details, the framework assumes the foreign key field of the table detail is the same the parent table
        /// if not supplied by an explicit foreign key definition
        /// </summary>
        /// <param name="Keys">
        /// Key object to use, if null, it implies to create a new data object.
        /// </param>
        public virtual void Load(ClsKeys Keys)
        { this.Load(Keys, null); }

        /// <summary>
        /// Loads the Data Object with the supplied Key,
        /// when loading table details, the framework assumes the foreign key field of the table detail is the same the parent table
        /// if not supplied by an explicit foreign key definition
        /// </summary>
        /// <param name="Keys">
        /// Key object to use, if null, it implies to create a new data object.
        /// </param>
        /// <param name="Obj_Parent">
        /// The Parent Data Object.
        /// </param>
        public virtual void Load(ClsKeys Keys, ClsBase Obj_Parent)
        {
            try
            {
                this.mDa.Connect();
                this.Load(this.mDa, Keys, Obj_Parent);
            }
            catch (Exception Ex) { throw Ex; }
            finally { this.mDa.Close(); }
        }

        /// <summary>
        /// Loads the Data Object with the supplied Key,
        /// when loading table details, the framework assumes the foreign key field of the table detail is the same the parent table
        /// if not supplied by an explicit foreign key definition
        /// </summary>
        /// <param name="Da">
        /// An open DataAccess Object to be used.
        /// </param>
        /// <param name="Keys">
        /// Key object to use, if null, it implies to create a new data object.
        /// </param>
        public virtual void Load(Interface_DataAccess Da, ClsKeys Keys)
        { this.Load(Da, Keys, (ClsBase)null); }

        /// <summary>
        /// Loads the Data Object with the supplied Key,
        /// when loading table details, the framework assumes the foreign key field of the table detail is the same the parent table
        /// if not supplied by an explicit foreign key definition
        /// </summary>
        /// <param name="Da">
        /// An open DataAccess Object to be used.
        /// </param>
        /// <param name="Keys">
        /// Key object to use, if null, it implies to create a new data object.
        /// </param>
        /// <param name="Obj_Parent">
        /// The Parent Data Object.
        /// </param>
        public virtual void Load(Interface_DataAccess Da, ClsKeys Keys, ClsBase Obj_Parent)
        {
            this.mObj_Parent = Obj_Parent;
            this.mHeader_Dr = Da.Load(this.mHeader_ViewName, this.mHeader_Key, Keys);
            this.Load_Details(Da, Keys);
            this.AddRequired();
        }

        /// <summary>
        /// Loads the Data Object from an existing data row.
        /// </summary>
        /// <param name="Dr">
        /// Source data row to use
        /// </param>
        public virtual void Load(DataRow Dr)
        { 
            this.mHeader_Dr = Dr;
            this.Load_Details(this.GetKeys()); 
        }

        /// <summary>
        /// Loads the declared details of the data object.
        /// </summary>
        /// <param name="Keys">
        /// Key object to use.
        /// </param>
        protected void Load_Details(ClsKeys Keys = null)
        {
            this.Load_Details(this.mDa, Keys);
        }

        /// <summary>
        /// Loads the declared details of the data object.
        /// </summary>
        /// <param name="Da">
        /// An open DataAccess Object to be used.
        /// </param>
        /// <param name="Keys">
        /// Key object to use.
        /// </param>
        protected void Load_Details(Interface_DataAccess Da, ClsKeys Keys = null)
        {
            if (this.mBase_TableDetail != null)
            {
                foreach (ClsBaseTableDetail Inner_Obj in this.mBase_TableDetail)
                { Inner_Obj.Load(Da, Keys); }
            }

            //[-]

            if (this.mBase_RowDetail != null)
            {
				foreach (ClsBaseRowDetail Inner_Obj in this.mBase_RowDetail)
				{ Inner_Obj.Load(Da, Keys); }
            }

            //[-]

            if (this.mBase_ListDetail != null)
            {
				foreach (ClsBaseListDetail Inner_Obj in this.mBase_ListDetail)
				{ Inner_Obj.Load(Da, Keys); }
            }
        }

        /// <summary>
        /// Saves changes to the Data Object
        /// </summary>
        /// <param name="Da">
        /// An open Data_Access Objects that is reused from the calling method
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

                if (this.mBase_ListDetail != null)
                {
                    foreach (ClsBaseListDetail Inner_Obj in this.mBase_ListDetail)
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
        /// Deletes the Data Object
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
                Int64 ID = Do_Methods.Convert_Int64(this.mHeader_Dr[Key]);

                if (ID == 0)
                {
                    Obj = null;
                    break;
                }

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

                if (ID == 0)
                {
                    Obj = null;
                    break; 
                }

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
            bool IsFound = true;
            ClsKeys Key = new ClsKeys();

            foreach (string Inner_Key in KeyNames)
            {
                if (!Information.IsDBNull(Dr[Inner_Key]))
                {
                    Int64 Inner_KeyID = Do_Methods.Convert_Int64(Dr[Inner_Key]);
                    if (Inner_KeyID != 0)
                    { Key.Add(Inner_Key, Inner_KeyID); }
                    else
                    { 
                        IsFound = false;
                        break;
                    }
                }
                else
                {
                    IsFound = false;
                    break;
                }
            }

            if (!IsFound)
            { Key = null; }

            return Key;
        }

        /// <summary>
        /// Adds required columns (e.g. validation flags) to supplier datatable,
        /// mostly used in detail tables
        /// </summary>
        /// <param name="Dt">
        /// The target data table object
        /// </param>
        public virtual void AddRequired(DataTable Dt)
        {
            Int64 Ct = 0;

            try
            {
                Dt.Columns.Add("TmpKey", typeof(Int64));
                Dt.Columns.Add("IsError", typeof(bool));                
            }
            catch { }

            foreach (DataRow Dr in Dt.Rows)
            {
                Ct++;
                Dr["TmpKey"] = Ct;
            }

            Dt.AcceptChanges();
        }

        /// <summary>
        /// Calls AddRequired() to all defined Table Details
        /// </summary>
        protected virtual void AddRequired()
        {
            if (this.mBase_TableDetail == null)
            { return; }

            foreach (ClsBaseTableDetail Obj in this.mBase_TableDetail)
            { this.AddRequired(Obj.pDt); }
        }

        /// <summary>
        /// Clears Validation Flags to the specified data table
        /// </summary>
        /// <param name="Dt">
        /// The target data table object
        /// </param>
        public virtual void Check_Clear(DataTable Dt)
        {
            DataRow[] Arr_Dr = Dt.Select("", "", DataViewRowState.CurrentRows);
            foreach (DataRow Dr in Arr_Dr)
            {
                Dr["IsError"] = false;
            }
        }

        /// <summary>
        /// Clears Validation flags to all defined Table Details
        /// </summary>
        public virtual void Check_Clear()
        {
            if (this.mBase_TableDetail == null)
            { return; }

            foreach (ClsBaseTableDetail Obj in this.mBase_TableDetail)
            { this.Check_Clear(Obj.pDt); }
        }

        /// <summary>
        /// Gets a new TmpKey Value from the specified  data table with a TmpKey column
        /// </summary>
        /// <param name="Dt_Source">
        /// The source data table
        /// </param>
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
            try { IsDeleted = (bool)Do_Methods.IsNull(Dr["IsDeleted"], false); }
            catch { }

            if (IsDeleted) { throw new ClsCustomException("This record is deleted."); }
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Gets the parent data object of this instance.
        /// </summary>
        public ClsBase pObj_Parent
        {
            get { return this.mObj_Parent; }
        }

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

        public ClsBase_List pListDetail_Get(string Name)
        {
            ClsBaseListDetail Ld = this.mBase_ListDetail.FirstOrDefault(Item => Item.pName == Name);
            ClsBase_List Obj_List = null;
            if (Ld != null)
            { Obj_List = Ld.pObj_List; }

            return Obj_List;            
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
