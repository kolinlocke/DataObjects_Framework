using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;

namespace Layer01_Common.Common
{
    public class Methods_Query
    {
        public static DataTable GetQuery(ClsConnection_SqlServer Da, string ViewObject, string Fields, string Condition, string Sort) 
        {
            if (ViewObject.Trim() != "") ViewObject = " From " + ViewObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Condition.Trim() != "") Condition = " Where " + Condition;
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            ClsPreparedQuery Pq = new ClsPreparedQuery(Da);
            Pq.pQuery = @"Declare @Query As VarChar(Max); Set @Query = 'Select ' + @Fields + ' ' + @ViewObject + ' ' + @Condition + ' ' + @Sort; Exec(@Query)";
            Pq.Add_Parameter("ViewObject", SqlDbType.VarChar, 8000, 0, 0, ViewObject);
            Pq.Add_Parameter("Fields", SqlDbType.VarChar, 8000, 0, 0, Fields);
            Pq.Add_Parameter("Condition", SqlDbType.VarChar, 8000, 0, 0, Condition);
            Pq.Add_Parameter("Sort", SqlDbType.VarChar, 8000, 0, 0, Sort);
            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }

        public static DataTable GetQuery(ClsConnection_SqlServer Da, string ViewObject, string Fields, string Condition)
        {
            return GetQuery(Da, ViewObject, Fields, Condition, "");
        }

        public static DataTable GetQuery(ClsConnection_SqlServer Da, string ViewObject, string Fields)
        {
            return GetQuery(Da, ViewObject, Fields, "", "");
        }

        public static DataTable GetQuery(ClsConnection_SqlServer Da, string ViewObject)
        {
            return GetQuery(Da, ViewObject, "", "", "");
        }

        public static DataTable GetQuery(string ViewObject, string Fields, string Condition, string Sort)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return GetQuery(Da, ViewObject, Fields, Condition, Sort);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static DataTable GetQuery(string ViewObject, string Fields, string Condition)
        {
            return GetQuery(ViewObject, Fields, Condition, "");
        }

        public static DataTable GetQuery(string ViewObject, string Fields)
        {
            return GetQuery(ViewObject, Fields, "", "");
        }

        public static DataTable GetQuery(string ViewObject)
        {
            return GetQuery(ViewObject, "", "", "");
        }

        public static DataTable GetQuery(ClsConnection_SqlServer Da, string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            string Query_RowNumberSort = Sort;
            if (Query_RowNumberSort.Trim() == "") Query_RowNumberSort = "(Select 0)";

            string Query_Top = "";
            if (Top > 0) Query_Top = "Top " + Top.ToString();

            Int64 PageCondition = 0;
            if (Page > 0)
            { PageCondition = Top * (Page - 1); }

            if (ViewObject.Trim() != "") ViewObject = " From " + ViewObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            ClsPreparedQuery Pq = new ClsPreparedQuery(Da);
            Pq.Add_Parameter("ViewObject", SqlDbType.VarChar, 8000, 0, 0, ViewObject);
            Pq.Add_Parameter("Fields", SqlDbType.VarChar, 8000, 0, 0, Fields);
            Pq.Add_Parameter("Sort", SqlDbType.VarChar, 8000, 0, 0, Sort);

            string Query_Condition = "";
            if (Condition != null)
            {
                Query_Condition = " Where 1 = 1 ";
                Query_Condition += " And " + Condition.GetQueryCondition();
                Pq.Add_Parameter(Condition.GetParameters());
            }

            Pq.pQuery = @"Select " + Query_Top + @" [Tb].* From ( Select Row_Number() Over (Order By " + Query_RowNumberSort + @") As [RowNumber], " + Fields + " " + ViewObject + " " + Query_Condition + @" ) As [Tb] Where [Tb].RowNumber > " + PageCondition + " " + Sort;
            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }

        public static DataTable GetQuery(string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", Int64 Top = 0, Int32 Page = 0)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return GetQuery(Da, ViewObject, Fields, Condition, Sort, Top, Page);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static DataTable GetQueryWithPage(ClsConnection_SqlServer Da, string ViewObject, string Fields, string Condition, string Sort, Int64 Top, Int32 Page)
        {
            string Query_RowNumberSort = Sort;
            if (Query_RowNumberSort.Trim() == "") Query_RowNumberSort = "(Select 0)";

            string Query_Top = "";
            if (Top > 0) Query_Top = "Top " + Top.ToString();

            Int64 PageCondition = 0;
            if (Page > 0) 
            {
                PageCondition = Top * (Page - 1);
            }

            if (ViewObject.Trim() != "") ViewObject = " From " + ViewObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Condition.Trim() != "") Condition = " Where " + Condition;
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            ClsPreparedQuery Pq = new ClsPreparedQuery(Da);
            Pq.pQuery = @"Declare @Query As VarChar(Max); Set @Query = 'Select ' + @Top ' + [Tb].* From ( Select Row_Number() Over (Order By ' + @RowNumberSort + ') As [RowNumber], ' + @Fields + ' ' + @ViewObject + ' ' + @Condition + ' ' + @Sort + ' ) As [Tb] Where [Tb].RowNumber >= ' + @PageCondtion + ''; Exec(@Query)";
            Pq.Add_Parameter("ViewObject", SqlDbType.VarChar, 8000, 0, 0, ViewObject);
            Pq.Add_Parameter("Top", SqlDbType.VarChar, 8000, 0, 0, Query_Top);
            Pq.Add_Parameter("RowNumberSort", SqlDbType.VarChar, 8000, 0, 0, Query_RowNumberSort);
            Pq.Add_Parameter("PageCondtion", SqlDbType.BigInt, 0, 0, 0, PageCondition);
            Pq.Add_Parameter("Fields", SqlDbType.VarChar, 8000, 0, 0, Fields);
            Pq.Add_Parameter("Condition", SqlDbType.VarChar, 8000, 0, 0, Condition);
            Pq.Add_Parameter("Sort", SqlDbType.VarChar, 8000, 0, 0, Sort);
            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }

        public static DataTable GetQueryWithPage(ClsConnection_SqlServer Da, string ViewObject, string Fields, string Condition, string Sort, Int64 Top)
        {
            return GetQueryWithPage(Da, ViewObject, Fields, Condition, Sort, Top, 0);
        }

        public static DataTable GetQueryWithPage(ClsConnection_SqlServer Da, string ViewObject, string Fields, string Condition, string Sort)
        {
            return GetQueryWithPage(Da, ViewObject, Fields, Condition, Sort, 0, 0);
        }

        public static DataTable GetQueryWithPage(ClsConnection_SqlServer Da, string ViewObject, string Fields, string Condition)
        {
            return GetQueryWithPage(Da, ViewObject, Fields, Condition, "", 0, 0);
        }

        public static DataTable GetQueryWithPage(ClsConnection_SqlServer Da, string ViewObject, string Fields)
        {
            return GetQueryWithPage(Da, ViewObject, Fields, "", "", 0, 0);
        }

        public static DataTable GetQueryWithPage(ClsConnection_SqlServer Da, string ViewObject)
        {
            return GetQueryWithPage(Da, ViewObject, "", "", "", 0, 0);
        }

        public static DataTable GetQueryWithPage(string ViewObject, string Fields, string Condition, string Sort, Int64 Top, Int32 Page)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return GetQueryWithPage(Da, ViewObject, Fields, Condition, Sort,Top,Page);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static DataTable GetQueryWithPage(string ViewObject, string Fields, string Condition, string Sort, Int64 Top)
        {
            return GetQueryWithPage(ViewObject, Fields, Condition, Sort, Top, 0);
        }

        public static DataTable GetQueryWithPage(string ViewObject, string Fields, string Condition, string Sort)
        {
            return GetQueryWithPage(ViewObject, Fields, Condition, Sort, 0, 0);
        }

        public static DataTable GetQueryWithPage(string ViewObject, string Fields, string Condition)
        {
            return GetQueryWithPage(ViewObject, Fields, Condition, "", 0, 0);
        }

        public static DataTable GetQueryWithPage(string ViewObject, string Fields)
        {
            return GetQueryWithPage(ViewObject, Fields, "", "", 0, 0);
        }

        public static DataTable GetQueryWithPage(string ViewObject)
        {
            return GetQueryWithPage(ViewObject, "", "", "", 0, 0);
        }

        public static Int32 ExecuteNonQuery(string ProcedureName, Common.Layer01_Constants.Str_Parameters[] ProcedureParameters)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return Da.ExecuteNonQuery(ProcedureName, ProcedureParameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static Int32 ExecuteNonQuery(string ProcedureName, List<Common.Layer01_Constants.Str_Parameters> ProcedureParameters)
        {
            return ExecuteNonQuery(ProcedureName, ProcedureParameters.ToArray());
        }

        public static Int32 ExecuteNonQuery(string Query)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return Da.ExecuteNonQuery(Query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static DataSet ExecuteQuery(string ProcedureName, Common.Layer01_Constants.Str_Parameters[] ProcedureParameters)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return Da.ExecuteQuery(ProcedureName, ProcedureParameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static DataSet ExecuteQuery(string ProcedureName, List<Common.Layer01_Constants.Str_Parameters> ProcedureParameters)
        {
            return ExecuteQuery(ProcedureName, ProcedureParameters.ToArray());
        }

        public static DataSet ExecuteQuery(string Query)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return Da.ExecuteQuery(Query);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Da.Close();
            }
        }

        public static DataTable GetTableDef(string TableName)
        {
            List<Common.Layer01_Constants.Str_Parameters> Sp = new List<Common.Layer01_Constants.Str_Parameters>();
            Sp.Add(new Common.Layer01_Constants.Str_Parameters("@TableName",TableName));
            return ExecuteQuery("usp_GetTableDef", Sp).Tables[0];
        }

        public static string GetSystemParameter(string ParameterName)
        {
            string Rv = "";
            List<Common.Layer01_Constants.Str_Parameters> Sp = new List<Common.Layer01_Constants.Str_Parameters>();
            Sp.Add(new Common.Layer01_Constants.Str_Parameters("ParameterName", ParameterName));
            DataTable Dt = ExecuteQuery("usp_Get_System_Parameter", Sp).Tables[0];
            if (Dt.Rows.Count > 0)
            { Rv = (string)Dt.Rows[0][0]; }

            return Rv;
        }

        public static DataRow GetSystemBindDefinition(string Name)
        {
            DataTable Dt_Bind = GetQuery(@"System_BindDefinition", @"", @"Name = '" + Name + "'");
            DataRow Dr_Bind;
            if (Dt_Bind.Rows.Count > 0)
            { Dr_Bind = Dt_Bind.Rows[0]; }
            else
            { throw new Exception("GetSystemBindDefinition: Bind Defintion not found."); }

            return Dr_Bind;
        }

        public static DataTable GetSystemLookup(string LookupName)
        {
            DataTable Dt = GetQuery(@"udf_System_Lookup('" + LookupName + @"')");
            return Dt;
        }

        public static DataTable GetLookup(string LookupName)
        {
            DataTable Dt = GetQuery(@"udf_Lookup('" + LookupName + @"')");
            return Dt;
        }

        public static void AddSelected(
            DataTable Dt_Target
            , DataTable Dt_Selected
            , string Query_Selected_Source
            , string Query_Selected_Key
            , string Target_Key
            , List<Common.Layer01_Constants.Str_AddSelectedFields> Obj_Fields = null
            , List<Common.Layer01_Constants.Str_AddSelectedFieldsDefault> Obj_FieldsDefault = null)
        {
            if (!(Dt_Selected.Rows.Count > 0))
            { return; }

            ClsPreparedQuery Pq = new ClsPreparedQuery();
            Pq.pQuery = @"Select * From " + Query_Selected_Source + @" Where " + Query_Selected_Key + @" = @ID";
            Pq.pParameters.Add("ID", SqlDbType.BigInt);
            Pq.Prepare();

            foreach (DataRow Dr_Selected in Dt_Selected.Rows)
            {
                Pq.pParameters["ID"].Value = (Int64)Methods.IsNull(Dr_Selected["ID"], 0);
                DataTable Inner_Dt_Selected = Pq.ExecuteQuery().Tables[0];
                if (Inner_Dt_Selected.Rows.Count > 0)
                {
                    DataRow Inner_Dr_Selected = Inner_Dt_Selected.Rows[0];
                    DataRow[] Inner_ArrDr;
                    DataRow Inner_Dr_Target = null;
                    bool Inner_IsFound = false;

                    Inner_ArrDr = Dt_Target.Select(Target_Key + " = " + Convert.ToInt64(Inner_Dr_Selected[Query_Selected_Key]));
                    if (Inner_ArrDr.Length > 0)
                    {
                        Inner_Dr_Target = Inner_ArrDr[0];
                        if ((bool)Methods.IsNull(Inner_Dr_Target["IsDeleted"], false))
                        {
                            Inner_Dr_Target["IsDeleted"] = DBNull.Value;
                            Inner_IsFound = true;
                        }
                    }

                    if (!Inner_IsFound)
                    {
                        Int64 Ct = 0;
                        Inner_ArrDr = Dt_Target.Select("", "TmpKey Desc");
                        if (Inner_ArrDr.Length > 0)
                        { Ct = (Int64)Inner_ArrDr[0]["TmpKey"]; }
                        Ct++;

                        DataRow Nr = Dt_Target.NewRow();
                        Nr["TmpKey"] = Ct;
                        Nr["Item_Style"] = "";
                        Nr[Target_Key] = (Int64)Inner_Dr_Selected[Query_Selected_Key];
                        Dt_Target.Rows.Add(Nr);

                        Inner_Dr_Target = Nr;
                    }

                    if (Obj_Fields != null)
                    {
                        foreach (Layer01_Constants.Str_AddSelectedFields Inner_Obj in Obj_Fields)
                        { Inner_Dr_Target[Inner_Obj.Field_Target] = Inner_Dr_Selected[Inner_Obj.Field_Selected]; }
                    }

                    if (Obj_FieldsDefault != null)
                    {
                        foreach (Layer01_Constants.Str_AddSelectedFieldsDefault Inner_Obj in Obj_FieldsDefault)
                        { Inner_Dr_Target[Inner_Obj.Field_Target] = Inner_Obj.Value; }
                    }
                }
            }
        }

    }
}
