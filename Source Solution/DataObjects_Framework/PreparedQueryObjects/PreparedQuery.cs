using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Objects;

namespace DataObjects_Framework.PreparedQueryObjects
{
    /// <summary>
    /// Abstract class PreparedQuery
    /// Wrapper for using prepared statements
    /// </summary>
    public abstract class PreparedQuery
    {
        #region _Variables

        protected Interface_DataAccess mDa = null;
        protected Interface_Connection mCn = null;
        protected DbCommand mCmd = null;
        protected String mQuery = "";
        protected List<QueryParameter> mParameters = new List<QueryParameter>();

        #endregion

        #region _Constructors

        internal PreparedQuery() { }

        public PreparedQuery(Interface_Connection Cn, String Query = "", List<QueryParameter> Parameters = null)
        { this.Setup(Cn, Query, Parameters); }

        public PreparedQuery(String Query = "", List<QueryParameter> Parameters = null)
        {
            Interface_Connection Cn = Do_Methods.CreateDataAccess().CreateConnection();
            Cn.Connect();
            this.Setup(Cn, Query, Parameters);
        }

        void Setup(Interface_Connection Cn, String Query = "", List<QueryParameter> Parameters = null)
        {
            this.mCn = Cn;
            this.mCmd = this.mCn.CreateCommand();
            this.mCmd.Connection = this.mCn.pConnection;
            this.mCmd.Transaction = this.mCn.pTransaction;
            this.mCmd.CommandType = CommandType.Text;

            if (Query != "")
            { this.mCmd.CommandText = Query; }

            if (Parameters != null)
            { this.Add_Parameter(Parameters); }
        }

        #endregion

        #region _Methods

        public void Add_Parameter(QueryParameter Parameter)
        {
            DbParameter Dbp = this.ConvertParameter(Parameter);
            if (Dbp != null) { this.mCmd.Parameters.Add(Dbp); }

            this.mParameters.Add(Parameter);
        }

        public void Add_Parameter(List<QueryParameter> Parameters)
        {
            foreach (QueryParameter Inner_Parameter in Parameters)
            { this.Add_Parameter(Inner_Parameter); }
        }

        public void Add_Parameter(String Name, Do_Constants.eParameterType Type, Object Value = null, Int32 Size = 0, Byte Scale = 0, Byte Precision = 0)
        {
            QueryParameter Parameter =
                new QueryParameter()
                {
                    Name = Name,
                    Value = Value,
                    Type = Type,
                    Size = Size,
                    Scale = Scale,
                    Precision = Precision
                };

            this.Add_Parameter(Parameter);
        }

        public void UpdateParameterValue()
        {
            foreach (QueryParameter Sp in this.mParameters)
            { this.mCmd.Parameters[Sp.Name].Value = Sp.Value; }
        }

        //public void UpdateParameterValue(Do_Constants.Str_Parameters Parameter)
        //{
        //    if (this.mParameters.Exists(X => X.Name == Parameter.Name))
        //    { this.mCmd.Parameters[Parameter.Name].Value = Parameter.Value; }            
        //}

        //public void UpdateParameterValue(List<Do_Constants.Str_Parameters> Parameters)
        //{
        //    foreach (Do_Constants.Str_Parameters Inner_Parameter in Parameters)
        //    { this.UpdateParameterValue(Inner_Parameter); }
        //}

        protected virtual DbParameter ConvertParameter(QueryParameter Parameter)
        { return null; }

        public virtual void Prepare()
        { this.mCmd.Prepare(); }

        void CheckParameterValues()
        {
            foreach (QueryParameter Sp in this.mParameters)
            {
                QueryParameter Inner_Sp = Sp;
                Inner_Sp.Value = this.ConvertParameterValues(Inner_Sp.Value, Inner_Sp.Type);
                this.mCmd.Parameters[Inner_Sp.Name].Value = Inner_Sp.Value;
            }
        }

        Object ConvertParameterValues(Object Input, Do_Constants.eParameterType Type)
        {
            if (Input == null)
            { return DBNull.Value; }

            if (Input == DBNull.Value)
            { return DBNull.Value; }

            switch (Type)
            {
                case Do_Constants.eParameterType.Binary:
                    Input = Do_Methods.Convert_Byte(Input);
                    break;
                case Do_Constants.eParameterType.Boolean:
                    Input = Do_Methods.Convert_Boolean(Input);
                    break;
                case Do_Constants.eParameterType.DateTime:
                    DateTime? Inner_Date = Do_Methods.Convert_DateTime(Input);
                    if (Inner_Date == null) { Input = DBNull.Value; }
                    Input = Inner_Date.Value;
                    break;
                case Do_Constants.eParameterType.Int:
                    Input = Do_Methods.Convert_Int32(Input);
                    break;
                case Do_Constants.eParameterType.Long:
                    Input = Do_Methods.Convert_Int64(Input);
                    break;
                case Do_Constants.eParameterType.Numeric:
                    Input = Do_Methods.Convert_Double(Input);
                    break;
                case Do_Constants.eParameterType.VarChar:
                    Input = Do_Methods.Convert_String(Input);
                    break;
            }

            return Input;
        }

        public virtual DataSet ExecuteQuery()
        {
            this.CheckParameterValues();
            this.UpdateParameterValue();

            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            return Da.ExecuteQuery(this.mCn, this.mCmd);
        }

        public virtual void ExecuteNonQuery()
        {
            this.CheckParameterValues();
            this.UpdateParameterValue();

            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            Da.ExecuteNonQuery(this.mCn, this.mCmd);
        }

        #endregion

        #region _Properties

        public Interface_Connection pCn
        {
            get { return this.mCn; }
        }

        public DbCommand pCmd
        {
            get { return this.mCmd; }
        }

        public String pQuery
        {
            get
            { return this.mQuery; }
            set
            {
                this.mQuery = value;
                if (this.mCmd != null)
                { this.mCmd.CommandText = this.mQuery; }
            }
        }

        public List<QueryParameter> pParameters
        {
            get { return this.mParameters; }
        }

        public Object pParameter_Get(String Name)
        {
            QueryParameter P = this.mParameters.FirstOrDefault(O => O.Name == Name);
            if (P != null)
            {
                return P.Value;
            }
            else
            { throw new Exception("Parameter not found."); }
        }

        public void pParameter_Set(String Name, Object Value)
        {
            QueryParameter P = this.mParameters.FirstOrDefault(O => O.Name == Name);
            if (P != null)
            {
                P.Value = Value;
            }
            else
            { throw new Exception("Parameter not found."); }
        }

        #endregion
    }
}
