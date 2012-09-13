using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DataObjects_Framework;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.Connection
{
    //This class is for reconstruction...

    /// <summary>
    /// Wrapper for prepared sql statements, provides various methods for ease of use, (implemented using SQL Server)
    /// </summary>
    public class ClsPreparedQuery_Ex
    {
        #region _Variables

        //string mQuery = "";
        DbCommand mCmd = null;
        protected Interface_Connection mCn = null;
        protected List<Do_Constants.Str_Parameters> mParameters = new List<Do_Constants.Str_Parameters>();
        //bool IsConnection = false;

        #endregion

        #region _Constructor

        /// <summary>
        /// Constructor for ClsPreparedQuery, uses the ClsConnection_SqlServer object
        /// </summary>
        /// <param name="Cn">
        /// An open ClsConnection_SqlServer object
        /// </param>
        /// <param name="Query">
        /// The query string to be executed
        /// </param>
        /// <param name="Parameters">
        /// </param>
        public ClsPreparedQuery_Ex(Interface_Connection Cn, string Query, List<Do_Constants.Str_Parameters> Parameters)
        {
            this.mCn = Cn;
            this.mCmd = this.mCn.CreateCommand();
            this.mCmd.Connection = this.mCn.pConnection;
            this.mCmd.Transaction = this.mCn.pTransaction;
            this.mCmd.CommandType = CommandType.Text;
            this.mCmd.CommandText = Query;

            this.mParameters = Parameters;

            foreach (Do_Constants.Str_Parameters Sp in this.mParameters)
            {
                DbParameter P = this.mCn.CreateParameter();
                P.ParameterName = Sp.Name;
                P.Value = Sp.Value;
                P.Size = Sp.Size;

                //this.mCmd.Parameters.Add( this.mCn.CreateParameter() {  }   ) 
            }

            //foreach (DbParameter Sp in ArrSp)
            //{ this.mCmd.Parameters.Add(Sp); }
        }

        /// <summary>
        /// Constructor for ClsPreparedQuery, uses the ClsConnection_SqlServer object
        /// </summary>
        /// <param name="Cn">
        /// An open ClsConnection_SqlServer object
        /// </param>
        public ClsPreparedQuery_Ex(Interface_Connection Cn)
        {
            this.mCn = Cn;
			this.mCmd = this.mCn.CreateCommand();
            this.mCmd.Connection = this.mCn.pConnection;
			this.mCmd.Transaction = this.mCn.pTransaction;
			this.mCmd.CommandType = CommandType.Text;
        }

        /// <summary>
        /// Constructor for ClsPreparedQuery, uses the ClsConnection_SqlServer object
        /// </summary>
        public ClsPreparedQuery_Ex()
        {
			this.mCn = Do_Methods.CreateDataAccess().CreateConnection();
            this.mCn.Connect();
			this.mCmd = this.mCn.CreateCommand();
            this.mCmd.Connection = this.mCn.pConnection;
            this.mCmd.Transaction = this.mCn.pTransaction;
            this.mCmd.CommandType = CommandType.Text;
            //this.IsConnection = true;
        }

        /// <summary>
        /// Deconstructor for ClsPreparedQuery
        /// </summary>
        ~ClsPreparedQuery_Ex()
        {            
            //if (this.IsConnection)
            //{ this.mCn.Close(); }
        }

        #endregion

        #region _Methods

        /// <summary>
        /// Adds a new parameter
        /// </summary>
        /// <param name="Sp"></param>
        public void Add_Parameter(DbParameter Sp)
        { this.mCmd.Parameters.Add(Sp); }

        /// <summary>
        /// Adds parameters from an list of parameters
        /// </summary>
        /// <param name="List_Sp"></param>
		public void Add_Parameter(List<DbParameter> List_Sp)
		{ this.mCmd.Parameters.AddRange(List_Sp.ToArray()); }

		/// <summary>
		/// Adds parameters from an array of parameters
		/// </summary>
		/// <param name="Arr_Sp"></param>
		public void Add_Parameter(DbParameter[] Arr_Sp)
		{ this.mCmd.Parameters.AddRange(Arr_Sp); }

        /// <summary>
        /// Creates a prepared version of the command on an instance of SQL Server.
        /// </summary>
        public virtual void Prepare()
        { this.mCmd.Prepare(); }

        /// <summary>
        /// Executes the query with the supplied parameters and returns the resulting data set.
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteQuery()
        {
			Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            return Da.ExecuteQuery(this.mCn, this.mCmd);
        }

        /// <summary>
        /// Executes the query with the supplied parameters without returning the result set.
        /// </summary>
        public void ExecuteNonQuery()
        { 
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            Da.ExecuteNonQuery(this.mCn, this.mCmd);
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Gets the connection object in this instance.
        /// </summary>
        public Interface_Connection pDa 
        {
            get { return this.mCn; }
        }

        /// <summary>
        /// Gets the sql command object in this instance.
        /// </summary>
        public DbCommand pCmd
        {
            get { return this.mCmd; }
        }

        /// <summary>
        /// Gets the parameter collection in this instance.
        /// </summary>
        public DbParameterCollection pParameters 
        {
            get { return this.mCmd.Parameters; }
        }

        /// <summary>
        /// Gets/Sets the query text in this instance.
        /// </summary>
        public string pQuery 
        {
            get { return this.mCmd.CommandText; }
            set { this.mCmd.CommandText = value; }
        }

        #endregion
    }
}
