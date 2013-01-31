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
    public class PreparedQuery_SqlServer : PreparedQuery
    {
        #region _Constructors

        public PreparedQuery_SqlServer(Interface_Connection Cn, String Query = "", List<QueryParameter> Parameters = null) : base(Cn, Query, Parameters) { }

        public PreparedQuery_SqlServer(String Query = "", List<QueryParameter> Parameters = null) : base(Query, Parameters) { }

        #endregion

        #region _Methods

        protected override DbParameter ConvertParameter(QueryParameter Parameter)
        {
            SqlParameter Rv_Parameter = new SqlParameter();
            Rv_Parameter.ParameterName = Parameter.Name;
            Rv_Parameter.Value = Parameter.Value;
            Rv_Parameter.Size = Parameter.Size;
            Rv_Parameter.Scale = Parameter.Scale;
            Rv_Parameter.Precision = Parameter.Precision;

            switch (Parameter.Type)
            {
                case Do_Constants.eParameterType.Binary:
                    Rv_Parameter.SqlDbType = SqlDbType.VarBinary;
                    break;
                case Do_Constants.eParameterType.Boolean:
                    Rv_Parameter.SqlDbType = SqlDbType.Bit;
                    break;
                case Do_Constants.eParameterType.DateTime:
                    Rv_Parameter.SqlDbType = SqlDbType.DateTime;
                    break;
                case Do_Constants.eParameterType.Guid:
                    Rv_Parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                    break;
                case Do_Constants.eParameterType.Int:
                    Rv_Parameter.SqlDbType = SqlDbType.Int;
                    break;
                case Do_Constants.eParameterType.Long:
                    Rv_Parameter.SqlDbType = SqlDbType.BigInt;
                    break;
                case Do_Constants.eParameterType.Numeric:
                    Rv_Parameter.SqlDbType = SqlDbType.Decimal;
                    break;
                case Do_Constants.eParameterType.VarChar:
                    Rv_Parameter.SqlDbType = SqlDbType.VarChar;
                    break;
            }

            return Rv_Parameter;
        }

        #endregion
    }
}
