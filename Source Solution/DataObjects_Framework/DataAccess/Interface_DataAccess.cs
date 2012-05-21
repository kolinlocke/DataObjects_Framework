using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;

namespace DataObjects_Framework.DataAccess
{
    public interface Interface_DataAccess
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
        /// <returns></returns>
        DataTable GetQuery(
            Interface_Connection Connection
            , string SourceObject
            , string Fields = ""
            , string Condition = ""
            , string Sort = "");
        
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
        /// <returns></returns>
        DataTable GetQuery(
            string SourceObject
            , string Fields = ""
            , string Condition = ""
            , string Sort = "");

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
            , string SourceObject
            , string Fields
            , ClsQueryCondition Condition
            , string Sort = ""
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
            string SourceObject
            , string Fields
            , ClsQueryCondition Condition
            , string Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);
        
        /// <summary>
        /// Gets the current using Connection Object used by this instance
        /// </summary>
        Interface_Connection Connection { get; }

        //[-]

        /// <summary>
        /// Connects to the defined datasource
        /// </summary>
        void Connect();

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
        /// <returns></returns>
        bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false, List<string> CustomKeys = null);
        
        //[-]

        DataTable List(
            string ObjectName
            , string Condition = ""
            , string Sort = "");
        
        DataTable List(
            string ObjectName
            , ClsQueryCondition Condition
            , string Sort = ""
            , Int32 Top = 0
            , Int32 Page = 0);

        Int64 List_Count(string ObjectName, ClsQueryCondition Condition = null);

        DataTable List_Empty(Interface_Connection Connection, string ObjectName);

        DataTable List_Empty(string ObjectName);

        DataRow Load(string ObjectName, List<string> List_Key, ClsKeys Keys);

        DataTable Load_TableDetails(
            string ObjectName
            , ClsKeys Keys
            , string Condition
            , List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys);

        DataRow Load_RowDetails(
            string ObjectName
            , ClsKeys Keys
            , string Condition
            , List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys);

        //[-]

        ClsQueryCondition CreateQueryCondition();

        //[-]

        DataTable GetTableDef(string TableName);

        string GetSystemParameter(string ParameterName, string DefaultValue = "");

        string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "");

        void SetSystemParameter(string ParameterName, string ParameterValue);

        void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue);

        //[-]

        /*
        void AddSelected(
            DataTable Dt_Target
            , List<Int64> Selected_IDs
            , string Selected_DataSourceName
            , string Selected_KeyName
            , string Target_Key
            , bool HasTmpKey = false
            , List<Constants.Str_AddSelectedFields> List_Selected_Fields = null
            , List<Constants.Str_AddSelectedFieldsDefault> List_Selected_FieldsDefault = null);
        */

    }
}
