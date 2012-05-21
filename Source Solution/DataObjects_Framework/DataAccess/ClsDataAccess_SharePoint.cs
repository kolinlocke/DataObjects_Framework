using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Layer01_Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;
using Layer01_Common.Common;
using Layer01_Common_IdeSeg;
using Layer01_Common_IdeSeg.SharePoint;
using Layer01_Common_IdeSeg.SharePoint.Caml;
using Layer01_Common_IdeSeg.SharePoint.Caml.QueryParser;
using Layer01_Common_IdeSeg.SharePoint.Caml.QueryParser.AST;
using Layer01_Common_IdeSeg.SharePoint.Caml.QueryParser.AST.Base;
using Layer01_Common_IdeSeg.SharePoint.Caml.QueryParser.AST.CAML;
using Layer01_Common_IdeSeg.SharePoint.Caml.QueryParser.LexScanner;
using Layer01_Common_IdeSeg.SharePoint.Caml.QueryParser.Parser;
using Layer02_Objects;
using Layer02_Objects.DataAccess;
using Layer02_Objects.Modules_Objects;
using Layer02_Objects._System;

namespace Layer02_Objects.DataAccess
{
    public class ClsDataAccess_SharePoint: Interface_DataAccess
    {
        #region _Variables

        ClsConnection_SharePoint mConnection;

        #endregion

        #region _ImpelementedMethods
        
        public DataTable GetQuery(Interface_Connection Connection, string ViewObject, string Fields = "", string Condition = "", string Sort = "")
        {
            ClsConnection_SharePoint Cn = (ClsConnection_SharePoint)Connection;
            DataTable Dt = Cn.GetData(ViewObject, ClsDataAccess_SharePoint.ConvertToCAML(Condition));
            return Dt;
        }

        public DataTable GetQuery(string ViewObject, string Fields = "", string Condition = "", string Sort = "")
        {
            ClsConnection_SharePoint Cn = new ClsConnection_SharePoint();
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

        public DataTable GetQuery(Layer01_Common.Connection.Interface_Connection Connection, string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            ClsQueryCondition_SharePoint Qc_Sp = (ClsQueryCondition_SharePoint)Condition;
            ClsConnection_SharePoint Cn = (ClsConnection_SharePoint)Connection;
            DataTable Dt = Cn.GetData(ViewObject, ClsDataAccess_SharePoint.ConvertToCAML(Qc_Sp.GetQueryCondition()));
            return Dt;
        }

        public DataTable GetQuery(string ViewObject, string Fields, ClsQueryCondition Condition, string Sort = "", long Top = 0, int Page = 0)
        {
            ClsConnection_SharePoint Cn = new ClsConnection_SharePoint();
            try
            {
                Cn.Connect();
                return this.GetQuery(Cn, ViewObject, Fields, Condition, Sort, Top, Page);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Cn.Close(); }
        }

        public Layer01_Common.Connection.Interface_Connection Connection
        {
            get { return this.mConnection; }
        }

        public void Connect()
        { 
            this.mConnection = new ClsConnection_SharePoint();
            this.mConnection.Connect();
        }

        public void Close()
        { this.mConnection.Close(); }

        public void BeginTransaction()
        { }

        public void CommitTransaction()
        { }

        public void RollbackTransaction()
        { }

        public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName = "", bool IsDelete = false)
        {
            this.mConnection.SaveData(TableName, ObjDataRow);
            return true;
        }
        
        public DataTable List(string ObjectName, string Condition = "", string Sort = "")
        {
            DataTable Dt = this.GetQuery(ObjectName, "", Condition, Sort);
            return Dt;
        }

        public DataTable List(string ObjectName, ClsQueryCondition Condition, string Sort = "", int Top = 0, int Page = 0)
        {
            DataTable Dt = this.GetQuery(ObjectName, "", Condition, Sort, Top, Page);
            return Dt;
        }

        public long List_Count(string ObjectName, ClsQueryCondition Condition = null)
        {
            throw new NotImplementedException();
        }

        public DataTable List_Empty(Interface_Connection Connection, string ObjectName)
        {
            ClsConnection_SharePoint Cn = (ClsConnection_SharePoint)Connection;
            return Cn.GetData_Empty(ObjectName);
        }

        public DataTable List_Empty(string ObjectName)
        {
            ClsConnection_SharePoint Cn = new ClsConnection_SharePoint();
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
            { Dr = this.List_Empty(ObjectName).NewRow(); }
            else
            {
                string Condition = "";
                if (Keys.Count() > 0)
                { Condition = @"ID = " + Keys[0]; }

                Dt = this.GetQuery(this.Connection, ObjectName, "", Condition.ToString());
                Dr = Dt.Rows[0];
            }
            return Dr;
        }

        public DataTable Load_TableDetails(string ObjectName, ClsKeys Keys, string Condition)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            DataTable Dt;

            if (Keys == null)
            { Dt = this.List_Empty(ObjectName); }
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

                Dt = this.GetQuery(this.Connection, ObjectName, "", Sb_Condition.ToString() + OtherCondition);
            }
            return Dt;
        }

        public DataRow Load_RowDetails(string ObjectName, ClsKeys Keys, string Condition)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            DataTable Dt;
            DataRow Dr;

            if (Keys == null)
            { Dr = this.List_Empty(ObjectName).NewRow(); }
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

                Dt = this.GetQuery(this.Connection, ObjectName, "", Sb_Condition.ToString() + OtherCondition);
                if (Dt.Rows.Count > 0) Dr = Dt.Rows[0];
                else Dr = Dt.NewRow();
            }
            return Dr;
        }

        public ClsQueryCondition CreateQueryCondition()
        { return new ClsQueryCondition_SharePoint(); }

        public DataTable GetTableDef(string TableName)
        {
            DataTable Rv = null;
            ClsConnection_SharePoint Cn = new ClsConnection_SharePoint();
            try
            {
                Cn.Connect();
                Rv = Cn.GetTableDef(TableName);
            }
            catch { }
            finally
            { Cn.Close(); }
            return Rv;
        }

        public string GetSystemParameter(string ParameterName, string DefaultValue = "")
        {
            throw new NotImplementedException();
        }

        public string GetSystemParameter(Interface_Connection Connection, string ParameterName, string DefaultValue = "")
        {
            throw new NotImplementedException();
        }

        public void SetSystemParameter(string ParameterName, string ParameterValue)
        {
            throw new NotImplementedException();
        }

        public void SetSystemParameter(Interface_Connection Connection, string ParameterName, string ParameterValue)
        {
            throw new NotImplementedException();
        }

        public void AddSelected(DataTable Dt_Target, List<long> Selected_IDs, string Selected_DataSourceName, string Selected_KeyName, string Target_Key, bool HasTmpKey = false, List<Layer01_Constants.Str_AddSelectedFields> List_Selected_Fields = null, List<Layer01_Constants.Str_AddSelectedFieldsDefault> List_Selected_FieldsDefault = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region _Methods

        public static CamlQuery ConvertToCAML(string Condition, Int64 Top = 0)
        {
            Condition = "Where " + Condition.Replace(@"'", @"""");

            NParser parser = new NParser(Condition, new ASTNodeCAMLFactory());
            string Parsed = "";

            try
            {
                var generator = new CodeGenerator(parser.Parse());
                generator.Generate();
                Parsed = generator.Code;
            }
            catch { }

            string RowLimit = "";
            if (Top > 0)
            { RowLimit = @"<RowLimit Paged='False'>" + Top + @"</RowLimit>"; }

            CamlQuery Query = new CamlQuery();
            Query.ViewXml = @"<View>" + Parsed + RowLimit + @"</View>";
            return Query;
        }

        #endregion        
    }
}
