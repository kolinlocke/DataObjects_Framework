using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataObjects_Framework;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Common;
using DataObjects_Framework.Objects;

namespace DataObjects_Framework.PreparedQueryObjects
{
    /// <summary>
    /// Wcf implementation of PreparedQuery
    /// </summary>
    public class PreparedQuery_Wcf : PreparedQuery
    {
        #region _Variables

        String mPreparedQuerySessionID = String.Empty;

        #endregion

        #region _Constructors

        public PreparedQuery_Wcf(Interface_Connection Cn, String Query = "", List<QueryParameter> Parameters = null)
        { 
            this.mCn = Cn;
            this.mQuery = Query;
            this.mParameters = Parameters ?? new List<QueryParameter>();
        }

        public PreparedQuery_Wcf(String Query = "", List<QueryParameter> Parameters = null)
        { 
            this.mCn = Do_Methods.CreateDataAccess().CreateConnection();
            this.mQuery = Query;
            this.mParameters = Parameters ?? new List<QueryParameter>();
        }

        #endregion

        #region _Methods

        public override void Prepare()
        {
            Do_Constants.Str_Request_PreparedQuery_Prepare Rpqp = new Do_Constants.Str_Request_PreparedQuery_Prepare();
            Rpqp.Query = this.mQuery;
            Rpqp.Parameters = this.mParameters;
            Rpqp.ConnectionString = this.mCn.pConnectionString;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
			Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.PreparedQuery_Prepare(Rs, Rpqp);

            this.mPreparedQuerySessionID = ResponseData;
        }

        public override DataSet ExecuteQuery()
        {
            Do_Constants.Str_Request_PreparedQuery_Parameters Rpqp = new Do_Constants.Str_Request_PreparedQuery_Parameters();
            Rpqp.Parameters = this.mParameters;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            String ResponseData = Client.PreparedQuery_ExecuteQuery(Rs, this.mPreparedQuerySessionID, Rpqp);

            SimpleDataSet Sds = SimpleDataSet.Deserialize(ResponseData);
            return Sds.ToDataSet();
        }

        public override void ExecuteNonQuery()
        {
            Do_Constants.Str_Request_PreparedQuery_Parameters Rpqp = new Do_Constants.Str_Request_PreparedQuery_Parameters();
            Rpqp.Parameters = this.mParameters;

            Do_Constants.Str_Request_Session Rs = new Do_Constants.Str_Request_Session();
            Rs.SessionID = ClsDataAccess_Wcf_Session.Instance.pSessionID;

            Client_WcfService Client = Client_WcfService.CreateObject();
            Client.PreparedQuery_ExecuteNonQuery(Rs, this.mPreparedQuerySessionID, Rpqp);
        }

        #endregion
    }
}
