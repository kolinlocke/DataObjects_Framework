using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Objects;
using DataObjects_Framework.PreparedQueryObjects;
using DataObjects_Wcf;
using Microsoft.VisualBasic;

namespace DataObjects_Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfService : Interface_WcfService, IDisposable, IServiceBehavior, IErrorHandler
    {
        #region _Variables

        List<String> mSessionIDs = new List<String>();
        List<Str_PreparedQuerySession?> mPreparedQuerySessions = new List<Str_PreparedQuerySession?>();

        Thread mThread_PreparedQuerySessionChecker;
        Boolean mThread_PreparedQuerySessionChecker_IsExit = false;
        const Int32 Cns_PreparedQuerySessionTimeout = 30;

        delegate void Ds_Generic();

        struct Str_PreparedQuerySession
        {
            public String SessionID;
            public String PreparedQuerySessionID;
            public PreparedQuery Obj;
            public DateTime SessionDate;
        }

        #endregion

        #region _Constructor

        public WcfService()
        {
            Do_Globals.gSettings.pDataAccessType = Do_Constants.eDataAccessType.DataAccess_SqlServer;
            Do_Globals.gSettings.pConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            this.PreparedQuerySessionChecker_Start();
        }

        #endregion

        #region _Methods

        #region _WcfRequests

        public string NewSession()
        {
            String SessionID = String.Empty;
            Boolean IsValid = false;
            while (!IsValid)
            {
                SessionID = Guid.NewGuid().ToString();
                if (!this.mSessionIDs.Exists(X => X == SessionID))
                {
                    this.mSessionIDs.Add(SessionID);
                    IsValid = true;
                }
            }

            return SessionID;
        }

        public String List(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
        {
            SimpleDataTable Rv = new SimpleDataTable();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = null;

                if (Do_Methods.Convert_String(Request_List.Condition_String) != "")
                {
                    Dt = Da.List(
                    Request_List.ObjectName
                    , Request_List.Condition_String
                    , Request_List.Sort);
                    Rv = new SimpleDataTable(Dt);
                }
                else
                {
                    Dt = Da.List(
                    Request_List.ObjectName
                    , Request_List.Condition
                    , Request_List.Sort
                    , Request_List.Top
                    , Request_List.Page);
                    Rv = new SimpleDataTable(Dt);
                }
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: List"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public long List_Count(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
        {
            Int64 Rv = 0;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                Rv = Da.List_Count(Request_List.ObjectName, Request_List.Condition);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: List_Count"); }
            finally
            { Da.Close(); }

            return Rv;
        }

        public String List_Empty(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
        {
            SimpleDataTable Rv = new SimpleDataTable();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = Da.List_Empty(Request_List.ObjectName);
                Rv = new SimpleDataTable(Dt);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: List_Empty"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
        {
            SimpleDataRow Rv = new SimpleDataRow();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                DataRow Dr = Da.Load(Request_Load.ObjectName, Request_Load.ObjectKeys, Request_Load.Key);
                SimpleDataTable Sds = new SimpleDataTable(Dr.Table.Clone());
                Rv = Sds.NewRow(Dr);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: Load"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load_TableDetails(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
        {
            SimpleDataTable Rv = new SimpleDataTable();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                Rv = new SimpleDataTable(Da.Load_TableDetails(Request_Load.ObjectName, Request_Load.Key, Request_Load.Condition, Request_Load.ForeignKeys));
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: Load_TableDetails"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load_RowDetails(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
        {
            SimpleDataRow Rv = new SimpleDataRow();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                DataRow Dr = Da.Load_RowDetails(Request_Load.ObjectName, Request_Load.Key, Request_Load.Condition, Request_Load.ForeignKeys);
                SimpleDataTable Sdt = new SimpleDataTable(Dr.Table.Clone());
                Rv = Sdt.NewRow(Dr);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: Load_RowDetails"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String SaveDataRow(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Save Request_Save)
        {
            Do_Constants.Str_Response_Save Rv = new Do_Constants.Str_Response_Save();
            SimpleDataRow Sdr_Obj = SimpleDataRow.Deserialize(Request_Save.Serialized_ObjectDataRow);
            DataRow Dr_Obj = Sdr_Obj.ToDataRow();
            Boolean SaveResult = false;

            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Save.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Save.ConnectionString); }

                SaveResult =
                    Da.SaveDataRow(
                        Dr_Obj
                        , Request_Save.TableName
                        , Request_Save.SchemaName
                        , Request_Save.IsDelete
                        , Request_Save.CustomKeys);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: SaveDataRow"); }
            finally
            { Da.Close(); }

            Rv.SaveResult = SaveResult;
            Rv.Sdr = new SimpleDataRow(Dr_Obj);

            return Do_Methods.SerializeObject_Json(Rv);
        }

        public String GetQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_GetQuery Request_GetQuery)
        {
            SimpleDataTable Rv = new SimpleDataTable();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_GetQuery.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_GetQuery.ConnectionString); }

                DataTable Dt = null;

                if (Do_Methods.Convert_String(Request_GetQuery.Condition_String) != "")
                {
                    Dt = Da.GetQuery(
                    Request_GetQuery.ObjectName
                    , Request_GetQuery.Fields
                    , Request_GetQuery.Condition_String
                    , Request_GetQuery.Sort);
                    Rv = new SimpleDataTable(Dt);
                }
                else
                {
                    Dt = Da.GetQuery(
                    Request_GetQuery.ObjectName
                    , Request_GetQuery.Fields
                    , Request_GetQuery.Condition
                    , Request_GetQuery.Sort
                    , Request_GetQuery.Top
                    , Request_GetQuery.Page);
                    Rv = new SimpleDataTable(Dt);
                }
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: GetQuery"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String ExecuteNonQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Execute Request_Execute)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            Int32 Result = 0;

            try
            {
                if (Do_Methods.Convert_String(Request_Execute.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Execute.ConnectionString); }

                if (Do_Methods.Convert_String(Request_Execute.Query) != "")
                { Result = Da.ExecuteNonQuery(Request_Execute.Query); }
                else
                { Result = Da.ExecuteNonQuery(Request_Execute.ProcedureName, Request_Execute.ProcedureParameters); }
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: ExecuteNonQuery"); }
            finally
            { Da.Close(); }

            return Do_Methods.SerializeObject_Json(typeof(Int32), Result);
        }

        public string ExecuteQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Execute Request_Execute)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            String Rv = "";
            try
            {
                if (Do_Methods.Convert_String(Request_Execute.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Execute.ConnectionString); }

                if (Do_Methods.Convert_String(Request_Execute.Query) != "")
                {
                    DataSet Ds = Da.ExecuteQuery(Request_Execute.Query);
                    SimpleDataSet Sds = new SimpleDataSet(Ds);
                    Rv = Sds.Serialize();
                }
                else
                {
                    DataSet Ds = Da.ExecuteQuery(Request_Execute.ProcedureName, Request_Execute.ProcedureParameters);
                    SimpleDataSet Sds = new SimpleDataSet(Ds);
                    Rv = Sds.Serialize();
                }
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: ExecuteQuery"); }
            finally
            { Da.Close(); }

            return Rv;
        }

        public string PreparedQuery_Prepare(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_PreparedQuery_Prepare Request_Command)
        {
            String PreparedQuerySessionID = "";
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Command.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Command.ConnectionString); }

                PreparedQuery Obj_Pq = Da.CreatePreparedQuery(Request_Command.Query, Request_Command.Parameters);
                PreparedQuerySessionID = Do_Methods.GenerateGuid(
                    (
                    from O in this.mPreparedQuerySessions
                    where O.Value.SessionID == Request_Session.SessionID
                    select O.Value.PreparedQuerySessionID).ToList()
                    );

                Str_PreparedQuerySession Obj_Pqs = new Str_PreparedQuerySession();
                Obj_Pqs.SessionID = Request_Session.SessionID;
                Obj_Pqs.PreparedQuerySessionID = PreparedQuerySessionID;
                Obj_Pqs.Obj = Obj_Pq;
                Obj_Pqs.SessionDate = DateTime.Now;

                this.mPreparedQuerySessions.Add(Obj_Pqs);

                Obj_Pq.Prepare();
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: PreparedQuery_Prepare"); }

            return PreparedQuerySessionID;
        }

        public void PreparedQuery_ExecuteNonQuery(Do_Constants.Str_Request_Session Request_Session, string PreparedQuerySessionID, Do_Constants.Str_Request_PreparedQuery_Parameters Request_Parameters)
        {
            try
            {
                Str_PreparedQuerySession? Obj_Pqs = this.mPreparedQuerySessions.FirstOrDefault(X => X.Value.PreparedQuerySessionID == PreparedQuerySessionID && X.Value.SessionID == Request_Session.SessionID);
                if (Obj_Pqs != null)
                {
                    Str_PreparedQuerySession Inner_Obj_Pqs = Obj_Pqs.Value;
                    Inner_Obj_Pqs.SessionDate = DateTime.Now;

                    PreparedQuery Obj_Pq = Obj_Pqs.Value.Obj;
                    if (Request_Parameters.Parameters != null)
                    {
                        foreach (QueryParameter Sp in Request_Parameters.Parameters)
                        { Obj_Pq.pParameter_Set(Sp.Name, Sp.Value); }
                    }

                    Obj_Pq.ExecuteNonQuery();
                }
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: PreparedQuery_ExecuteNonQuery"); }
        }

        public string PreparedQuery_ExecuteQuery(Do_Constants.Str_Request_Session Request_Session, string PreparedQuerySessionID, Do_Constants.Str_Request_PreparedQuery_Parameters Request_Parameters)
        {
            SimpleDataSet Rv = new SimpleDataSet();

            try
            {
                Str_PreparedQuerySession? Obj_Pqs = this.mPreparedQuerySessions.FirstOrDefault(X => X.Value.PreparedQuerySessionID == PreparedQuerySessionID && X.Value.SessionID == Request_Session.SessionID);
                if (Obj_Pqs != null)
                {
                    Str_PreparedQuerySession Inner_Obj_Pqs = Obj_Pqs.Value;
                    Inner_Obj_Pqs.SessionDate = DateTime.Now;

                    PreparedQuery Obj_Pq = Obj_Pqs.Value.Obj;
                    if (Request_Parameters.Parameters != null)
                    {
                        foreach (QueryParameter Sp in Request_Parameters.Parameters)
                        { Obj_Pq.pParameter_Set(Sp.Name, Sp.Value); }
                    }

                    DataSet Ds = Obj_Pq.ExecuteQuery();
                    SimpleDataSet Sds = new SimpleDataSet(Ds);
                    Rv = Sds;
                }
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: PreparedQuery_ExecuteQuery"); }

            return Rv.Serialize();
        }

        public string GetTableDef(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
        {
            SimpleDataTable Rv = new SimpleDataTable();
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = Da.GetTableDef(Request_List.ObjectName);
                Rv = new SimpleDataTable(Dt);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: GetTableDef"); }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public string GetSystemParameter(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_SystemParameter Request_SystemParameter)
        {
            String Rv = "";
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_SystemParameter.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_SystemParameter.ConnectionString); }

                Rv = Da.GetSystemParameter(Request_SystemParameter.ParameterName, Request_SystemParameter.ParameterValue);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: GetSystemParameter"); }
            finally
            { Da.Close(); }

            return Rv;
        }

        public void SetSystemParameter(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_SystemParameter Request_SystemParameter)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_SystemParameter.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_SystemParameter.ConnectionString); }

                Da.SetSystemParameter(Request_SystemParameter.ParameterName, Request_SystemParameter.ParameterValue);
            }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: SetSystemParameter"); }
            finally
            { Da.Close(); }
        }

        public void InvokeError()
        {
            try
            { throw new CustomException("InvokeError method has been invoked!"); }
            catch (Exception Ex)
            { this.ErrorHandler(Ex, "WcfService.Method: InvokeError"); }
        }

        #endregion

        #region _Internal

        void PreparedQuerySessionChecker_Start()
        {
            ThreadStarter Ts = new ThreadStarter();
            Ds_Generic D = new Ds_Generic(this.PreparedQuerySessionChecker);
            Ts.Setup(D, null, this);
            this.mThread_PreparedQuerySessionChecker = Ts.pThread;
            Ts.Start();
        }

        void PreparedQuerySessionChecker()
        {
            while (!this.mThread_PreparedQuerySessionChecker_IsExit)
            {
                this.mPreparedQuerySessions.RemoveAll(X => DateAndTime.DateAdd(DateInterval.Minute, Cns_PreparedQuerySessionTimeout, X.Value.SessionDate) < DateTime.Now);
                Thread.Sleep(1000);
            }
        }

        public void Dispose()
        {
            this.mThread_PreparedQuerySessionChecker_IsExit = true;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = new WcfService();

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        public bool HandleError(Exception error)
        {
            this.ErrorHandler(error, this.GetType().Name);
            return true;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            FaultException faultException = new FaultException(error.Message);
            MessageFault messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, faultException.Action);
        }

        public void ErrorHandler(Exception Ex, string ModuleName)
        {
            String Msg = @"Error Log: " + ModuleName + ": " + Ex.Message + " : " + Ex.Source + " : " + (Ex.TargetSite != null ? Ex.TargetSite.Name : "");
            this.LogWrite(Msg);
        }

        public void LogWrite(String Msg)
        {
            Msg = @"[" + DateTime.Now.ToString() + @"] " + Msg;
            String FileName = @"System Logs [" + string.Format("{0:yyyy.MM.dd}", DateTime.Now.ToString()) + @"].log";
            Do_Methods.LogWrite(Msg, Do_Methods.SetFolderPath(ConfigurationManager.AppSettings["SystemLogPath"]) + "System Logs");
        }

        #endregion

        #endregion
    }
}
