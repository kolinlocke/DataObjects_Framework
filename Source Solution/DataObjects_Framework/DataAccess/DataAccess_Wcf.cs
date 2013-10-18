using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.PreparedQueryObjects;

namespace DataObjects_Framework.DataAccess
{
    /// <summary>
    /// The WCF implementation of Interface_DataAccess
    /// Connects to a WCF Server App provided for DataObjects_Framework
    /// Requires explicit setting to Do_Globals.gSettings.pWcfAddress
    /// </summary>
    public class DataAccess_Wcf : Interface_DataAccess
    {
        #region _Variables

        Connection_Wcf mConnection;

        #endregion

        #region _Methods

        Connection_Wcf GetConnection()
        {
            Connection_Wcf Cn = null;
            if (this.mConnection == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }
            else
            { Cn = this.mConnection; }

            return Cn;
        }

        #endregion

        #region _ImplementedMethods

        //public string BuildQuery_SourceObject(string SourceObject, string Fields, string Condition, string Sort)
        //{
        //    Do_Constants.Str_Request_BuildQuerySource Rbsq = new Do_Constants.Str_Request_BuildQuerySource();
        //    Rbsq.ObjectName = SourceObject;
        //    Rbsq.Fields = Fields;
        //    Rbsq.Condition = Condition;
        //    Rbsq.Sort = Sort;

        //    Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
        //    Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

        //    Client_WcfService Client = Client_WcfService.CreateObject();
        //    String ResponseData = Client.BuildQuery_SourceObject(Rs, Rbsq);

        //    return ResponseData;
        //}

        public DataTable GetQuery(Interface_Connection Connection, string SourceObject, string Fields = "", string Condition = "", string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_GetQuery Rgq = new Do_Constants.Str_Request_GetQuery();
            Rgq.ObjectName = SourceObject;
            Rgq.Fields = Fields;
            Rgq.Condition_String = Condition;
            Rgq.Sort = Sort;
            Rgq.Top = Top;
            Rgq.Page = Page;

            Rgq.ConnectionString = (Connection as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetQuery(Rs, Rgq);

            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataTable GetQuery(string SourceObject, string Fields = "", string Condition = "", string Sort = "", long Top = 0, int Page = 0)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort);
        }

        public DataTable GetQuery(Interface_Connection Connection, string SourceObject, string Fields, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_GetQuery Rgq = new Do_Constants.Str_Request_GetQuery();
            Rgq.ObjectName = SourceObject;
            Rgq.Fields = Fields;
            Rgq.Condition = Condition;
            Rgq.Sort = Sort;
            Rgq.Top = Do_Methods.Convert_Int32(Top);
            Rgq.Page = Page;

            Rgq.ConnectionString = (Connection as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetQuery(Rs, Rgq);
            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);

            return Sdt.ToDataTable();
        }

        public DataTable GetQuery(string SourceObject, string Fields, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort);
        }

        public DataTable GetQuery(Interface_Connection Connection, Do_Constants.Str_QuerySource SourceObject, string Fields, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_GetQuery Rgq = new Do_Constants.Str_Request_GetQuery();
            Rgq.ObjectQuerySource = SourceObject;
            Rgq.Fields = Fields;
            Rgq.Condition = Condition;
            Rgq.Sort = Sort;
            Rgq.Top = Do_Methods.Convert_Int32(Top);
            Rgq.Page = Page;

            Rgq.ConnectionString = (Connection as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetQuery(Rs, Rgq);
            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);

            return Sdt.ToDataTable();
        }

        public DataTable GetQuery(Do_Constants.Str_QuerySource SourceObject, string Fields, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.GetQuery(Cn, SourceObject, Fields, Condition, Sort);
        }

        public int ExecuteNonQuery(Interface_Connection Connection, string ProcedureName, List<QueryParameter> ProcedureParameters)
        {
            Do_Constants.Str_Request_Execute Rqe = new Do_Constants.Str_Request_Execute();
            Rqe.ConnectionString = (Connection as Connection_Wcf).pConnectionString;
            Rqe.ProcedureName = ProcedureName;
            Rqe.ProcedureParameters = ProcedureParameters;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.ExecuteNonQuery(Rs, Rqe);
            Int32 Result = (Int32)Do_Methods.DeserializeObject_Json(typeof(Int32), ResponseData);
            return Result;
        }

        public int ExecuteNonQuery(string ProcedureName, List<QueryParameter> ProcedureParameters)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.ExecuteNonQuery(Cn, ProcedureName, ProcedureParameters);
        }

        public int ExecuteNonQuery(Interface_Connection Connection, string Query)
        {
            Do_Constants.Str_Request_Execute Rqe = new Do_Constants.Str_Request_Execute();
            Rqe.ConnectionString = (Connection as Connection_Wcf).pConnectionString;
            Rqe.Query = Query;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.ExecuteNonQuery(Rs, Rqe);
            Int32 Result = (Int32)Do_Methods.DeserializeObject_Json(typeof(Int32), ResponseData);
            return Result;
        }

        public int ExecuteNonQuery(string Query)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.ExecuteNonQuery(Cn, Query);
        }

        public int ExecuteNonQuery(Interface_Connection Cn, DbCommand Cmd)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(DbCommand Cmd)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(Interface_Connection Connection, string ProcedureName, List<QueryParameter> ProcedureParameters)
        {
            Do_Constants.Str_Request_Execute Rqe = new Do_Constants.Str_Request_Execute();
            Rqe.ConnectionString = (Connection as Connection_Wcf).pConnectionString;
            Rqe.ProcedureName = ProcedureName;
            Rqe.ProcedureParameters = ProcedureParameters;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.ExecuteQuery(Rs, Rqe);
            SimpleDataSet Sds = SimpleDataSet.Deserialize(ResponseData);

            return Sds.ToDataSet();
        }

        public DataSet ExecuteQuery(string ProcedureName, List<QueryParameter> ProcedureParameters)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.ExecuteQuery(ProcedureName, ProcedureParameters);
        }

        public DataSet ExecuteQuery(Interface_Connection Connection, string Query)
        {
            Do_Constants.Str_Request_Execute Rqe = new Do_Constants.Str_Request_Execute();
            Rqe.ConnectionString = (Connection as Connection_Wcf).pConnectionString;
            Rqe.Query = Query;
            
            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.ExecuteQuery(Rs, Rqe);
            SimpleDataSet Sds = SimpleDataSet.Deserialize(ResponseData);

            return Sds.ToDataSet();
        }

        public DataSet ExecuteQuery(string Query)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.ExecuteQuery(Cn, Query);
        }

        public DataSet ExecuteQuery(Interface_Connection Cn, DbCommand Cmd)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(DbCommand Cmd)
        {
            throw new NotImplementedException();
        }

        public Interface_Connection Connection
        {
            get { return this.mConnection; }
        }

        public void Connect()
        {
            this.mConnection = new Connection_Wcf();
            this.mConnection.Connect();
        }

        public void Connect(string ConnectionString)
        {
            this.mConnection = new Connection_Wcf();
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
            Connection_Wcf Cn = this.GetConnection();

            Do_Constants.Str_Request_Save Rs = new Do_Constants.Str_Request_Save();
            Rs.TableName = TableName;
            Rs.SchemaName = SchemaName;
            Rs.IsDelete = IsDelete;
            Rs.CustomKeys = CustomKeys;
            Rs.ConnectionString = Cn.pConnectionString;

            SimpleDataRow Sdr = new SimpleDataRow(ObjDataRow);
            Rs.Serialized_ObjectDataRow = Sdr.Serialize();

            Do_Constants.Str_Request_Session Rss = new Do_Constants.Str_Request_Session();
            Rss.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.SaveDataRow(Rss, Rs);

            Do_Constants.Str_Response_Save Rps = Do_Methods.DeserializeObject_Json<Do_Constants.Str_Response_Save>(ResponseData);
            Boolean SaveResult = Rps.SaveResult;
            DataRow Dr_Response = Rps.Sdr.ToDataRow();

            foreach (DataColumn Dc in ObjDataRow.Table.Columns)
            { ObjDataRow[Dc.ColumnName] = Dr_Response[Dc.ColumnName]; }
            
            return SaveResult;            
        }

        public DataTable List(string ObjectName, string Condition = "", string Sort = "")
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.List(Cn, ObjectName, Condition, Sort);
        }

        public DataTable List(Interface_Connection Cn, string ObjectName, string Condition = "", string Sort = "")
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition_String = Condition;
            Rl.Sort = Sort;
            Rl.ConnectionString = (Cn as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List(Rs, Rl);

            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataTable List(string ObjectName, QueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            //if (this.mConnection == null)
            //{
            //    Connection_Wcf Cn = new Connection_Wcf();
            //    try
            //    {
            //        Cn.Connect();
            //        return this.List(Cn, ObjectName, Condition, Sort, Top, Page);
            //    }
            //    catch (Exception Ex)
            //    { throw Ex; }
            //}
            //else
            //{ return this.List(this.mConnection, ObjectName, Condition, Sort, Top, Page); }

            //[-]

            Connection_Wcf Cn = this.GetConnection();
            return this.List(Cn, ObjectName, Condition, Sort, Top, Page);
        }

        public DataTable List(Interface_Connection Cn, string ObjectName, QueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.Sort = Sort;
            Rl.Top = Top;
            Rl.Page = Page;
            Rl.ConnectionString = (Cn as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List(Rs, Rl);

            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public long List_Count(string ObjectName, Objects.QueryCondition Condition = null)
        {
            Connection_Wcf Cn = this.GetConnection();
            return this.List_Count(Cn, ObjectName, Condition);
        }

        public long List_Count(Interface_Connection Cn, string ObjectName, QueryCondition Condition = null)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.ConnectionString = (Cn as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            Int64 ResponseData = Client.List_Count(Rs, Rl);
            return ResponseData;
        }

        public DataTable List_Empty(string ObjectName)
        {
            //if (this.mConnection == null)
            //{
            //    Connection_Wcf Cn = new Connection_Wcf();
            //    try
            //    {
            //        Cn.Connect();
            //        return this.List_Empty(Cn, ObjectName);
            //    }
            //    catch (Exception Ex)
            //    { throw Ex; }
            //}
            //else
            //{ return this.List_Empty(this.mConnection, ObjectName); }

            //[-]

            Connection_Wcf Cn = this.GetConnection();
            return this.List_Empty(Cn, ObjectName);
        }

        public DataTable List_Empty(Interface_Connection Cn, string ObjectName)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.ConnectionString = (Cn as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.List_Empty(Rs, Rl);

            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataRow Load(string ObjectName, List<string> List_Key, Keys Keys)
        {
            Connection_Wcf Cn = this.GetConnection();

            Do_Constants.Str_Request_Load Rl = new Do_Constants.Str_Request_Load();
            Rl.ObjectName = ObjectName;
            Rl.ObjectKeys = List_Key;
            Rl.Key = Keys;
            Rl.ConnectionString = Cn.pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.Load(Rs, Rl);

            SimpleDataRow Sdr = SimpleDataRow.Deserialize(ResponseData);
            return Sdr.ToDataRow();
        }

        public DataTable Load_TableDetails(string ObjectName, Keys Keys, string Condition, List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            Connection_Wcf Cn = null;
            if (this.mConnection == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }
            else
            { Cn = this.mConnection; }

            Do_Constants.Str_Request_Load Rl = new Do_Constants.Str_Request_Load();
            Rl.ObjectName = ObjectName;
            Rl.ForeignKeys = ForeignKeys;
            Rl.Condition = Condition;
            Rl.Key = Keys;
            Rl.ConnectionString = Cn.pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.Load_TableDetails(Rs, Rl);

            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataRow Load_RowDetails(string ObjectName, Keys Keys, string Condition, List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys)
        {
            Connection_Wcf Cn = null;
            if (this.mConnection == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }
            else
            { Cn = this.mConnection; }

            Do_Constants.Str_Request_Load Rl = new Do_Constants.Str_Request_Load();
            Rl.ObjectName = ObjectName;
            Rl.ForeignKeys = ForeignKeys;
            Rl.Condition = Condition;
            Rl.Key = Keys;
            Rl.ConnectionString = Cn.pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.Load_RowDetails(Rs, Rl);

            SimpleDataRow Sdr = SimpleDataRow.Deserialize(ResponseData);
            return Sdr.ToDataRow();
        }

        public Interface_Connection CreateConnection()
        {
            return new Connection_Wcf();
        }

        public QueryCondition CreateQueryCondition()
        {
            return new QueryCondition();
        }

        public PreparedQuery CreatePreparedQuery(Interface_Connection Cn, string Query = "", List<QueryParameter> Parameters = null)
        {
            return new PreparedQuery_Wcf(Cn, Query, Parameters);
        }

        public PreparedQuery CreatePreparedQuery(string Query = "", List<QueryParameter> Parameters = null)
        {
            Connection_Wcf Cn = this.mConnection;
            if (Cn == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }

            return new PreparedQuery_Wcf(Cn, Query, Parameters);
        }

        public DataTable GetTableDef(string TableName)
        {
            Connection_Wcf Cn = null;
            if (this.mConnection == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }
            else
            { Cn = this.mConnection; }

            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = TableName;
            Rl.ConnectionString = Cn.pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            string ResponseData = Client.GetTableDef(Rs, Rl);

            SimpleDataTable Sdt = SimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public string GetSystemParameter(string ParameterName, string DefaultValue = "")
        {
            //Connection_Wcf Cn = new Connection_Wcf();

            //try
            //{
            //    Cn.Connect();
            //    return this.GetSystemParameter(Cn, ParameterName, DefaultValue);
            //}
            //catch (Exception ex)
            //{ throw ex; }

            //[-]

            Connection_Wcf Cn = null;
            if (this.mConnection == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }
            else
            { Cn = this.mConnection; }

            return this.GetSystemParameter(Cn, ParameterName, DefaultValue);
        }

        public string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "")
        {
            Do_Constants.Str_Request_SystemParameter Rsp = new Do_Constants.Str_Request_SystemParameter();
            Rsp.ParameterName = ParameterName;
            Rsp.ParameterValue = DefaultValue;
            Rsp.ConnectionString = (Connection as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.GetSystemParameter(Rs, Rsp);

            return ResponseData;
        }

        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            Connection_Wcf Cn = null;
            if (this.mConnection == null)
            {
                Cn = new Connection_Wcf();
                Cn.Connect();
            }
            else
            { Cn = this.mConnection; }

            this.SetSystemParameter(Cn, ParameterName, ParameterValue);
        }

        public void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue)
        {
            Do_Constants.Str_Request_SystemParameter Rsp = new Do_Constants.Str_Request_SystemParameter();
            Rsp.ParameterName = ParameterName;
            Rsp.ParameterValue = ParameterValue;
            Rsp.ConnectionString = (Connection as Connection_Wcf).pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            Client.SetSystemParameter(Rs, Rsp);
        }

        public void Dispose()
        {
            if (this.mConnection != null)
            { this.mConnection = null; }
        }

        public void InvokeError()
        {
            Client_WcfService Client = Client_WcfService.CreateObject();
            Client.InvokeError();
        }

        #endregion
    }

    public sealed class ClsDataAccess_Wcf_Session
    {
        #region _Constructor

        static ClsDataAccess_Wcf_Session()
        {
            Client_WcfService Client = Client_WcfService.CreateObject();
            Instance.pSessionID = Client.NewSession();
        }

        static readonly ClsDataAccess_Wcf_Session mInstance = new ClsDataAccess_Wcf_Session();

        public static ClsDataAccess_Wcf_Session Instance
        {
            get { return mInstance; }
        }

        #endregion

        #region _Properties

        public String pSessionID { get; set; }

        #endregion
    }
}
