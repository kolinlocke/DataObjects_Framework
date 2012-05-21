using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Layer01_Common.Connection
{
    public class ClsPreparedQuery
    {

        #region _Variables

        //string mQuery = "";
        SqlCommand mCmd = null;
        ClsConnection_SqlServer mDa = null;
        //bool IsDa = false;

        #endregion

        #region _Constructor

        public ClsPreparedQuery(ClsConnection_SqlServer pDa, string pQuery, SqlParameter[] pArrSp)
        {
            this.mDa = pDa;
            this.mCmd = new SqlCommand();
            this.mCmd.Connection = (SqlConnection)this.mDa.pConnection;
            this.mCmd.Transaction = (SqlTransaction)this.mDa.pTransaction;
            this.mCmd.CommandType = System.Data.CommandType.Text;
            this.mCmd.CommandText = pQuery;

            foreach (SqlParameter Sp in pArrSp)
            {
                this.mCmd.Parameters.Add(Sp);
            }
        }

        public ClsPreparedQuery(ClsConnection_SqlServer pDa)
        {
            this.mDa = pDa;
            this.mCmd = new SqlCommand();
            this.mCmd.Connection = (SqlConnection)this.mDa.pConnection;
            this.mCmd.Transaction = (SqlTransaction)this.mDa.pTransaction;
            this.mCmd.CommandType = System.Data.CommandType.Text;
        }

        public ClsPreparedQuery()
        {
            this.mDa = new ClsConnection_SqlServer();
            this.mDa.Connect();
            this.mCmd = new SqlCommand();
            this.mCmd.Connection = (SqlConnection)this.mDa.pConnection;
            this.mCmd.Transaction = (SqlTransaction)this.mDa.pTransaction;
            this.mCmd.CommandType = System.Data.CommandType.Text;
            //this.IsDa = true;
        }

        ~ClsPreparedQuery()
        {
            /*
            if (this.IsDa)
            { this.mDa.Close(); }
            */
        }

        #endregion

        #region _Methods

        public void Add_Parameter(SqlParameter[] Arr_Sp)
        {
            foreach (SqlParameter Sp in Arr_Sp)
            { this.mCmd.Parameters.Add(Sp); }
        }

        public void Add_Parameter(SqlParameter Sp)
        {
            this.mCmd.Parameters.Add(Sp);
        }

        public void Add_Parameter(string Name, SqlDbType DbType, Int32 Size = 0, byte Precision = 0, byte Scale = 0, Object Value = null)
        {
            SqlParameter Sp = new SqlParameter(Name, DbType, Size);
            Sp.Scale = Scale;
            Sp.Precision = Precision;
            Sp.Value = Value;
            this.mCmd.Parameters.Add(Sp);
        }

        /*
        public void Add_Parameter(string Name, SqlDbType DbType, Int32 Size, byte Precision, byte Scale)
        {
            this.Add_Parameter(Name, DbType, Size, Precision, Scale, null);
        }

        public void Add_Parameter(string Name, SqlDbType DbType, Int32 Size, byte Precision)
        {
            this.Add_Parameter(Name, DbType, Size, Precision, 0, null);
        }

        public void Add_Parameter(string Name, SqlDbType DbType, Int32 Size)
        {
            this.Add_Parameter(Name, DbType, Size, 0, 0, null);
        }

        public void Add_Parameter(string Name, SqlDbType DbType)
        {
            this.Add_Parameter(Name, DbType, 0, 0, 0, null);
        }
        */

        public void Prepare() 
        {
            this.mCmd.Prepare();
        }

        public DataSet ExecuteQuery()
        {
            DataSet Ds = new DataSet();
            SqlDataAdapter Adp = new SqlDataAdapter();
            Adp.SelectCommand = this.mCmd;
            Adp.Fill(Ds);
            return Ds;
        }

        public void ExecuteNonQuery()
        {
            this.mCmd.ExecuteNonQuery();
        }

        #endregion

        #region _Properties

        public ClsConnection_SqlServer pDa 
        {
            get 
            {
                return this.mDa;
            }
        }

        public SqlCommand pCmd
        {
            get
            {
                return this.mCmd;
            }
        }

        public SqlParameterCollection pParameters 
        {
            get
            {
                return this.mCmd.Parameters;
            }
        }

        public string pQuery 
        {
            get 
            {
                return this.mCmd.CommandText;
            }
            set 
            {
                this.mCmd.CommandText = value;
            }
        }

        #endregion

    }
}
