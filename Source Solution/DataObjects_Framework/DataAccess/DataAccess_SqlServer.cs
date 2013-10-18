using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.PreparedQueryObjects;

namespace DataObjects_Framework.DataAccess
{
    /// <summary>
    /// The SQL Server implementation of Interface_DataAccess
    /// </summary>
    public class DataAccess_SqlServer : Interface_DataAccess
    {
        #region _Variables

        Connection_SqlServer mConnection;

        #endregion

        #region _ImplementedMethods

        public String BuildQuery_SourceObject(String SourceObject, String Fields, String Condition, String Sort)
        {
            if (SourceObject.Trim() != "") SourceObject = " From " + SourceObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Condition.Trim() != "") Condition = " Where " + Condition;
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            String Query = @"(Select " + Fields + " " + SourceObject + " " + Condition + " " + Sort + @") As [Source]";

            return Query;
        }

        /// <summary>
        /// Fetches a result set from a data source object
        /// </summary>
        /// <param name="Connection">
        /// An open connection object
        /// </param>
        /// <param name="SourceObject">
        /// Data source object to fetch from
        /// </param>
        /// <param name="Fields">
        /// List of fields tp fetch (SQL valid syntax)
        /// </param>
        /// <param name="Condition">
        /// Condition to use the filter the result set (SQL Where valid syntax)
        /// </param>
        /// <param name="Sort">
        /// Sort expression to use to sort the result set (SQL Order By valid syntax)
        /// </param>
        /// <param name="Top">
        /// Limits the number of returned rows in the result set,
        /// used in pagination
        /// </param>
        /// <param name="Page">
        /// Selects a section based on the Page and Top values in the result set,
        /// used in pagination
        /// </param>
        /// <returns></returns>
        public DataTable GetQuery(Interface_Connection Connection, String SourceObject, String Fields = "", String Condition = "", String Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            SourceObject = Do_Methods.Convert_String(SourceObject);
            Fields = Do_Methods.Convert_String(Fields);
            Condition = Do_Methods.Convert_String(Condition);
            Sort = Do_Methods.Convert_String(Sort);

            string Query_RowNumberSort = Sort;
            if (Query_RowNumberSort.Trim() == "") Query_RowNumberSort = "(Select 0)";

            string Query_Top = "";
            if (Top > 0) { Query_Top = "Top " + Top.ToString(); }

            Int64 PageCondition = 0;
            if (Page > 0) { PageCondition = Top * (Page - 1); }

            if (SourceObject.Trim() != "") SourceObject = " From " + SourceObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Condition.Trim() != "") Condition = " Where " + Condition;
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            PreparedQuery Pq = this.CreatePreparedQuery();
            Pq.pQuery = @"Declare @Query As VarChar(Max); Set @Query = 'Select ' + @Top + ' [Tb].* From ( Select Row_Number() Over (Order By ' + @RowNumberSort + ') As [RowNumber], ' + @Fields + ' ' + @ViewObject + ' ' + @Condition + ' ) As [Tb] Where [Tb].RowNumber >= ' + @PageCondition + ' ' + @Sort; Exec(@Query)";
            Pq.Add_Parameter(new QueryParameter() { Name = "ViewObject", Value = SourceObject, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Top", Value = Query_Top, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "RowNumberSort", Value = Query_RowNumberSort, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "PageCondition", Value = PageCondition.ToString(), Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Fields", Value = Fields, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Condition", Value = Condition, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Sort", Value = Sort, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });

            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }

        /// <summary>
        /// Fetches a result set from a data source object
        /// </summary>
        /// <param name="SourceObject">
        /// Data source object to fetch from
        /// </param>
        /// <param name="Fields">
        /// List of fields tp fetch (SQL valid syntax)
        /// </param>
        /// <param name="Condition">
        /// Condition to use the filter the result set (SQL Where valid syntax)
        /// </param>
        /// <param name="Sort">
        /// Sort expression to use to sort the result set (SQL Order By valid syntax)
        /// </param>
        /// <param name="Top">
        /// Limits the number of returned rows in the result set,
        /// used in pagination
        /// </param>
        /// <param name="Page">
        /// Selects a section based on the Page and Top values in the result set,
        /// used in pagination
        /// </param>
        /// <returns></returns>
        public DataTable GetQuery(String SourceObject, String Fields = "", String Condition = "", String Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort);
                }
                catch (Exception ex)
                { throw ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.GetQuery(this.mConnection, SourceObject, Fields, Condition, Sort); }
        }

        /// <summary>
        /// Fetches a result set from a data source object
        /// </summary>
        /// <param name="Connection">
        /// An open connection object
        /// </param>
        /// <param name="SourceObject">
        /// Data source object to fetch from
        /// </param>
        /// <param name="Fields">
        /// List of fields tp fetch (SQL valid syntax)
        /// </param>
        /// <param name="Condition">
        /// ClsQueryCondition Object to use the filter the result set
        /// </param>
        /// <param name="Sort">
        /// Sort expression to use to sort the result set (SQL Order By valid syntax)
        /// </param>
        /// <param name="Top">
        /// Limits the number of returned rows in the result set,
        /// used in pagination
        /// </param>
        /// <param name="Page">
        /// Selects a section based on the Page and Top values in the result set,
        /// used in pagination
        /// </param>
        /// <returns></returns>
        public DataTable GetQuery(Interface_Connection Connection, String SourceObject, String Fields, QueryCondition Condition, String Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            SourceObject = Do_Methods.Convert_String(SourceObject);
            Fields = Do_Methods.Convert_String(Fields);
            Sort = Do_Methods.Convert_String(Sort);

            string Query_RowNumberSort = Sort;
            if (Query_RowNumberSort.Trim() == "") Query_RowNumberSort = "(Select 0)";

            string Query_Top = "";
            if (Top > 0) Query_Top = "Top " + Top.ToString();

            Int64 PageCondition = 0;
            if (Page > 0)
            { PageCondition = Top * (Page - 1); }

            if (SourceObject.Trim() != "") SourceObject = " From " + SourceObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            PreparedQuery Pq = this.CreatePreparedQuery();
            string Query_Condition = "";
            if (Condition != null)
            {
                Query_Condition = " Where 1 = 1 ";
                Query_Condition += " And " + Condition.GetQueryCondition();
                Pq.Add_Parameter(Condition.GetParameters());
            }

            Pq.pQuery = @"Select " + Query_Top + @" [Tb].* From ( Select Row_Number() Over (Order By " + Query_RowNumberSort + @") As [RowNumber], " + Fields + " " + SourceObject + " " + Query_Condition + @" ) As [Tb] Where [Tb].RowNumber > " + PageCondition + " " + Sort;
            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }

        /// <summary>
        /// Fetches a result set from a data source object
        /// </summary>
        /// <param name="SourceObject">
        /// Data source object to fetch from
        /// </param>
        /// <param name="Fields">
        /// List of fields tp fetch (SQL valid syntax)
        /// </param>
        /// <param name="Condition">
        /// ClsQueryCondition Object to use the filter the result set
        /// </param>
        /// <param name="Sort">
        /// Sort expression to use to sort the result set (SQL Order By valid syntax)
        /// </param>
        /// <param name="Top">
        /// Limits the number of returned rows in the result set,
        /// used in pagination
        /// </param>
        /// <param name="Page">
        /// Selects a section based on the Page and Top values in the result set,
        /// used in pagination
        /// </param>
        /// <returns></returns>
        public DataTable GetQuery(String SourceObject, String Fields, QueryCondition Condition, String Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort, Top, Page);
                }
                catch (Exception ex)
                { throw ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.GetQuery(this.mConnection, SourceObject, Fields, Condition, Sort, Top, Page); }
        }

        public DataTable GetQuery(Interface_Connection Connection, Do_Constants.Str_QuerySource SourceObject, string Fields, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Fields = Do_Methods.Convert_String(Fields);
            Sort = Do_Methods.Convert_String(Sort);

            string Query_RowNumberSort = Sort;
            if (Query_RowNumberSort.Trim() == "") 
            { Query_RowNumberSort = "(Select 0)"; }

            string Query_Top = "";
            if (Top > 0) 
            { Query_Top = "Top " + Top.ToString(); }

            Int64 PageCondition = 0;
            if (Page > 0)
            { PageCondition = Top * (Page - 1); }

            if (SourceObject.ObjectName.Trim() != "")
            { SourceObject.ObjectName = @" From  " + SourceObject.ObjectName + " "; }
            if (SourceObject.Fields.Trim() == "")
            { SourceObject.Fields = @" * "; }
            if (SourceObject.Condition.Trim() != "")
            { SourceObject.Condition = @" Where " + SourceObject.Condition; }

            if (Fields.Trim() == "") 
            { Fields = " * "; }
            if (Sort.Trim() != "") 
            { Sort = " Order By " + Sort; }

            PreparedQuery Pq = this.CreatePreparedQuery();
            string Query_Condition = "";
            if (Condition != null)
            {
                Query_Condition = " Where 1 = 1 ";
                Query_Condition += " And " + Condition.GetQueryCondition();
                Pq.Add_Parameter(Condition.GetParameters());
            }

            //Pq.pQuery = @"Select " + Query_Top + @" [Tb].* From ( Select Row_Number() Over (Order By " + Query_RowNumberSort + @") As [RowNumber], " + Fields + " " + SourceObject + " " + Query_Condition + @" ) As [Tb] Where [Tb].RowNumber > " + PageCondition + " " + Sort;
            //Pq.pQuery = @"Declare @Query As VarChar(Max); Set @Query = 'Select ' + @Top + ' [Tb].* From ( Select Row_Number() Over (Order By ' + @RowNumberSort + ') As [RowNumber], ' + @Fields + ' ' + @ViewObject + ' ' + @Condition + ' ) As [Tb] Where [Tb].RowNumber >= ' + @PageCondition + ' ' + @Sort; Exec(@Query)";
            Pq.pQuery = @"Declare @ViewObject As VarChar(Max); Set @ViewObject = ' Select * ' + @Param_ViewObject_TableName + ' ' + @Param_ViewObject_Condition + ' ';  Declare @Query As VarChar(Max); Set @Query = 'Select ' + @Param_Top + ' [Tb].* From ( Select Row_Number() Over (Order By ' + @Param_RowNumberSort + ') As [RowNumber] , ' + @Param_Fields + ' From ('+ @ViewObject + ') As [Source] '" + @Query_Condition + @"' ) As [Tb] Where [Tb].RowNumber >= ' + @Param_PageCondition + ' ' + @Param_Sort; Exec(@Query);";
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_ViewObject_TableName", Value = SourceObject.ObjectName, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_ViewObject_Condition", Value = SourceObject.Condition, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_Top", Value = Query_Top, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_RowNumberSort", Value = Query_RowNumberSort, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_PageCondition", Value = PageCondition.ToString(), Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_Fields", Value = Fields, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Add_Parameter(new QueryParameter() { Name = "Param_Sort", Value = Sort, Type = Do_Constants.eParameterType.VarChar, Size = 8000 });
            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }

        public DataTable GetQuery(Do_Constants.Str_QuerySource SourceObject, String Fields, QueryCondition Condition, String Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort, Top, Page);
                }
                catch (Exception ex)
                { throw ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.GetQuery(this.mConnection, SourceObject, Fields, Condition, Sort, Top, Page); }
        }
        
        public int ExecuteNonQuery(Interface_Connection Connection, string ProcedureName, List<QueryParameter> ProcedureParameters)
        { return (Connection as Connection_SqlServer).ExecuteNonQuery(ProcedureName, ProcedureParameters); }

        public int ExecuteNonQuery(string ProcedureName, List<QueryParameter> ProcedureParameters)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.ExecuteNonQuery(Cn, ProcedureName, ProcedureParameters);
                }
                catch (Exception Ex) { throw Ex; }
                finally { Cn.Close(); }
            }
            else
            { return this.ExecuteNonQuery(this.mConnection, ProcedureName, ProcedureParameters); }
        }

        public int ExecuteNonQuery(Interface_Connection Connection, string Query)
        { return (Connection as Connection_SqlServer).ExecuteNonQuery(Query); }

        public int ExecuteNonQuery(string Query)
        {
            Connection_SqlServer Cn = new Connection_SqlServer();
            try
            {
                Cn.Connect();
                return ExecuteNonQuery(Cn, Query);
            }
            catch (Exception Ex) { throw Ex; }
            finally { Cn.Close(); }
        }

        public Int32 ExecuteNonQuery(Interface_Connection Cn, DbCommand Cmd)
        { return (Cn as Connection_SqlServer).ExecuteNonQuery((SqlCommand)Cmd); }

        public Int32 ExecuteNonQuery(DbCommand Cmd)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return ExecuteNonQuery(Cn, Cmd);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.ExecuteNonQuery(this.mConnection, Cmd); }
        }

        public DataSet ExecuteQuery(Interface_Connection Connection, string ProcedureName, List<QueryParameter> ProcedureParameters)
        { return (Connection as Connection_SqlServer).ExecuteQuery(ProcedureName, ProcedureParameters); }

        public DataSet ExecuteQuery(string ProcedureName, List<QueryParameter> ProcedureParameters)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return ExecuteQuery(Cn, ProcedureName, ProcedureParameters);
                }
                catch (Exception Ex) { throw Ex; }
                finally { Cn.Close(); }
            }
            else
            { return ExecuteQuery(this.mConnection, ProcedureName, ProcedureParameters); }
        }

        public DataSet ExecuteQuery(Interface_Connection Connection, string Query)
        { return (Connection as Connection_SqlServer).ExecuteQuery(Query); }

        public DataSet ExecuteQuery(string Query)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return ExecuteQuery(Cn, Query);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.ExecuteQuery(this.mConnection, Query); }
        }

        public DataSet ExecuteQuery(Interface_Connection Cn, DbCommand Cmd)
        {
            return (Cn as Connection_SqlServer).ExecuteQuery((SqlCommand)Cmd);
        }

        public DataSet ExecuteQuery(DbCommand Cmd)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return ExecuteQuery(Cn, Cmd);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.ExecuteQuery(this.mConnection, Cmd); }
        }

        /// <summary>
        /// Gets the current using Connection Object used by this instance
        /// </summary>
        public Interface_Connection Connection
        {
            get { return this.mConnection; }
        }

        /// <summary>
        /// Connects to the datasource defined in Do_Globals.gSettings.Datasource
        /// </summary>
        public void Connect()
        {
            this.mConnection = new Connection_SqlServer();
            this.mConnection.Connect();
        }

        /// <summary>
        /// Connects to the specified datasource
        /// </summary>
        /// <param name="ConnectionString">
        /// The specified connection definition to the datasource
        /// </param>
        public void Connect(string ConnectionString)
        {
            this.mConnection = new Connection_SqlServer();
            this.mConnection.Connect(ConnectionString);
        }

        /// <summary>
        /// Closes the current connection
        /// </summary>
        public void Close()
        { this.mConnection.Close(); }

        /// <summary>
        /// Begins a new transaction
        /// </summary>
        public void BeginTransaction()
        { this.mConnection.BeginTransaction(); }

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        public void CommitTransaction()
        { this.mConnection.CommitTransaction(); }

        /// <summary>
        /// Reverts the current transaction
        /// </summary>
        public void RollbackTransaction()
        { this.mConnection.RollbackTransaction(); }

        /// <summary>
        /// Saves the datarow to the target table
        /// </summary>
        /// <param name="ObjDataRow">
        /// The source datarow to be saved
        /// </param>
        /// <param name="TableName">
        /// The name of the table to be operated
        /// </param>
        /// <param name="SchemaName">
        /// The name of the schema of the target table
        /// </param>
        /// <param name="IsDelete">
        /// If true, the operation will be a Delete operation
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        /// <returns></returns>
        public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false, List<string> CustomKeys = null)
        { return this.mConnection.SaveDataRow(ObjDataRow, TableName, SchemaName, IsDelete, CustomKeys); }

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
        /// <param name="Condition">
        /// Additional conditions to be used in fetching the data
        /// </param>
        /// <param name="Sort">
        /// Additional sorting to be used in fetching the data
        /// </param>
        /// <returns></returns>
        public DataTable List(
            string ObjectName
            , string Condition = ""
            , string Sort = "")
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.List(Cn, ObjectName, Condition, Sort);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.List(this.mConnection, ObjectName, Condition, Sort); }
        }

        public DataTable List(Interface_Connection Cn, string ObjectName, string Condition = "", string Sort = "")
        {
            DataTable Dt = this.GetQuery(Cn, ObjectName, "*", Condition, Sort);
            return Dt;
        }

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
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
        public DataTable List(
            string ObjectName
            , QueryCondition Condition
            , string Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.List(Cn, ObjectName, Condition, Sort, Top, Page);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.List(this.mConnection, ObjectName, Condition, Sort, Top, Page); }
        }

        public DataTable List(Interface_Connection Cn, string ObjectName, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            DataTable Dt = this.GetQuery(Cn, ObjectName, "*", Condition, Sort, Top, Page);
            return Dt;
        }

        /// <summary>
        /// Returns the Result Set Count with out actually fetching the result set, mainly used for pagination
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
        /// <param name="Condition">
        /// ClsQueryCondition Object to be used in fetching the data
        /// </param>
        /// <returns></returns>
        public long List_Count(string ObjectName, QueryCondition Condition = null)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.List_Count(Cn, ObjectName, Condition);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.List_Count(this.mConnection, ObjectName, Condition); }
        }

        public long List_Count(Interface_Connection Cn, string ObjectName, QueryCondition Condition = null)
        {
            DataTable Dt = this.GetQuery(Cn, ObjectName, "Count(1) As [Ct]", Condition);
            Int64 ReturnValue = 0;
            try { ReturnValue = Do_Methods.Convert_Int64(Dt.Rows[0]["Ct"], 0); }
            catch { }
            return ReturnValue;
        }

        /// <summary>
        /// Returns a Empy List based on the supplied source data object Name
        /// Used for getting the definition of the data object
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
        /// <returns></returns>
        public DataTable List_Empty(string ObjectName)
        {
            if (this.mConnection == null)
            {
                Connection_SqlServer Cn = new Connection_SqlServer();
                try
                {
                    Cn.Connect();
                    return this.List_Empty(Cn, ObjectName);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                { Cn.Close(); }
            }
            else
            { return this.List_Empty(this.mConnection, ObjectName); }
        }

        public DataTable List_Empty(Interface_Connection Cn, string ObjectName)
        {
            return this.GetQuery(Cn, ObjectName, "*", "1 = 0");
        }

        /// <summary>
        /// Loads the Data Object with the supplied Key,
        /// when loading table details, the framework assumes the foreign key field of the table detail is the same the parent table
        /// if not supplied by an explicit foreign key definition
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object to be fetched
        /// </param>
        /// <param name="List_Key">
        /// The defined Key names of the data objects
        /// </param>
        /// <param name="Keys">
        /// The ClsKey object to use
        /// </param>
        /// <returns></returns>
        public DataRow Load(string ObjectName, List<string> List_Key, Keys Keys)
        {
            DataTable Dt;
            DataRow Dr;

            if (Keys == null)
            {
                //Dr = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0").NewRow(); 
                Dt = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0");
                Dr = Dt.NewRow();
                Dt.Rows.Add(Dr);
            }
            else
            {
                StringBuilder Sb_Condition = new StringBuilder();

                if (Keys.Count() != List_Key.Count)
                { throw new Exception("Keys not equal to required keys."); }

                Sb_Condition.Append(" 1 = 1 ");
                foreach (string Inner_Key in List_Key)
                { Sb_Condition.Append(" And " + Inner_Key + " = " + Keys[Inner_Key]); }

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString());
                if (Dt.Rows.Count > 0)
                { Dr = Dt.Rows[0]; }
                else
                { throw new CustomException("Record not found."); }
            }
            return Dr;
        }

        /// <summary>
        /// Loads the defined Data Object Table Detail
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object of the Table Detail
        /// </param>
        /// <param name="Keys">
        /// The ClsKey object to use
        /// </param>
        /// <param name="Condition">
        /// Additional conditions to be used in fetching the data
        /// </param>
        /// <param name="ForeignKeys">
        /// Custom defined Keys of the Table Detail
        /// </param>
        /// <returns></returns>
        public DataTable Load_TableDetails(string ObjectName, Keys Keys, string Condition, List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            DataTable Dt;

            if (Keys == null)
            { Dt = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0"); }
            else
            {
                /*
                string Inner_Condition_And = "";
                bool IsStart = false;
                foreach (string Inner_Key in Keys.pName)
                {
                    Sb_Condition.Append(Inner_Condition_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
                    if (!IsStart) Inner_Condition_And = " And ";
                    IsStart = true;
                }

                string OtherCondition = "";
                if (Condition != "") OtherCondition = " And " + Condition;

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString() + OtherCondition);
                */

                if (ForeignKeys == null)
                {
                    string Inner_Condition_And = "";
                    bool IsStart = false;
                    foreach (string Inner_Key in Keys.pName)
                    {
                        Sb_Condition.Append(Inner_Condition_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
                        if (!IsStart) { Inner_Condition_And = " And "; }
                        IsStart = true;
                    }
                }
                else
                {
                    string Inner_Condition_And = "";
                    bool IsStart = false;
                    foreach (string Inner_Key in Keys.pName)
                    {
                        Do_Constants.Str_ForeignKeyRelation Fk = ForeignKeys.FirstOrDefault(item => item.Parent_Key == Inner_Key);

                        if (Fk.Parent_Key == string.Empty) { throw new Exception("All foreign keys must match the parent keys."); }

                        Sb_Condition.Append(Inner_Condition_And + " " + Fk.Child_Key + " = " + Keys[Inner_Key]);
                        if (!IsStart) { Inner_Condition_And = " And "; }
                        IsStart = true;
                    }
                }

                string OtherCondition = "";
                if (Condition != "") { OtherCondition = " And " + Condition; }

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString() + OtherCondition);
            }

            return Dt;
        }

        /// <summary>
        /// Loads the defined Data Object Row Detail
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object of the Row Detail
        /// </param>
        /// <param name="Keys">
        /// The ClsKey object to use
        /// </param>
        /// <param name="Condition">
        /// Additional conditions to be used in fetching the data
        /// </param>
        /// <param name="ForeignKeys">
        /// Custom defined Keys of the Row Detail
        /// </param>
        /// <returns></returns>
        public DataRow Load_RowDetails(string ObjectName, Keys Keys, string Condition, List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            DataTable Dt = null;
            DataRow Dr = null;

            if (Keys == null)
            { Dr = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0").NewRow(); }
            else
            {
                /*
                string Inner_Condition_And = "";
                bool IsStart = false;
                foreach (string Inner_Key in Keys.pName)
                {
                    Sb_Condition.Append(Inner_Condition_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
                    if (!IsStart) Inner_Condition_And = " And ";
                    IsStart = true;
                }

                string OtherCondition = "";
                if (Condition != "") OtherCondition = " And " + Condition;

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString() + OtherCondition);
                if (Dt.Rows.Count > 0) Dr = Dt.Rows[0];
                else Dr = Dt.NewRow();
                */

                if (ForeignKeys == null)
                {
                    string Inner_Condition_And = "";
                    bool IsStart = false;
                    foreach (string Inner_Key in Keys.pName)
                    {
                        Sb_Condition.Append(Inner_Condition_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
                        if (!IsStart) { Inner_Condition_And = " And "; }
                        IsStart = true;
                    }
                }
                else
                {
                    string Inner_Condition_And = "";
                    bool IsStart = false;
                    foreach (string Inner_Key in Keys.pName)
                    {
                        Do_Constants.Str_ForeignKeyRelation Fk = ForeignKeys.FirstOrDefault(item => item.Parent_Key == Inner_Key);

                        if (Fk.Parent_Key == string.Empty) { throw new Exception("All foreign keys must match the parent keys."); }

                        Sb_Condition.Append(Inner_Condition_And + " " + Fk.Child_Key + " = " + Keys[Inner_Key]);
                        if (!IsStart) { Inner_Condition_And = " And "; }
                        IsStart = true;
                    }

                    string OtherCondition = "";
                    if (Condition != "") { OtherCondition = " And " + Condition; }

                    Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString() + OtherCondition);
                    if (Dt.Rows.Count > 0) { Dr = Dt.Rows[0]; }
                    else { Dr = Dt.NewRow(); }
                }
            }
            return Dr;
        }

        public Interface_Connection CreateConnection()
        {
            return new Connection_SqlServer();
        }

        /// <summary>
        /// Creates a ClsQueryCondition based on the SQL Server implementation
        /// </summary>
        /// <returns></returns>
        public QueryCondition CreateQueryCondition()
        { return new QueryCondition(); }

        public PreparedQuery CreatePreparedQuery(Interface_Connection Cn, string Query = "", List<QueryParameter> Parameters = null)
        { return new PreparedQuery_SqlServer(Cn, Query, Parameters); }

        public PreparedQuery CreatePreparedQuery(string Query = "", List<QueryParameter> Parameters = null)
        {
            Connection_SqlServer Cn = this.mConnection;
            if (Cn == null)
            {
                Cn = new Connection_SqlServer();
                Cn.Connect();
            }

            return new PreparedQuery_SqlServer(Cn, Query, Parameters);
        }

        /// <summary>
        /// Gets the definition of the requested data object
        /// </summary>
        /// <param name="TableName">
        /// The requested data object name
        /// </param>
        /// <returns></returns>
        public DataTable GetTableDef(string TableName)
        {
            DataTable Rv = null;
            List<QueryParameter> Sp = new List<QueryParameter>();
            Sp.Add(new QueryParameter("@TableName", TableName));
            Connection_SqlServer Cn = new Connection_SqlServer();
            try
            {
                Cn.Connect();
                Rv = Cn.ExecuteQuery("usp_DataObjects_GetTableDef", Sp).Tables[0];
            }
            catch { }
            finally
            { Cn.Close(); }
            return Rv;
        }

        /// <summary>
        /// Gets the specified system parameter value, or creates a new system parameter with the specified default value
        /// </summary>
        /// <param name="ParameterName">
        /// The system parameter name
        /// </param>
        /// <param name="DefaultValue">
        /// The default value for the parameter if it doesn't exists
        /// </param>
        /// <returns></returns>
        public string GetSystemParameter(string ParameterName, string DefaultValue = "")
        {
            Connection_SqlServer Cn = new Connection_SqlServer();
            try
            {
                Cn.Connect();
                return this.GetSystemParameter(Cn, ParameterName, DefaultValue);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        /// <summary>
        /// Gets the specified system parameter value, or creates a new system parameter with the specified default value
        /// </summary>
        /// <param name="Connection">
        /// An open connection object
        /// </param>
        /// <param name="ParameterName">
        /// The system parameter name
        /// </param>
        /// <param name="DefaultValue">
        /// The default value for the parameter if it doesn't exists
        /// </param>
        /// <returns></returns>
        public string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "")
        {
            Connection_SqlServer Cn = (Connection_SqlServer)Connection;

            string Rv = "";
            List<QueryParameter> Sp = new List<QueryParameter>();
            Sp.Add(new QueryParameter("ParameterName", ParameterName));
            Sp.Add(new QueryParameter("DefaultValue", DefaultValue));
            DataTable Dt = Cn.ExecuteQuery("usp_DataObjects_Parameter_Get", Sp).Tables[0];
            if (Dt.Rows.Count > 0)
            { Rv = (string)Dt.Rows[0][0]; }
            return Rv;
        }

        /// <summary>
        /// Sets a new value to the specified system parameter
        /// </summary>
        /// <param name="ParameterName">
        /// The system parameter name
        /// </param>
        /// <param name="ParameterValue">
        /// The value to be set
        /// </param>
        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            Connection_SqlServer Cn = new Connection_SqlServer();
            try
            {
                Cn.Connect();
                this.SetSystemParameter(ParameterName, ParameterValue);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        /// <summary>
        /// Sets a new value to the specified system parameter
        /// </summary>
        /// <param name="Connection">
        /// An open connection object
        /// </param>
        /// <param name="ParameterName">
        /// The system parameter name
        /// </param>
        /// <param name="ParameterValue">
        /// The value to be set
        /// </param>
        public void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue)
        {
            Connection_SqlServer Cn = (Connection_SqlServer)Connection;
            List<QueryParameter> Sp = new List<QueryParameter>();
            Sp.Add(new QueryParameter("ParameterName", ParameterName));
            Sp.Add(new QueryParameter("ParameterValue", ParameterValue));
            Cn.ExecuteNonQuery("usp_DataObjects_Parameter_Set", Sp);
        }

        public void Dispose()
        {
            if (this.mConnection != null)
            { this.mConnection.Close(); }
        }

        public void InvokeError()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
