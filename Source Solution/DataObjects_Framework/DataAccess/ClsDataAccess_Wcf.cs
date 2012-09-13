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
using DataObjects_Framework.PreparedQuery;

namespace DataObjects_Framework.DataAccess
{
	/// <summary>
	/// The WCF implementation of Interface_DataAccess
	/// Connects to a WCF Server App provided for DataObjects_Framework
	/// Requires explicit setting to Do_Globals.gSettings.pWcfAddress
	/// </summary>
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

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			string ResponseData = Client.GetQuery(Rs, Rgq);

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

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			string ResponseData = Client.GetQuery(Rs, Rgq);
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

        public int ExecuteNonQuery(Interface_Connection Connection, string ProcedureName, List<ClsParameter> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string ProcedureName, List<ClsParameter> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(Interface_Connection Connection, string Query)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(string Query)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(Interface_Connection Cn, DbCommand Cmd)
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery(DbCommand Cmd)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(Interface_Connection Connection, string ProcedureName, List<ClsParameter> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(string ProcedureName, List<ClsParameter> ProcedureParameters)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(Interface_Connection Connection, string Query)
        {
            throw new NotImplementedException();
        }

        public DataSet ExecuteQuery(string Query)
        {
            throw new NotImplementedException();
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

			Do_Constants.Str_Request_Session Rss = new Do_Constants.Str_Request_Session();
			Rss.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			Boolean ResponseData = Client.SaveDataRow(Rss, Rs);

            return ResponseData;
        }

        public DataTable List(string ObjectName, string Condition = "", string Sort = "")
        {
            //Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            //Rl.ObjectName = ObjectName;
            //Rl.Condition_String = Condition;
            //Rl.Sort = Sort;
            //Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            //Client_WcfService Client = Client_WcfService.CreateObject();
            //string ResponseData = Client.List(Rl);

            //ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            //return Sdt.ToDataTable();

            if (this.mConnection == null)
            {
                ClsConnection_Wcf Cn = new ClsConnection_Wcf();
                try
                {
                    Cn.Connect();
                    return this.List(Cn, ObjectName, Condition, Sort);
                }
                catch (Exception Ex)
                { throw Ex; }
            }
            else
            { return this.List(this.mConnection, ObjectName, Condition, Sort); }
        }

        public DataTable List(Interface_Connection Cn, string ObjectName, string Condition = "", string Sort = "")
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition_String = Condition;
            Rl.Sort = Sort;
            Rl.ConnectionString = (Cn as ClsConnection_Wcf).pConnectionString;

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			string ResponseData = Client.List(Rs, Rl);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public DataTable List(string ObjectName, ClsQueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            if (this.mConnection == null)
            {
                ClsConnection_Wcf Cn = new ClsConnection_Wcf();
                try
                {
                    Cn.Connect();
                    return this.List(Cn, ObjectName, Condition, Sort, Top, Page);
                }
                catch (Exception Ex)
                { throw Ex; }
            }
            else
            { return this.List(this.mConnection, ObjectName, Condition, Sort, Top, Page); }
        }

        public DataTable List(Interface_Connection Cn, string ObjectName, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.Sort = Sort;
            Rl.Top = Top;
            Rl.Page = Page;
            Rl.ConnectionString = (Cn as ClsConnection_Wcf).pConnectionString;

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			string ResponseData = Client.List(Rs, Rl);

            ClsSimpleDataTable Sdt = ClsSimpleDataTable.Deserialize(ResponseData);
            return Sdt.ToDataTable();
        }

        public long List_Count(string ObjectName, Objects.ClsQueryCondition Condition = null)
        {
            //Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            //Rl.ObjectName = ObjectName;
            //Rl.Condition = Condition;
            //Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

            //Client_WcfService Client = Client_WcfService.CreateObject();
            //Int64 ResponseData = Client.List_Count(Rl);

            //return ResponseData;

            if (this.mConnection == null)
            {
                ClsConnection_Wcf Cn = new ClsConnection_Wcf();
                try
                {
                    Cn.Connect();
                    return this.List_Count(Cn, ObjectName, Condition);
                }
                catch (Exception Ex)
                { throw Ex; }
            }
            else
            { return this.List_Count(this.mConnection, ObjectName, Condition); }
        }

        public long List_Count(Interface_Connection Cn, string ObjectName, ClsQueryCondition Condition = null)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.Condition = Condition;
            Rl.ConnectionString = (Cn as ClsConnection_Wcf).pConnectionString;

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			Int64 ResponseData = Client.List_Count(Rs, Rl);
            return ResponseData;
        }

        public DataTable List_Empty(string ObjectName)
        {
            if (this.mConnection == null)
            {
                ClsConnection_Wcf Cn = new ClsConnection_Wcf();
                try
                {
                    Cn.Connect();
                    return this.List_Empty(Cn, ObjectName);
                }
                catch (Exception Ex)
                { throw Ex; }
            }
            else
            { return this.List_Empty(this.mConnection, ObjectName); }
        }

        public DataTable List_Empty(Interface_Connection Cn, string ObjectName)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = ObjectName;
            Rl.ConnectionString = (Cn as ClsConnection_Wcf).pConnectionString;

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			string ResponseData = Client.List_Empty(Rs, Rl);

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

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			String ResponseData = Client.Load(Rs, Rl);

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

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			String ResponseData = Client.Load_TableDetails(Rs, Rl);

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

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			String ResponseData = Client.Load_RowDetails(Rs, Rl);

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

        public ClsPreparedQuery CreatePreparedQuery(Interface_Connection Cn, string Query = "", List<ClsParameter> Parameters = null)
        {
            return new ClsPreparedQuery_Wcf(Cn, Query, Parameters);
        }

        public ClsPreparedQuery CreatePreparedQuery(string Query = "", List<ClsParameter> Parameters = null)
        {
            ClsConnection_Wcf Cn = this.mConnection;
            if (Cn == null)
            {
                Cn = new ClsConnection_Wcf();
                Cn.Connect();                
            }

            return new ClsPreparedQuery_Wcf(Cn, Query, Parameters);
        }

        public DataTable GetTableDef(string TableName)
        {
            Do_Constants.Str_Request_List Rl = new Do_Constants.Str_Request_List();
            Rl.ObjectName = TableName;
            Rl.ConnectionString = (this.mConnection as ClsConnection_Wcf).pConnectionString;

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			string ResponseData = Client.GetTableDef(Rs, Rl);

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

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			String ResponseData = Client.GetSystemParameter(Rs, Rsp);

            return ResponseData;
        }

        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            Do_Constants.Str_Request_SystemParameter Rsp = new Do_Constants.Str_Request_SystemParameter();
            Rsp.ParameterName = ParameterName;
            Rsp.ParameterValue = ParameterValue;
            Rsp.ConnectionString = (Connection as ClsConnection_Wcf).pConnectionString;

			Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
			Client.SetSystemParameter(Rs, Rsp);
        }

        public void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue)
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

        public void Dispose()
        {
            if (this.mConnection != null)
            { this.mConnection = null; }
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
