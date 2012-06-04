using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DataObjects_Framework.Connection
{
    /// <summary>
    /// Wrapper for prepared sql statements, provides various methods for ease of use, (implemented using SQL Server)
    /// </summary>
    public class ClsPreparedQuery
    {
        #region _Variables

        //string mQuery = "";
        SqlCommand mCmd = null;
        ClsConnection_SqlServer mCn = null;
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
        /// <param name="ArrSp">
        /// Array of parameters to be used
        /// </param>
        public ClsPreparedQuery(ClsConnection_SqlServer Cn, string Query, SqlParameter[] ArrSp)
        {
            this.mCn = Cn;
            this.mCmd = new SqlCommand();
            this.mCmd.Connection = (SqlConnection)this.mCn.pConnection;
            this.mCmd.Transaction = (SqlTransaction)this.mCn.pTransaction;
            this.mCmd.CommandType = System.Data.CommandType.Text;
            this.mCmd.CommandText = Query;

            foreach (SqlParameter Sp in ArrSp)
            { this.mCmd.Parameters.Add(Sp); }
        }

        /// <summary>
        /// Constructor for ClsPreparedQuery, uses the ClsConnection_SqlServer object
        /// </summary>
        /// <param name="Cn">
        /// An open ClsConnection_SqlServer object
        /// </param>
        public ClsPreparedQuery(ClsConnection_SqlServer Cn)
        {
            this.mCn = Cn;
            this.mCmd = new SqlCommand();
            this.mCmd.Connection = (SqlConnection)this.mCn.pConnection;
            this.mCmd.Transaction = (SqlTransaction)this.mCn.pTransaction;
            this.mCmd.CommandType = System.Data.CommandType.Text;
        }

        /// <summary>
        /// Constructor for ClsPreparedQuery, uses the ClsConnection_SqlServer object
        /// </summary>
        public ClsPreparedQuery()
        {
            this.mCn = new ClsConnection_SqlServer();
            this.mCn.Connect();
            this.mCmd = new SqlCommand();
            this.mCmd.Connection = (SqlConnection)this.mCn.pConnection;
            this.mCmd.Transaction = (SqlTransaction)this.mCn.pTransaction;
            this.mCmd.CommandType = System.Data.CommandType.Text;
            //this.IsConnection = true;
        }

        /// <summary>
        /// Deconstructor for ClsPreparedQuery
        /// </summary>
        ~ClsPreparedQuery()
        {            
            //if (this.IsConnection)
            //{ this.mCn.Close(); }
        }

        #endregion

        #region _Methods

        /// <summary>
        /// Adds parameters from an array of parameters
        /// </summary>
        /// <param name="Arr_Sp"></param>
        public void Add_Parameter(SqlParameter[] Arr_Sp)
        {
            foreach (SqlParameter Sp in Arr_Sp)
            { this.mCmd.Parameters.Add(Sp); }
        }

        /// <summary>
        /// Adds a new parameter
        /// </summary>
        /// <param name="Sp"></param>
        public void Add_Parameter(SqlParameter Sp)
        { this.mCmd.Parameters.Add(Sp); }

        /// <summary>
        /// Adds a new parameter
        /// </summary>
        /// <param name="Name">
        /// The parameter name
        /// </param>
        /// <param name="DbType">
        /// The datatype of the parameter
        /// </param>
        /// <param name="Size">
        /// The size of the parameter, only needed in text datatypes such as varchar, nchar, etc.
        /// </param>
        /// <param name="Precision">
        /// The precision value of the parameter, only needed in Numeric and Decimal datatypes
        /// </param>
        /// <param name="Scale">
        /// The scale value of the parameter, only needed in Numeric and Decimal datatypes
        /// </param>
        /// <param name="Value">
        /// The parameter value
        /// </param>
        public void Add_Parameter(
            string Name
            , SqlDbType DbType
            , Int32 Size = 0
            , byte Precision = 0
            , byte Scale = 0
            , Object Value = null)
        {
            SqlParameter Sp = new SqlParameter(Name, DbType, Size);
            Sp.Scale = Scale;
            Sp.Precision = Precision;
            Sp.Value = Value;
            this.mCmd.Parameters.Add(Sp);
        }

        /// <summary>
        /// Creates a prepared version of the command on an instance of SQL Server.
        /// </summary>
        public void Prepare()
        { this.mCmd.Prepare(); }

        /// <summary>
        /// Executes the query with the supplied parameters and returns the resulting data set.
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteQuery()
        {
            DataSet Ds = new DataSet();
            SqlDataAdapter Adp = new SqlDataAdapter();
            Adp.SelectCommand = this.mCmd;
            Adp.Fill(Ds);
            return Ds;
        }

        /// <summary>
        /// Executes the query with the supplied parameters without returning the result set.
        /// </summary>
        public void ExecuteNonQuery()
        { this.mCmd.ExecuteNonQuery(); }

        #endregion

        #region _Properties

        /// <summary>
        /// Gets the connection object in this instance.
        /// </summary>
        public ClsConnection_SqlServer pDa 
        {
            get { return this.mCn; }
        }

        /// <summary>
        /// Gets the sql command object in this instance.
        /// </summary>
        public SqlCommand pCmd
        {
            get { return this.mCmd; }
        }

        /// <summary>
        /// Gets the parameter collection in this instance.
        /// </summary>
        public SqlParameterCollection pParameters 
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
