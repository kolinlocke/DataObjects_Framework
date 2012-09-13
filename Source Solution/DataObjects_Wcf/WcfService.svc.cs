using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Objects;
using DataObjects_Framework.PreparedQuery;
using DataObjects_Wcf;
using Microsoft.VisualBasic;

namespace DataObjects_Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfService : Interface_WcfService, IDisposable
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
            public ClsPreparedQuery Obj;
            public DateTime SessionDate;
        }

        #endregion

        #region _Constructor

        public WcfService()
        {
            Do_Globals.gSettings.pDataAccessType = Do_Constants.eDataAccessType.DataAccess_SqlServer;
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
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = null;

                if (Do_Methods.Convert_String(Request_List.Condition_String) == "")
                {
                    Dt = Da.List(
                    Request_List.ObjectName
                    , Request_List.Condition_String
                    , Request_List.Sort);
                    Rv = new ClsSimpleDataTable(Dt);
                }
                else
                {
                    Dt = Da.List(
                    Request_List.ObjectName
                    , Request_List.Condition
                    , Request_List.Sort
                    , Request_List.Top
                    , Request_List.Page);
                    Rv = new ClsSimpleDataTable(Dt);
                }
            }
            catch (Exception Ex)
            { throw Ex; }
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
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv;
        }

        public String List_Empty(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
        {
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = Da.List_Empty(
                Request_List.ObjectName);
                Rv = new ClsSimpleDataTable(Dt);
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
        {
            ClsSimpleDataRow Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                DataRow Dr = Da.Load(Request_Load.ObjectName, Request_Load.ObjectKeys, Request_Load.Key);
                ClsSimpleDataTable Sds = new ClsSimpleDataTable(Dr.Table.Clone());
                Rv = Sds.NewRow(Dr);
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load_TableDetails(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
        {
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                Rv = new ClsSimpleDataTable(Da.Load_TableDetails(Request_Load.ObjectName, Request_Load.Key, Request_Load.Condition, Request_Load.ForeignKeys));
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public String Load_RowDetails(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Load Request_Load)
        {
            ClsSimpleDataRow Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Load.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Load.ConnectionString); }

                DataRow Dr = Da.Load_RowDetails(Request_Load.ObjectName, Request_Load.Key, Request_Load.Condition, Request_Load.ForeignKeys);
                ClsSimpleDataTable Sdt = new ClsSimpleDataTable(Dr.Table.Clone());
                Rv = Sdt.NewRow(Dr);
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public bool SaveDataRow(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Save Request_Save)
        {
            Boolean Rv = false;
            ClsSimpleDataRow Sdr_Obj = ClsSimpleDataRow.Deserialize(Request_Save.Serialized_ObjectDataRow);
            DataRow Dr_Obj = Sdr_Obj.ToDataRow();

            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Save.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Save.ConnectionString); }

                Rv = Da.SaveDataRow(Dr_Obj, Request_Save.TableName, Request_Save.SchemaName, Request_Save.IsDelete, Request_Save.CustomKeys);
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv;
        }

        public string GetQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_GetQuery Request_GetQuery)
        {
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_GetQuery.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_GetQuery.ConnectionString); }

                DataTable Dt = null;

                if (Do_Methods.Convert_String(Request_GetQuery.Condition_String) == "")
                {
                    Dt = Da.GetQuery(
                    Request_GetQuery.ObjectName
                    , Request_GetQuery.Fields
                    , Request_GetQuery.Condition_String
                    , Request_GetQuery.Sort);
                    Rv = new ClsSimpleDataTable(Dt);
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
                    Rv = new ClsSimpleDataTable(Dt);
                }
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }

            return Rv.Serialize();
        }

        public void ExecuteNonQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Execute Request_Execute)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Execute.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Execute.ConnectionString); }

                if (Do_Methods.Convert_String(Request_Execute.Query) == "")
                { Da.ExecuteNonQuery(Request_Execute.Query); }
                else
                { Da.ExecuteNonQuery(Request_Execute.ProcedureName, Request_Execute.ProcedureParameters); }
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }
        }

        public string ExecuteQuery(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_Execute Request_Execute)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_Execute.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_Execute.ConnectionString); }

                if (Do_Methods.Convert_String(Request_Execute.Query) == "")
                {
                    DataSet Ds = Da.ExecuteQuery(Request_Execute.Query);
                    ClsSimpleDataSet Sds = new ClsSimpleDataSet(Ds);
                    return Sds.Serialize();
                }
                else
                {
                    DataSet Ds = Da.ExecuteQuery(Request_Execute.ProcedureName, Request_Execute.ProcedureParameters);
                    ClsSimpleDataSet Sds = new ClsSimpleDataSet(Ds);
                    return Sds.Serialize();
                }
            }
            catch (Exception Ex)
            { throw Ex; }
            finally
            { Da.Close(); }
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

                ClsPreparedQuery Obj_Pq = Da.CreatePreparedQuery(Request_Command.Query, Request_Command.Parameters);
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
            { throw Ex; }

            return PreparedQuerySessionID;
        }

        public void PreparedQuery_ExecuteNonQuery(Do_Constants.Str_Request_Session Request_Session, string PreparedQuerySessionID, Do_Constants.Str_Request_PreparedQuery_Parameters Request_Parameters)
        {
            Str_PreparedQuerySession? Obj_Pqs = this.mPreparedQuerySessions.FirstOrDefault(X => X.Value.PreparedQuerySessionID == PreparedQuerySessionID && X.Value.SessionID == Request_Session.SessionID);
            if (Obj_Pqs != null)
            {
                Str_PreparedQuerySession Inner_Obj_Pqs = Obj_Pqs.Value;
                Inner_Obj_Pqs.SessionDate = DateTime.Now;

                ClsPreparedQuery Obj_Pq = Obj_Pqs.Value.Obj;
                if (Request_Parameters.Parameters != null)
                { 
                    foreach (ClsParameter Sp in Request_Parameters.Parameters)
                    { Obj_Pq.pParameter_Set(Sp.Name, Sp.Value); }
                }

                Obj_Pq.ExecuteNonQuery();
            }
        }

        public string PreparedQuery_ExecuteQuery(Do_Constants.Str_Request_Session Request_Session, string PreparedQuerySessionID, Do_Constants.Str_Request_PreparedQuery_Parameters Request_Parameters)
        {
            String Rv = "";
            Str_PreparedQuerySession? Obj_Pqs = this.mPreparedQuerySessions.FirstOrDefault(X => X.Value.PreparedQuerySessionID == PreparedQuerySessionID && X.Value.SessionID == Request_Session.SessionID);
            if (Obj_Pqs != null)
            {
                Str_PreparedQuerySession Inner_Obj_Pqs = Obj_Pqs.Value;
                Inner_Obj_Pqs.SessionDate = DateTime.Now;

                ClsPreparedQuery Obj_Pq = Obj_Pqs.Value.Obj;
                if (Request_Parameters.Parameters != null)
                { 
                    foreach (ClsParameter Sp in Request_Parameters.Parameters)
                    { Obj_Pq.pParameter_Set(Sp.Name, Sp.Value); }
                }

                DataSet Ds = Obj_Pq.ExecuteQuery();
                ClsSimpleDataSet Sds = new ClsSimpleDataSet(Ds);
                Rv = Sds.Serialize();
            }

            return Rv;
        }

        public string GetTableDef(Do_Constants.Str_Request_Session Request_Session, Do_Constants.Str_Request_List Request_List)
        {
            ClsSimpleDataTable Rv = null;
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                if (Do_Methods.Convert_String(Request_List.ConnectionString) == "")
                { Da.Connect(); }
                else
                { Da.Connect(Request_List.ConnectionString); }

                DataTable Dt = Da.GetTableDef(Request_List.ObjectName);
                Rv = new ClsSimpleDataTable(Dt);
            }
            catch (Exception Ex)
            { throw Ex; }
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
            { throw Ex; }
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
            { throw Ex; }
            finally
            { Da.Close(); }
        }

        #endregion

        #region _Internal

        void PreparedQuerySessionChecker_Start()
        {
            ClsThreadStarter Ts = new ClsThreadStarter();
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

        #endregion

        #endregion
    }
}
