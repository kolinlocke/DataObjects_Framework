using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Connection;
using Layer01_Common.Common;
using Layer01_Common.Objects;
using DataObjects_Framework;
using DataObjects_Framework._System;

namespace DataObjects_Framework.DataAccess
{
    public class ClsDataAccess_SqlServer : Interface_DataAccess
    {
        #region _Variables

        ClsConnection_SqlServer mConnection;

        #endregion

        #region _ImpelementedMethods

        public DataTable GetQuery(
            Interface_Connection Connection
            , string ViewObject
            , string Fields = ""
            , string Condition = ""
            , string Sort = "")
        {
            if (ViewObject.Trim() != "") ViewObject = " From " + ViewObject + " ";
            if (Fields.Trim() == "") Fields = " * ";
            if (Condition.Trim() != "") Condition = " Where " + Condition;
            if (Sort.Trim() != "") Sort = " Order By " + Sort;

            ClsPreparedQuery Pq = new ClsPreparedQuery((ClsConnection_SqlServer)Connection);
            Pq.pQuery = @"Declare @Query As VarChar(Max); Set @Query = 'Select ' + @Fields + ' ' + @ViewObject + ' ' + @Condition + ' ' + @Sort; Exec(@Query)";
            Pq.Add_Parameter("ViewObject", SqlDbType.VarChar, 8000, 0, 0, ViewObject);
            Pq.Add_Parameter("Fields", SqlDbType.VarChar, 8000, 0, 0, Fields);
            Pq.Add_Parameter("Condition", SqlDbType.VarChar, 8000, 0, 0, Condition);
            Pq.Add_Parameter("Sort", SqlDbType.VarChar, 8000, 0, 0, Sort);
            Pq.Prepare();

            return Pq.ExecuteQuery().Tables[0];
        }
        
        public DataTable GetQuery(string ViewObject, string Fields = "", string Condition = "", string Sort = "")
        {
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                return this.GetQuery(Cn, ViewObject, Fields, Condition, Sort);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        public DataTable GetQuery(Interface_Connection Connection, string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
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

            ClsPreparedQuery Pq = new ClsPreparedQuery((ClsConnection_SqlServer)Connection);
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

        public DataTable GetQuery(string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return GetQuery(Da, ViewObject, Fields, Condition, Sort, Top, Page);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Da.Close(); }
        }
                
        public Interface_Connection Connection
        {
            get { return this.mConnection; }
        }

        public void Connect()
        {
            this.mConnection = new ClsConnection_SqlServer();
            this.mConnection.Connect();
        }

        public void Close()
        { this.mConnection.Close(); }

        public void BeginTransaction()
        { this.mConnection.BeginTransaction(); }

        public void CommitTransaction()
        { this.mConnection.CommitTransaction(); }

        public void RollbackTransaction()
        { this.mConnection.RollbackTransaction(); }

        public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false)
        { return this.mConnection.SaveDataRow(ObjDataRow, TableName, SchemaName, IsDelete); }
        
        public DataTable List(
            string ObjectName
            , string Condition = ""
            , string Sort = "")
        {
            DataTable Dt = this.GetQuery(ObjectName, "*", Condition, Sort);
            return Dt;
        }

        public DataTable List(
            string ObjectName
            , ClsQueryCondition Condition
            , string Sort = ""
            , int Top = 0
            , int Page = 0)
        {
            DataTable Dt = this.GetQuery(ObjectName, "*", Condition, Sort, Top, Page);
            return Dt;
        }
        
        public long List_Count(string ObjectName, ClsQueryCondition Condition = null)
        {
            DataTable Dt = this.GetQuery(ObjectName, "Count(1) As [Ct]", Condition);
            Int64 ReturnValue = 0;
            try
            { ReturnValue =  Layer01_Methods.Convert_Int64(Dt.Rows[0]["Ct"], 0); }
            catch { }
            return ReturnValue;
        }

        public DataTable List_Empty(Interface_Connection Connection, string ObjectName)
        {
            
            return this.GetQuery(Connection, ObjectName, "*", "1 = 0");
        }

        public DataTable List_Empty(string ObjectName)
        {
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                return this.List_Empty(Cn, ObjectName);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        public DataRow Load(string ObjectName, List<string> List_Key, ClsKeys Keys)
        {
            DataTable Dt;
            DataRow Dr;
            
            if (Keys == null)
            { Dr = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0").NewRow(); }
            else
            {
                StringBuilder Sb_Condition = new StringBuilder();
                
                if (Keys.Count() != List_Key.Count)
                { throw new Exception("Keys not equal to required keys."); }
                
                Sb_Condition.Append(" 1 = 1 ");
                foreach (string Inner_Key in List_Key)
                { Sb_Condition.Append(" And " + Inner_Key + " = " + Keys[Inner_Key]); }

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString());
                if (Dt.Rows.Count > 0)
                { Dr = Dt.Rows[0]; }
                else
                { throw new ClsCustomException("Record not found."); }
            }
            return Dr;
        }

        public DataTable Load_TableDetails(string ObjectName,  ClsKeys Keys, string Condition)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            DataTable Dt;

            if (Keys == null)
            { Dt = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0"); }
            else
            {
                string Inner_Condition_And = "";
                bool IsStart = false;
                foreach (string Inner_Key in Keys.pName)
                {
                    Sb_Condition.Append(Inner_Condition_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
                    if (!IsStart) Inner_Condition_And = " And ";
                    IsStart = true;
                }

                string OtherCondition = "";
                if (Condition != "") OtherCondition = " And " + Condition;

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString() + OtherCondition);
            }
            
            return Dt;
        }

        public DataRow Load_RowDetails(string ObjectName,  ClsKeys Keys, string Condition)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            DataTable Dt;
            DataRow Dr;

            if (Keys == null)
            { Dr = this.GetQuery(this.Connection, ObjectName, "*", "1 = 0").NewRow(); }
            else
            {
                string Inner_Condition_And = "";
                bool IsStart = false;
                foreach (string Inner_Key in Keys.pName)
                {
                    Sb_Condition.Append(Inner_Condition_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
                    if (!IsStart) Inner_Condition_And = " And ";
                    IsStart = true;
                }

                string OtherCondition = "";
                if (Condition != "") OtherCondition = " And " + Condition;

                Dt = this.GetQuery(this.Connection, ObjectName, "*", Sb_Condition.ToString() + OtherCondition);
                if (Dt.Rows.Count > 0) Dr = Dt.Rows[0];
                else Dr = Dt.NewRow();                
            }
            return Dr;
        }

        public ClsQueryCondition CreateQueryCondition()
        { return new ClsQueryCondition(); }

        public DataTable GetTableDef(string TableName)
        {
            DataTable Rv = null;
            List<Layer01_Constants.Str_Parameters> Sp = new List<Layer01_Constants.Str_Parameters>();
            Sp.Add(new Layer01_Constants.Str_Parameters("@TableName", TableName));
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                Rv = Cn.ExecuteQuery("usp_GetTableDef", Sp).Tables[0];
            }
            catch { }
            finally
            { Cn.Close(); }
            return Rv;
        }

        public string GetSystemParameter(string ParameterName, string DefaultValue = "")
        {
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                return this.GetSystemParameter(Cn, ParameterName, DefaultValue);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        public string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "")
        {
            ClsConnection_SqlServer Cn = (ClsConnection_SqlServer)Connection;

            string Rv = "";
            List<Layer01_Constants.Str_Parameters> Sp = new List<Layer01_Constants.Str_Parameters>();
            Sp.Add(new Layer01_Constants.Str_Parameters("ParameterName", ParameterName));
            Sp.Add(new Layer01_Constants.Str_Parameters("DefaultValue", DefaultValue));
            DataTable Dt = Cn.ExecuteQuery("usp_Get_System_Parameter", Sp).Tables[0];
            if (Dt.Rows.Count > 0)
            { Rv = (string)Dt.Rows[0][0]; }
            return Rv;                
        }

        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            ClsConnection_SqlServer Cn = new ClsConnection_SqlServer();
            try
            {
                Cn.Connect();
                this.SetSystemParameter(ParameterName, ParameterValue);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        public void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue)
        {
            ClsConnection_SqlServer Cn = (ClsConnection_SqlServer)Connection;
            List<Layer01_Constants.Str_Parameters> Sp = new List<Layer01_Constants.Str_Parameters>();
            Sp.Add(new Layer01_Constants.Str_Parameters("ParameterName", ParameterName));
            Sp.Add(new Layer01_Constants.Str_Parameters("ParameterValue", ParameterValue));
            Cn.ExecuteNonQuery("usp_Set_System_Parameter", Sp);
        }

        public void AddSelected(
            DataTable Dt_Target
            , List<long> Selected_IDs
            , string Selected_DataSourceName
            , string Selected_KeyName
            , string Target_Key
            , bool HasTmpKey
            , List<Layer01_Constants.Str_AddSelectedFields> List_Selected_Fields = null
            , List<Layer01_Constants.Str_AddSelectedFieldsDefault> List_Selected_FieldsDefault = null)
        {
            if (Selected_IDs == null)
            { return; }

            if (Selected_IDs.Count == 0)
            { return; }

            ClsPreparedQuery Pq = new ClsPreparedQuery();
            Pq.pQuery = @"Select * From " + Selected_DataSourceName + @" Where " + Selected_KeyName + @" = @ID";
            Pq.Add_Parameter("ID", SqlDbType.BigInt);
            Pq.Prepare();

            foreach (Int64 Selected_ID in Selected_IDs)
            {
                Pq.pParameters["ID"].Value = Selected_ID;
                DataTable Dt_Selected = Pq.ExecuteQuery().Tables[0];
                if (Dt_Selected.Rows.Count > 0)
                {
                    DataRow Dr_Selected = Dt_Selected.Rows[0];
                    DataRow[] ArrDr;
                    DataRow Dr_Target = null;
                    
                    ArrDr = Dt_Target.Select(Target_Key + @" = " + Layer01_Methods.Convert_Int64(Dr_Selected[Selected_KeyName]));
                    if (ArrDr.Length > 0)
                    { Dr_Target = ArrDr[0]; }
                    else
                    {
                        Dr_Target = Dt_Target.NewRow();
                        Dt_Target.Rows.Add(Dr_Target);

                        Dr_Target[Target_Key] = Dr_Selected[Selected_KeyName];

                        if (HasTmpKey)
                        {
                            Int64 Ct = 0;
                            ArrDr = Dt_Target.Select("", "TmpKey Desc", DataViewRowState.CurrentRows);
                            if (ArrDr.Length > 0)
                            { Ct = Layer01_Methods.Convert_Int64(ArrDr[0]["TmpKey"]); }
                            Ct++;

                            Dr_Target["TmpKey"] = Ct;
                            Dr_Target["Item_Style"] = "";
                        }
                    }

                    if (List_Selected_Fields != null)
                    {
                        foreach (Layer01_Constants.Str_AddSelectedFields Selected_Field in List_Selected_Fields)
                        { Dr_Target[Selected_Field.Field_Target] = Dr_Selected[Selected_Field.Field_Selected]; }
                    }

                    if (List_Selected_FieldsDefault != null)
                    {
                        foreach (Layer01_Constants.Str_AddSelectedFieldsDefault Selected_FieldDefault in List_Selected_FieldsDefault)
                        { Dr_Target[Selected_FieldDefault.Field_Target] = Selected_FieldDefault.Value; }
                    }

                }
            }

        }

        #endregion
    }
}
