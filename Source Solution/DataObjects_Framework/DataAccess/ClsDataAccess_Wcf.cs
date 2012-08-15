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

        //string mConnectionString = "";
        ClsConnection_Wcf mConnection;

        #endregion

        #region _ImplementedMethods

        public DataTable GetQuery(Interface_Connection Connection, string SourceObject, string Fields = "", string Condition = "", string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_GetQuery Rgq = new Do_Constants.Str_Request_GetQuery();
            Rgq.ObjectName = SourceObject;
            Rgq.Condition_String = Condition;
            Rgq.Sort = Sort;
            Rgq.Top = Top;
            Rgq.Page = Page;

            Rgq.ConnectionString = (Connection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetQuery(Rgq);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataTable GetQuery(string SourceObject, string Fields = "", string Condition = "", string Sort = "", long Top = 0, int Page = 0)
        {
            ClsConnection_Wcf Cn = new ClsConnection_Wcf();
            try
            {
                Cn.Connect();
                return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public DataTable GetQuery(Interface_Connection Connection, string SourceObject, string Fields, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_GetQuery Rgq = new Do_Constants.Str_Request_GetQuery();
            Rgq.ObjectName = SourceObject;
            Rgq.Condition = Condition;
            Rgq.Sort = Sort;
            Rgq.Top = Do_Methods.Convert_Int32(Top);
            Rgq.Page = Page;

            Rgq.ConnectionString = (Connection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetQuery(Rgq);
            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);

            return Sdt.ToDataTable();
        }

        public DataTable GetQuery(string SourceObject, string Fields, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            ClsConnection_Wcf Cn = new ClsConnection_Wcf();
            try
            {
                Cn.Connect();
                return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public int ExecuteNonQuery(Interface_Connection Connection, string ProcedureName, List<Common.Do_Constants.Str_Parameters> ProcedureParameters)
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
        { 
            this.mConnection = new ClsConnection_Wcf();
            this.mConnection.Connect();            
        }

        public void Connect(string ConnectionString)
        { 
            this.mConnection = new ClsConnection_Wcf();
            this.mConnection.Connect(ConnectionString);
        }

        public void Close()
        { 
            this.mConnection = null;
        }

        public void BeginTransaction() { }

        public void CommitTransaction() { }

        public void RollbackTransaction() { }

        public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false, List<string> CustomKeys = null)
        {
            Do_Constants.Str_Request_Save Rs = new Do_Constants.Str_Request_Save();
            Rs.TableName = TableName;
            Rs.SchemaName = SchemaName;
            Rs.IsDelete = IsDelete;
            Rs.CustomKeys = CustomKeys;

            ClsSimpleDataRow Sdr = new ClsSimpleDataRow(ObjDataRow);
            Rs.Serialized_ObjectDataRow = Sdr.Serialize();

            Client_WcfService Client = Client_WcfService.CreateObject();
            Boolean ResponseData = Client.SaveDataRow(Rs);

            return ResponseData;
        }

        public DataTable List(string ObjectName, string Condition = "", string Sort = "")
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition_String = Condition;
            Rl.Sort = Sort;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List(Rl);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataTable List(string ObjectName, ClsQueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.Sort = Sort;
            Rl.Top = Top;
            Rl.Page = Page;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

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
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            Int64 ResponseData = Client.List_Count(Rl);

            return ResponseData;
        }

        public DataTable List_Empty(string ObjectName)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List_Empty(Rl);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataRow Load(string ObjectName, List<string> List_Key, ClsKeys Keys)
        {
            Do_Constants.Str_Request_Load Rl = new Do_Constants.Str_Request_Load();
            Rl.ObjectName = ObjectName;
            Rl.ObjectKeys = List_Key;
            Rl.Key = Keys;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.Load(Rl);

            ClsSimpleDataRow Sdr = ClsSimpleDataRow.Deserialize(ResponseData);
            return Sdr.ToDataRow();
        }

        public DataTable Load_TableDetails(string ObjectName, ClsKeys Keys, string Condition, List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            Do_Constants.Str_Request_Load Rl = new Do_Constants.Str_Request_Load();
            Rl.ObjectName = ObjectName;
            Rl.ForeignKeys = ForeignKeys;
            Rl.Condition = Condition;
            Rl.Key = Keys;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.Load_TableDetails(Rl);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataRow Load_RowDetails(string ObjectName, ClsKeys Keys, string Condition, List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            Do_Constants.Str_Request_Load Rl = new Do_Constants.Str_Request_Load();
            Rl.ObjectName = ObjectName;
            Rl.ForeignKeys = ForeignKeys;
            Rl.Condition = Condition;
            Rl.Key = Keys;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.Load_RowDetails(Rl);

            ClsSimpleDataRow Sdr = ClsSimpleDataRow.Deserialize(ResponseData);
            return Sdr.ToDataRow();
        }

        public Interface_Connection CreateConnection()
        {
            return new ClsConnection_Wcf();
        }

        public ClsQueryCondition CreateQueryCondition()
        {
            return new ClsQueryCondition();
        }

        public DataTable GetTableDef(string TableName)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = TableName;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetTableDef(Rl);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public string GetSystemParameter(string ParameterName, string DefaultValue = "")
        {
            ClsConnection_Wcf Cn = new ClsConnection_Wcf();
            try
            {
                Cn.Connect();
                return this.GetSystemParameter(Cn, ParameterName, DefaultValue);
            }
            catch (Exception ex)
            { throw ex; }
        }

        public string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "")
        {
            Do_Constants.Str_Request_SystemParameter Rsp = new Do_Constants.Str_Request_SystemParameter();
            Rsp.ParameterName = ParameterName;
            Rsp.ParameterValue = DefaultValue;
            Rsp.ConnectionString = (Connection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.GetSystemParameter(Rsp);

            return ResponseData;
        }

        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            Do_Constants.Str_Request_SystemParameter Rsp = new Do_Constants.Str_Request_SystemParameter();
            Rsp.ParameterName = ParameterName;
            Rsp.ParameterValue = ParameterValue;
            Rsp.ConnectionString = (Connection as ClsConnection_Wcf).pConnectionString;

            Client_WcfService Client = Client_WcfService.CreateObject();
            Client.SetSystemParameter(Rsp);
        }

        public void SetSystemParameter(Connection.Interface_Connection Connection, string ParameterName, string ParameterValue)
        {
            ClsConnection_Wcf Cn = new ClsConnection_Wcf();
            try
            {
                Cn.Connect();
                this.SetSystemParameter(Cn, ParameterName, ParameterValue);
            }
            catch (Exception ex)
            { throw ex; }
        }

        #endregion
    }
}
