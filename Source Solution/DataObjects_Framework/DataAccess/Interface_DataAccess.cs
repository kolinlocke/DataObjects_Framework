using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.PreparedQueryObjects;

namespace DataObjects_Framework.DataAccess
{
    /// <summary>
    /// Interface for Data Access Methods
    /// </summary>
    public interface Interface_DataAccess : IDisposable
    {
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
        DataTable GetQuery(
            Interface_Connection Connection
            , String SourceObject
            , String Fields = ""
            , String Condition = ""
            , String Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

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
        DataTable GetQuery(
            String SourceObject
            , String Fields = ""
            , String Condition = ""
            , String Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

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
        DataTable GetQuery(
            Interface_Connection Connection
            , String SourceObject
            , String Fields
            , QueryCondition Condition
            , String Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

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
        DataTable GetQuery(
            String SourceObject
            , String Fields
            , QueryCondition Condition
            , String Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

        DataTable GetQuery(
            Interface_Connection Connection
            , Do_Constants.Str_QuerySource SourceObject
            , String Fields
            , QueryCondition Condition
            , String Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

        DataTable GetQuery(
            Do_Constants.Str_QuerySource SourceObject
            , String Fields
            , QueryCondition Condition
            , String Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

        Int32 ExecuteNonQuery(Interface_Connection Connection, string ProcedureName, List<QueryParameter> ProcedureParameters);

        Int32 ExecuteNonQuery(string ProcedureName, List<QueryParameter> ProcedureParameters);

        Int32 ExecuteNonQuery(Interface_Connection Connection, string Query);

        Int32 ExecuteNonQuery(string Query);

        Int32 ExecuteNonQuery(Interface_Connection Cn, DbCommand Cmd);

        Int32 ExecuteNonQuery(DbCommand Cmd);

        DataSet ExecuteQuery(Interface_Connection Connection, string ProcedureName, List<QueryParameter> ProcedureParameters);

        DataSet ExecuteQuery(string ProcedureName, List<QueryParameter> ProcedureParameters);

        DataSet ExecuteQuery(Interface_Connection Connection, string Query);

        DataSet ExecuteQuery(string Query);

        DataSet ExecuteQuery(Interface_Connection Cn, DbCommand Cmd);

        DataSet ExecuteQuery(DbCommand Cmd);

        /// <summary>
        /// Gets the current using Connection Object used by this instance
        /// </summary>
        Interface_Connection Connection { get; }

        //[-]

        /// <summary>
        /// Connects to the datasource defined in Do_Globals.gSettings.Datasource
        /// </summary>
        void Connect();

        /// <summary>
        /// Connects to the specified datasource
        /// </summary>
        /// <param name="ConnectionString">
        /// The specified connection definition to the datasource
        /// </param>
        void Connect(string ConnectionString);

        /// <summary>
        /// Closes the current connection
        /// </summary>
        void Close();

        /// <summary>
        /// Begins a new transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the current transaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Reverts the current transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Saves the datarow to the target table.
        /// </summary>
        /// <param name="ObjDataRow">
        /// The source datarow to be saved.
        /// </param>
        /// <param name="TableName">
        /// The name of the table to be operated.
        /// </param>
        /// <param name="SchemaName">
        /// The name of the schema of the target table.
        /// </param>
        /// <param name="IsDelete">
        /// If true, the operation will be a Delete operation.
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition.
        /// </param>
        /// <returns></returns>
        bool SaveDataRow(
            DataRow ObjDataRow
            , string TableName
            , string SchemaName = ""
            , bool IsDelete = false
            , List<string> CustomKeys = null);

        //[-]

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
        DataTable List(
            string ObjectName
            , string Condition = ""
            , string Sort = "");

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="Cn">
        /// An open connection object
        /// </param>
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
        DataTable List(
            Interface_Connection Cn
            , string ObjectName
            , string Condition = ""
            , string Sort = "");

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
        DataTable List(
            string ObjectName
            , QueryCondition Condition
            , string Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

        /// <summary>
        /// Returns a List based on the supplied Table/View Name
        /// </summary>
        /// <param name="Cn">
        /// An open connection object
        /// </param>
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
        DataTable List(
            Interface_Connection Cn
            , string ObjectName
            , QueryCondition Condition
            , string Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

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
        Int64 List_Count(string ObjectName, QueryCondition Condition = null);

        /// <summary>
        /// Returns the Result Set Count with out actually fetching the result set, mainly used for pagination
        /// </summary>
        /// <param name="Cn">
        /// An open connection object
        /// </param>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
        /// <param name="Condition">
        /// ClsQueryCondition Object to be used in fetching the data
        /// </param>
        /// <returns></returns>
        Int64 List_Count(Interface_Connection Cn, string ObjectName, QueryCondition Condition = null);

        /// <summary>
        /// Returns a Empy List based on the supplied source data object Name
        /// Used for getting the definition of the data object
        /// </summary>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
        /// <returns></returns>
        DataTable List_Empty(string ObjectName);

        /// <summary>
        /// Returns a Empy List based on the supplied source data object Name
        /// Used for getting the definition of the data object
        /// </summary>
        /// <param name="Cn">
        /// An open connection object
        /// </param>
        /// <param name="ObjectName">
        /// The source data object name
        /// </param>
        /// <returns></returns>
        DataTable List_Empty(Interface_Connection Cn, string ObjectName);

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
        DataRow Load(string ObjectName, List<string> List_Key, Keys Keys);

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
        DataTable Load_TableDetails(
            string ObjectName
            , Keys Keys
            , string Condition
            , List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys);

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
        DataRow Load_RowDetails(
            string ObjectName
            , Keys Keys
            , string Condition
            , List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys);

        //[-]

        /// <summary>
        /// Creates a connection object based on the implementation of this interface
        /// </summary>
        /// <returns></returns>
        Interface_Connection CreateConnection();

        /// <summary>
        /// Creates a ClsQueryCondition based on the implementation of this interface
        /// </summary>
        /// <returns></returns>
        QueryCondition CreateQueryCondition();

        PreparedQuery CreatePreparedQuery(Interface_Connection Cn, String Query = "", List<QueryParameter> Parameters = null);

        PreparedQuery CreatePreparedQuery(String Query = "", List<QueryParameter> Parameters = null);

        //[-]

        /// <summary>
        /// Gets the definition of the requested data object
        /// </summary>
        /// <param name="TableName">
        /// The requested data object name
        /// </param>
        /// <returns></returns>
        DataTable GetTableDef(string TableName);

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
        string GetSystemParameter(string ParameterName, string DefaultValue = "");

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
        string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "");

        /// <summary>
        /// Sets a new value to the specified system parameter
        /// </summary>
        /// <param name="ParameterName">
        /// The system parameter name
        /// </param>
        /// <param name="ParameterValue">
        /// The value to be set
        /// </param>
        void SetSystemParameter(string ParameterName, string ParameterValue);

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
        void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue);

        void InvokeError();
    }
}

