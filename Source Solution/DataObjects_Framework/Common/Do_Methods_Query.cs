using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Objects;
using DataObjects_Framework.Base;

namespace DataObjects_Framework.Common
{
    /// <summary>
    /// Common Querying methods used by the framework are defined here.
    /// </summary>
    public class Do_Methods_Query
    {
        public static DataTable GetQuery(Interface_DataAccess Da, string ViewObject, string Fields = "", string Condition = "", string Sort = "", Int64 Top = 0, Int32 Page = 0)
        { return Da.GetQuery(ViewObject, Fields, Condition, Sort); }

        public static DataTable GetQuery(string ViewObject, string Fields = "", string Condition = "", string Sort = "", Int64 Top = 0 , Int32  Page = 0)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            try
            {
                Da.Connect();
                return GetQuery(Da, ViewObject, Fields, Condition, Sort, Top, Page);
            }
            catch (Exception ex) { throw ex; }
            finally { Da.Close(); }
        }

        public static DataTable GetQuery(Interface_DataAccess Da, string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        { return Da.GetQuery(ViewObject, Fields, Condition, Sort, Top, Page); }

        public static DataTable GetQuery(string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            DataTable Dt = null;
            try
            {
                Da.Connect();
                Dt = GetQuery(Da, ViewObject, Fields, Condition, Sort, Top, Page);
            }
            catch (Exception Ex) { throw Ex; }
            finally { Da.Close(); }

            return Dt;
        }

        public static Int32 ExecuteNonQuery(string ProcedureName, ClsParameter[] ProcedureParameters)
        {
            return Do_Methods_Query.ExecuteNonQuery(ProcedureName, ProcedureParameters.ToList());
        }

        public static Int32 ExecuteNonQuery(string ProcedureName, List<ClsParameter> ProcedureParameters)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            return Da.ExecuteNonQuery(ProcedureName, ProcedureParameters);
        }

        public static Int32 ExecuteNonQuery(string Query)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            return Da.ExecuteNonQuery(Query);
        }

        public static DataSet ExecuteQuery(string ProcedureName, ClsParameter[] ProcedureParameters)
        {
            return Do_Methods_Query.ExecuteQuery(ProcedureName, ProcedureParameters.ToList());
        }

        public static DataSet ExecuteQuery(string ProcedureName, List<ClsParameter> ProcedureParameters)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            return Da.ExecuteQuery(ProcedureName, ProcedureParameters);
        }

        public static DataSet ExecuteQuery(string Query)
        {
            Interface_DataAccess Da = Do_Methods.CreateDataAccess();
            return Da.ExecuteQuery(Query);
        }

        public static DataTable GetTableDef(string TableName)
        {
            List<ClsParameter> Sp = new List<ClsParameter>();
            Sp.Add(new ClsParameter("@TableName",TableName));
            return ExecuteQuery("usp_GetTableDef", Sp).Tables[0];
        }

        public static string GetSystemParameter(string ParameterName)
        {
            string Rv = "";
            List<ClsParameter> Sp = new List<ClsParameter>();
            Sp.Add(new ClsParameter("ParameterName", ParameterName));
            DataTable Dt = Do_Methods_Query.ExecuteQuery("usp_Get_System_Parameter", Sp).Tables[0];
            if (Dt.Rows.Count > 0)
            { Rv = (string)Dt.Rows[0][0]; }

            return Rv;
        }

        public static DataRow GetSystemBindDefinition(string Name)
        {
            DataTable Dt_Bind = Do_Methods_Query.GetQuery(@"System_BindDefinition", @"", @"Name = '" + Name + "'");
            DataRow Dr_Bind;
            if (Dt_Bind.Rows.Count > 0)
            { Dr_Bind = Dt_Bind.Rows[0]; }
            else
            { throw new Exception("GetSystemBindDefinition: Bind Defintion not found."); }

            return Dr_Bind;
        }

        public static DataTable GetSystemLookup(string LookupName)
        {
            DataTable Dt = Do_Methods_Query.GetQuery(@"udf_System_Lookup('" + LookupName + @"')");
            return Dt;
        }

        public static DataTable GetLookup(string LookupName)
        {
            DataTable Dt = Do_Methods_Query.GetQuery(@"udf_Lookup('" + LookupName + @"')");
            return Dt;
        }
    }
}
