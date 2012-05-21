using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;
using DataObjects_Framework;
using DataObjects_Framework._System;

namespace DataObjects_Framework.DataAccess
{
    public interface Interface_DataAccess
    {

        DataTable GetQuery(
            Interface_Connection Connection
            , string ViewObject
            , string Fields = ""
            , string Condition = ""
            , string Sort = "");
        
        DataTable GetQuery(
            string ViewObject
            , string Fields = ""
            , string Condition = ""
            , string Sort = "");
        
        DataTable GetQuery(
            Interface_Connection Connection
            , string ViewObject
            , string Fields
            , ClsQueryCondition Condition
            , string Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);

        DataTable GetQuery(
            string ViewObject
            , string Fields
            , ClsQueryCondition Condition
            , string Sort = ""
            , Int64 Top = 0
            , Int32 Page = 0);
        
        Interface_Connection Connection { get; }

        //[-]

        void Connect();

        void Close();

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
        
        bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false);
        
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

        DataTable Load_TableDetails(string ObjectName, ClsKeys Keys, string Condition);

        DataRow Load_RowDetails(string ObjectName, ClsKeys Keys, string Condition);

        //[-]

        ClsQueryCondition CreateQueryCondition();

        //[-]

        DataTable GetTableDef(string TableName);

        string GetSystemParameter(string ParameterName, string DefaultValue = "");

        string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "");

        void SetSystemParameter(string ParameterName, string ParameterValue);

        void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue);

        //[-]

        void AddSelected(
            DataTable Dt_Target
            , List<Int64> Selected_IDs
            , string Selected_DataSourceName
            , string Selected_KeyName
            , string Target_Key
            , bool HasTmpKey = false
            , List<Layer01_Constants.Str_AddSelectedFields> List_Selected_Fields = null
            , List<Layer01_Constants.Str_AddSelectedFieldsDefault> List_Selected_FieldsDefault = null);

    }
}
