using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using DataObjects_Framework;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Common;
using DataObjects_Framework.Objects;

namespace DataObjects_Framework.DataAccess
{
    public class ClsDataAccess_Wcf : Interface_DataAccess
    {
        #region _Variables

        string mConnectionString = "";

        #endregion

        #region _ImplementedMethods

        public DataTable GetQuery(Connection.Interface_Connection Connection, string SourceObject, string Fields = "", string Condition = "", string Sort = "", long Top = 0, int Page = 0)
        {
            throw new NotImplementedException();
        }

        public DataTable GetQuery(string SourceObject, string Fields = "", string Condition = "", string Sort = "", long Top = 0, int Page = 0)
        {
            throw new NotImplementedException();
        }

        public DataTable GetQuery(Connection.Interface_Connection Connection, string SourceObject, string Fields, Objects.ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            throw new NotImplementedException();
        }

        public DataTable GetQuery(string SourceObject, string Fields, Objects.ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(Connection.Interface_Connection Connection, string ProcedureName, List<Common.Do_Constants.Str_Parameters> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string ProcedureName, List<Common.Do_Constants.Str_Parameters> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(Connection.Interface_Connection Connection, string Query)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string Query)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(Connection.Interface_Connection Connection, string ProcedureName, List<Common.Do_Constants.Str_Parameters> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(string ProcedureName, List<Common.Do_Constants.Str_Parameters> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(Connection.Interface_Connection Connection, string Query)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(string Query)
        {
            throw new NotImplementedException();
        }

        public Interface_Connection Connection
        {
            get { throw new NotImplementedException(); }
        }

        public void Connect()
        { this.mConnectionString = Do_Globals.gSettings.pConnectionString; }

        public void Connect(string ConnectionString)
        { this.mConnectionString = ConnectionString; }

        public void Close()
        { this.mConnectionString = ""; }

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false, List<string> CustomKeys = null)
        {
            throw new NotImplementedException();
        }

        public DataTable List(string ObjectName, string Condition = "", string Sort = "")
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition_String = Condition;
            Rl.Sort = Sort;
            Rl.ConnectionString = this.mConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List(Rl);
            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);

            return Sdt.ToDataTable();
        }

        public DataTable List(string ObjectName, Objects.ClsQueryCondition Condition, string Sort = "", int Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.Sort = Sort;
            Rl.Top = Top;
            Rl.Page = Page;
            Rl.ConnectionString = this.mConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List(Rl);
            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);

            return Sdt.ToDataTable();
        }

        public long List_Count(string ObjectName, Objects.ClsQueryCondition Condition = null)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.ConnectionString = this.mConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            Int64 ResponseData = Client.List_Count(Rl);

            return ResponseData;
        }

        public DataTable List_Empty(string ObjectName)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.ConnectionString = this.mConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List_Empty(Rl);
            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);

            return Sdt.ToDataTable();
        }

        public DataRow Load(string ObjectName, List<string> List_Key, Objects.ClsKeys Keys)
        {
            throw new NotImplementedException();
        }

        public DataTable Load_TableDetails(string ObjectName, Objects.ClsKeys Keys, string Condition, List<Common.Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            throw new NotImplementedException();
        }

        public DataRow Load_RowDetails(string ObjectName, Objects.ClsKeys Keys, string Condition, List<Common.Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            throw new NotImplementedException();
        }

        public Objects.ClsQueryCondition CreateQueryCondition()
        {
            throw new NotImplementedException();
        }

        public DataTable GetTableDef(string TableName)
        {
            throw new NotImplementedException();
        }

        public string GetSystemParameter(string ParameterName, string DefaultValue = "")
        {
            throw new NotImplementedException();
        }

        public string GetSystemParameter(Connection.Interface_Connection Connection, string ParameterName, string DefaultValue = "")
        {
            throw new NotImplementedException();
        }

        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            throw new NotImplementedException();
        }

        public void SetSystemParameter(Connection.Interface_Connection Connection, string ParameterName, string ParameterValue)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
