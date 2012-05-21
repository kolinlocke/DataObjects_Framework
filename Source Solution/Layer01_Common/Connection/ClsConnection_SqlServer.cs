using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Layer01_Common.Common;
using Microsoft.VisualBasic;

namespace Layer01_Common.Connection
{
    public class ClsConnection_SqlServer: Interface_Connection
    {
        #region _Variables

        SqlConnection mConnection = null;
        SqlTransaction mTransaction = null;
        const int CnsQueryTimeout = 3000;

        enum eProcess : long
        {
            Process_Insert = 0
            , Process_Update = 1
            , Process_Delete = 2
        }

        #endregion

        #region _Methods

            #region _Connection
        
            public bool Connect()
            {
                string ConnectionString = "";

                /*
                StringBuilder Sb_ConnectionString = new StringBuilder();                
                string Server, Database, Username, Password;

                Server = Global_Variables.gConnection_Server;
                Database = Global_Variables.gConnection_Database;
                Username = Global_Variables.gConnection_Username;
                Password = Global_Variables.gConnection_Password;

                Sb_ConnectionString.Append("Server=" + Server + ";");
                Sb_ConnectionString.Append("Database=" + Database + ";");
                Sb_ConnectionString.Append("User ID=" + Username + ";");
                Sb_ConnectionString.Append("Password=" + Password + ";");
                Sb_ConnectionString.Append("Trusted_Connection=False;");
                
                ConnectionString = Sb_ConnectionString.ToString();
                */

				//Global_Variables.gDataba

                ConnectionString = Global_Variables.gConnection_SqlServerConnectionString;

                this.mConnection = new SqlConnection(ConnectionString);
                this.mConnection.Open();

                return true;
            }

            public void Close() 
            {
                try
                {
                    if (this.mConnection != null)
                    {
                        this.mConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (!Information.IsNothing(this.mTransaction)) 
                    {
                        this.mTransaction.Dispose();
                        this.mTransaction = null;
                    }
                    if (!Information.IsNothing(this.mConnection))
                    {
                        SqlConnection.ClearPool(this.mConnection);
                        this.mConnection.Dispose();
                        this.mConnection = null;
                    }
                }
            }

            public void BeginTransaction()
            {
                this.mTransaction = this.mConnection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            }

            public void CommitTransaction()
            {
                this.mTransaction.Commit();
            }

            public void RollbackTransaction()
            {
                this.mTransaction.Rollback();
            }

            #endregion

            #region _Query

            public int ExecuteNonQuery(string ProcedureName, List<Layer01_Common.Common.Layer01_Constants.Str_Parameters> ProcedureParameters)
            { return this.ExecuteNonQuery(ProcedureName, ProcedureParameters.ToArray()); }

            public int ExecuteNonQuery(string ProcedureName, Layer01_Common.Common.Layer01_Constants.Str_Parameters[] ProcedureParameters)
            {
                bool IsConnection = false;

                if (this.mConnection != null)
                {
                    if (this.mConnection.State == ConnectionState.Open)
                    { IsConnection = true; }
                }

                if (!IsConnection)
                { this.Connect(); }

                //[-]

                SqlCommand Cmd = new SqlCommand();
                int ReturnValue = 0;
                try
                {                    
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.CommandText = ProcedureName;
                    Cmd.CommandTimeout = CnsQueryTimeout;

                    if (!Information.IsNothing(ProcedureParameters))
                    {
                        foreach (Layer01_Common.Common.Layer01_Constants.Str_Parameters Inner_Obj in ProcedureParameters)
                        { Cmd.Parameters.AddWithValue(Inner_Obj.Name, Inner_Obj.Value); }
                    }

                    Cmd.Transaction = this.mTransaction;
                    Cmd.Connection = this.mConnection;
                    ReturnValue = Cmd.ExecuteNonQuery();
                }
                catch { }
                finally
                {
                    Cmd.Dispose();
                    Cmd = null;

                    if (!IsConnection)
                    { this.Close(); }
                }
                return ReturnValue;
            }

            public int ExecuteNonQuery(string Query)
            {
                bool IsConnection = false;

                if (this.mConnection != null)
                {
                    if (this.mConnection.State == ConnectionState.Open)
                    { IsConnection = true; }
                }

                if (!IsConnection)
                { this.Connect(); }

                //[-]

                SqlCommand Cmd = new SqlCommand(Query,this.mConnection);
                int ReturnValue = 0;
                try
                {
                    Cmd.CommandType = System.Data.CommandType.Text;
                    Cmd.Transaction = this.mTransaction;
                    ReturnValue = Cmd.ExecuteNonQuery();
                }
                catch { }
                finally
                {
                    Cmd.Dispose();
                    Cmd = null;

                    if (!IsConnection)
                    { this.Close(); }
                }
                return ReturnValue;
            }

            public DataSet ExecuteQuery(string ProcedureName, List<Layer01_Common.Common.Layer01_Constants.Str_Parameters> ProcedureParameters)
            {
                return this.ExecuteQuery(ProcedureName, ProcedureParameters.ToArray());
            }

            public DataSet ExecuteQuery(string ProcedureName, Layer01_Common.Common.Layer01_Constants.Str_Parameters[] ProcedureParameters)
            {
                bool IsConnection = false;

                if (this.mConnection != null)
                {
                    if (this.mConnection.State == ConnectionState.Open)
                    { IsConnection = true; }
                }

                if (!IsConnection)
                { this.Connect(); }

                //[-]

                SqlCommand Cmd = new SqlCommand();
                SqlDataAdapter Adp = new SqlDataAdapter();
                DataSet Ds = new DataSet();
                try
                {
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.CommandText = ProcedureName;
                    Cmd.CommandTimeout = CnsQueryTimeout;

                    if (!Information.IsNothing(ProcedureParameters))
                    {
                        foreach (Layer01_Common.Common.Layer01_Constants.Str_Parameters Inner_Obj in ProcedureParameters)
                        { Cmd.Parameters.AddWithValue(Inner_Obj.Name, Inner_Obj.Value); }
                    }

                    Cmd.Transaction = this.mTransaction;
                    Cmd.Connection = this.mConnection;
                    Adp.SelectCommand = Cmd;
                    Adp.Fill(Ds);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                {
                    Cmd.Dispose();
                    Cmd = null;
                    Adp.Dispose();
                    Adp = null;

                    if (!IsConnection)
                    { this.Close(); }
                }
                
                return Ds;
            }

            public DataSet ExecuteQuery(string Query)
            {
                bool IsConnection = false;

                if (this.mConnection != null)
                {
                    if (this.mConnection.State == ConnectionState.Open)
                    { IsConnection = true; }
                }

                if (!IsConnection)
                { this.Connect(); }

                //[-]

                SqlCommand Cmd = new SqlCommand();
                SqlDataAdapter Adp = new SqlDataAdapter();
                DataSet Ds = new DataSet();
                try
                {
                    Cmd.CommandType = System.Data.CommandType.Text;
                    Cmd.CommandText = Query;
                    Cmd.CommandTimeout = CnsQueryTimeout;
                    Cmd.Transaction = this.mTransaction;
                    Cmd.Connection = this.mConnection;
                    Adp.SelectCommand = Cmd;
                    Adp.Fill(Ds);
                }
                catch (Exception Ex)
                { throw Ex; }
                finally
                {
                    Cmd.Dispose();
                    Cmd = null;
                    Adp.Dispose();
                    Adp = null;

                    if (!IsConnection)
                    { this.Close(); }
                }

                return Ds;
            }

            #endregion

            #region _Save

            public bool SaveDataRow(DataRow ObjDataRow, string TableName)
            { return this.SaveDataRow(ObjDataRow, TableName, ""); }

            public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName)
            { return this.SaveDataRow(ObjDataRow, TableName, SchemaName, false); }

            public bool SaveDataRow(DataRow ObjDataRow, string TableName, string SchemaName, bool IsDelete)
            {
                bool Cns_IsSoftDelete = true;

                //[-]

                if (SchemaName == "")
                { SchemaName = "dbo"; }

                eProcess cProcess = eProcess.Process_Insert;
                DataTable Dt_TableDef = new DataTable(TableName);
                DataTable Dt_Def;
                List<Layer01_Common.Common.Layer01_Constants.Str_Parameters> List_Param;

                //[Get Table Definition]
                List_Param = new List<Layer01_Common.Common.Layer01_Constants.Str_Parameters>();
                List_Param.Add(new Layer01_Common.Common.Layer01_Constants.Str_Parameters(@"@TableName", TableName));
                List_Param.Add(new Layer01_Common.Common.Layer01_Constants.Str_Parameters(@"@SchemaName", SchemaName));

                Dt_Def = this.ExecuteQuery("usp_GetTableDef", List_Param).Tables[0];
                foreach (DataRow Inner_Dr in Dt_Def.Rows)
                {
                    System.Type Inner_Type = null;
                    switch (Layer01_Methods.IsNull(Inner_Dr["DataType"], "").ToString().ToLower())
                    {
                        case "tinyint":
                            Inner_Type = typeof(System.Byte);
                            break;
                        case "smallint":
                            Inner_Type = typeof(System.Int16);
                            break;
                        case "int":
                            Inner_Type = typeof(System.Int32);
                            break;
                        case "bigint":
                            Inner_Type = typeof(System.Int64);
                            break;
                        case "bit":
                            Inner_Type = typeof(System.Boolean);
                            break;
                        case "decimal":
                        case "numeric":
                            Inner_Type = typeof(System.Double);
                            break;
                        case "datetime":
                        case "smalldatetime":
                            Inner_Type = typeof(System.DateTime);
                            break;
                        case "char":
                        case "varchar":
                        case "text":
                        case "nchar":
                        case "nvarchar":
                        case "ntext":
                            Inner_Type = typeof(System.String);
                            break;
                    }
                    Dt_TableDef.Columns.Add((string)Layer01_Methods.IsNull(Inner_Dr["ColumnName"], ""), Inner_Type);
                }

                //[Check ObjDataRow Fields for PK Data]

                bool IsFound = false;
                Int32 PKsCt = 0;
                Int32 PKsFoundCt = 0;

                DataRow[] ArrDr_Dt_Def;
                ArrDr_Dt_Def = Dt_Def.Select("IsPK = 1");
                PKsCt = ArrDr_Dt_Def.Length;

                foreach (DataRow Inner_Dr in ArrDr_Dt_Def)
                {
                    foreach (DataColumn Inner_Dc in ObjDataRow.Table.Columns)
                    {
                        if ((string)Inner_Dr["ColumnName"] == Inner_Dc.ColumnName)
                        {
                            if (Convert.ToInt64(Layer01_Methods.IsNull(ObjDataRow[Inner_Dc.ColumnName], 0)) != 0)
                            {
                                PKsFoundCt++;
                                if (PKsFoundCt >= PKsCt) break;
                            }
                        }
                    }
                }

                //Check Process
                if (PKsFoundCt != PKsCt)
                {
                    cProcess = eProcess.Process_Insert;
                    DataRow[] ArrDr_Dt_Def_Pks = Dt_Def.Select(@"IsPK = 1 And IsIdentity = 0");
                    foreach (DataRow Inner_Dr in ArrDr_Dt_Def_Pks)
                    {
                        //Check PK if there is already a value
                        //If there is, continue the loop
                        string Inner_ColumnName = (string)Layer01_Methods.IsNull(Inner_Dr["ColumnName"], "");
                        if (Convert.ToInt64(Layer01_Methods.IsNull(ObjDataRow[Inner_ColumnName], 0)) != 0)
                        { continue; }

                        ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
                        try
                        {
                            Da.Connect();
                            Da.BeginTransaction();

                            Int64 NewID;
                            List_Param = new List<Layer01_Common.Common.Layer01_Constants.Str_Parameters>();
                            List_Param.Add(new Layer01_Common.Common.Layer01_Constants.Str_Parameters(@"@TableName", TableName + "." + Inner_ColumnName));
                            NewID = (Int64)Layer01_Methods.IsNull(Da.ExecuteQuery("usp_GetNextID", List_Param).Tables[0].Rows[0][0], 0);
                            ObjDataRow[Inner_ColumnName] = NewID;

                            Da.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            Da.RollbackTransaction();
                            throw ex;
                        }
                        finally
                        {
                            Da.Close();
                            Da = null;
                        }
                    }
                }
                else
                {
                    //Check if Row to be updated has rows to be updated
                    //If none the return the function true
                    DataRow[] Inner_ArrDr = Dt_Def.Select(@"IsPk = 0 And IsIdentity = 0");
                    if (Inner_ArrDr.Length == 0) return true;
                    cProcess = eProcess.Process_Update;
                }

                if (IsDelete) 
                {
                    if (cProcess == eProcess.Process_Update)
                    {
                        if (!Cns_IsSoftDelete)
                        { cProcess = eProcess.Process_Delete; }
                        else
                        { ObjDataRow["IsDeleted"] = true; }
                    }
                    else
                    { return true; }
                }
                

                //Prepare SQL Statement
                ClsPreparedQuery Pq = new ClsPreparedQuery(this);

                string Query_InsertFields = "";
                string Query_InsertFieldsValues = "";
                string Query_UpdateFields = "";
                string Query_Comma = "";

                foreach (DataColumn Dc_ObjDataRow in ObjDataRow.Table.Columns)
                {
                    IsFound = false;
                    foreach (DataColumn Dc_TableDef in Dt_TableDef.Columns)
                    {
                        if (Dc_ObjDataRow.ColumnName.ToLower() == Dc_TableDef.ColumnName.ToLower())
                        {
                            switch (cProcess)
                            {
                                case eProcess.Process_Insert:
                                    IsFound = true;
                                    break;
                                case eProcess.Process_Update:
                                    DataRow[] Inner_ArrDr = Dt_Def.Select(@"ColumnName = '" + Dc_ObjDataRow.ColumnName + "' And IsPk = 1");
                                    if (Inner_ArrDr.Length == 0) IsFound = true;
                                    break;
                            }
                            if (IsFound) break;
                        }
                    }

                    if (IsFound)
                    {
                        DataRow[] Inner_ArrDr_Def = Dt_Def.Select(@"ColumnName = '" + Dc_ObjDataRow.ColumnName + "'" + " And IsIdentity = 0");
                        if (Inner_ArrDr_Def.Length == 0) continue;

                        switch (cProcess)
                        {
                            case eProcess.Process_Insert:
                                Query_InsertFields += " " + Query_Comma + @" [" + Dc_ObjDataRow.ColumnName + @"] ";
                                Query_InsertFieldsValues += " " + Query_Comma + @" @" + Dc_ObjDataRow.ColumnName.Replace(@" ", @"_") + " ";
                                break;
                            case eProcess.Process_Update:
                                Query_UpdateFields += " " + Query_Comma + @" [" + Dc_ObjDataRow.ColumnName + @"] = @" + Dc_ObjDataRow.ColumnName.Replace(@" ", @"_") + " ";
                                break;
                        }

                        Query_Comma = ",";

                        SqlParameter Inner_Sp = new SqlParameter(@"@" + Dc_ObjDataRow.ColumnName.Replace(" ", "_"), this.SqlDataTypeLib((string)Inner_ArrDr_Def[0]["DataType"]), Convert.ToInt32(Inner_ArrDr_Def[0]["Length"]));
                        Inner_Sp.Direction = ParameterDirection.Input;
                        Inner_Sp.Precision = (byte)Inner_ArrDr_Def[0]["Precision"];
                        Inner_Sp.Scale = (byte)Inner_ArrDr_Def[0]["Scale"];
                        Pq.pParameters.Add(Inner_Sp);
                    }
                }

                DataRow[] Inner_ArrDr_Pk;

                switch (cProcess)
                {
                    case eProcess.Process_Insert:
                        StringBuilder Sb_Query_Output = new StringBuilder();
                        StringBuilder Sb_Query_Output_Table = new StringBuilder();
                        string Query_Output = "";
                        char Query_Output_Comma = ' ';
                        string Query_Output_Table = "";
                        string Query_Output_Table_Select = "";

                        Inner_ArrDr_Pk = Dt_Def.Select("IsPK = 1 And IsIdentity = 1");
                        foreach (DataRow Inner_Dr in Inner_ArrDr_Pk)
                        {
                            Sb_Query_Output.Append(@" " + Query_Output_Comma + @" Inserted.[" + (string)Layer01_Methods.IsNull(Inner_Dr["ColumnName"], "") + @"] Into @Tb");
                            Sb_Query_Output_Table.Append(@" " + Query_Output_Comma + @" [" + (string)Layer01_Methods.IsNull(Inner_Dr["ColumnName"], "") + @"] " + (string)Layer01_Methods.IsNull(Inner_Dr["DataType"], ""));
                            Query_Output_Comma = ',';
                        }

                        Query_Output = Sb_Query_Output.ToString();
                        if (Query_Output.Trim() != "") Query_Output = " Output " + Query_Output;

                        Query_Output_Table = Sb_Query_Output_Table.ToString();
                        if (Query_Output_Table.Trim() != "")
                        {
                            Query_Output_Table = @" Declare @Tb As Table (" + Query_Output_Table + @"); ";
                            Query_Output_Table_Select = " Select * From @Tb ";
                        }

                        if (Query_InsertFields != "")
                        {
                            Query_InsertFields = "(" + Query_InsertFields + ")";
                            Query_InsertFieldsValues = " Values (" + Query_InsertFieldsValues + ") ";
                        }
                        else
                        {
                            //This path will be reached if the table to be inserted has only one field that is an identity field
                            Query_InsertFieldsValues = " Default Values ";
                        }

                        Pq.pQuery = Query_Output_Table + " Insert Into [" + SchemaName + "].[" + TableName + "] " + Query_InsertFields + " " + Query_Output + " " + Query_InsertFieldsValues + "; " + Query_Output_Table_Select;
                        break;
                    case eProcess.Process_Update:
                        string Query_UpdateCriteria = "";
                        Query_Comma = "";

                        Inner_ArrDr_Pk = Dt_Def.Select("IsPk = 1");
                        foreach (DataRow Inner_Dr in Inner_ArrDr_Pk)
                        {
                            DataRow[] Inner_ArrDr_TableDef = Dt_Def.Select(@"ColumnName = '" + (string)Inner_Dr["ColumnName"] + @"'");
                            Query_UpdateCriteria += " " + Query_Comma + " [" + Inner_Dr["ColumnName"] + "] = @" + ((string)Inner_Dr["ColumnName"]).Replace(" ", "_") + " ";
                            Query_Comma = "And";

                            SqlParameter Inner_Sp = new SqlParameter("@" + ((string)Inner_Dr["ColumnName"]).Replace(" ", "_"), this.SqlDataTypeLib((string)Inner_Dr["DataType"]), Convert.ToInt32(Inner_Dr["Length"]));
                            Inner_Sp.Direction = ParameterDirection.Input;
                            Inner_Sp.Precision = (byte)Inner_Dr["Precision"];
                            Inner_Sp.Scale = (byte)Inner_Dr["Scale"];
                            Pq.pParameters.Add(Inner_Sp);
                        }

                        Pq.pQuery = "Update [" + SchemaName + "].[" + TableName + "] Set " + Query_UpdateFields + " Where " + Query_UpdateCriteria;
                        break;
                    case eProcess.Process_Delete:
                        string Query_DeleteCriteria = "";
                        Query_Comma = "";

                        Inner_ArrDr_Pk = Dt_Def.Select("IsPk = 1");
                        foreach (DataRow Inner_Dr in Inner_ArrDr_Pk)
                        {
                            DataRow[] Inner_ArrDr_TableDef = Dt_Def.Select(@"ColumnName = '" + (string)Inner_Dr["ColumnName"] + @"'");
                            Query_DeleteCriteria += " " + Query_Comma + " [" + Inner_Dr["ColumnName"] + "] = @" + ((string)Inner_Dr["ColumnName"]).Replace(" ", "_") + " ";
                            Query_Comma = "And";

                            SqlParameter Inner_Sp = new SqlParameter("@" + ((string)Inner_Dr["ColumnName"]).Replace(" ", "_"), this.SqlDataTypeLib((string)Inner_Dr["DataType"]), Convert.ToInt32(Inner_Dr["Length"]));
                            Inner_Sp.Direction = ParameterDirection.Input;
                            Inner_Sp.Precision = (byte)Inner_Dr["Precision"];
                            Inner_Sp.Scale = (byte)Inner_Dr["Scale"];
                            Pq.pParameters.Add(Inner_Sp);
                        }

                        Pq.pQuery = "Delete [" + SchemaName + "].[" + TableName + "] Where " + Query_DeleteCriteria;
                        break;
                }

                Pq.Prepare();

                foreach (DataColumn Dc_ObjDataRow in ObjDataRow.Table.Columns)
                {
                    foreach (SqlParameter Inner_Sp in Pq.pParameters)
                    {
                        if ("@" + Dc_ObjDataRow.ColumnName.Replace(" ", "_") == Inner_Sp.ParameterName)
                        {
                            if (Information.IsDBNull(Dc_ObjDataRow)) Inner_Sp.Value = DBNull.Value;
                            else Inner_Sp.Value = this.SqlConvertDataType(ObjDataRow[Dc_ObjDataRow], Inner_Sp.SqlDbType.ToString());
                            continue;
                        }
                    }
                }

                DataSet Ds_Output;
                DataTable Dt_Output;

                Ds_Output = Pq.ExecuteQuery();
                if (Ds_Output.Tables.Count > 0)
                {
                    Dt_Output = Ds_Output.Tables[0];
                    foreach (DataColumn Inner_Dc in Dt_Output.Columns)
                    {
                        ObjDataRow[Inner_Dc.ColumnName] = Dt_Output.Rows[0][Inner_Dc.ColumnName];
                    }
                }
                return true;
            }
            
            #endregion

            #region _Libs

            SqlDbType SqlDataTypeLib(string DataType)
            {

                foreach (var Value in Enum.GetValues(typeof(SqlDbType)))
                { 
                    if (Value.ToString().ToLower() == DataType.ToLower()) return (SqlDbType)Value;
                }

                if ("numeric" == DataType) return SqlDbType.Decimal;

                return SqlDbType.Variant;
            }

            Object SqlConvertDataType(Object InputObj, string DataType)
            {
                try
                {
                    DataType = DataType.ToLower();

                    if (DataType == SqlDbType.BigInt.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return Convert.ToInt64(InputObj);
                        else return DBNull.Value;
                    }
                    if (DataType == SqlDbType.Bit.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return (bool)InputObj;
                        else return DBNull.Value;
                    }                       
                    if (
                        (DataType == SqlDbType.Char.ToString().ToLower())
                        || (DataType == SqlDbType.NChar.ToString().ToLower())
                        || (DataType == SqlDbType.NText.ToString().ToLower())
                        || (DataType == SqlDbType.NVarChar.ToString().ToLower())
                        || (DataType == SqlDbType.Text.ToString().ToLower())
                        || (DataType == SqlDbType.VarChar.ToString().ToLower())
                        )
                    {
                        if (InputObj != DBNull.Value) return (string)InputObj;
                        else return DBNull.Value;
                    }
                    if (
                        (DataType == SqlDbType.Date.ToString().ToLower())
                        || (DataType == SqlDbType.DateTime.ToString().ToLower())
                        || (DataType == SqlDbType.DateTime2.ToString().ToLower())
                        || (DataType == SqlDbType.SmallDateTime.ToString().ToLower())
                        || (DataType == SqlDbType.Timestamp.ToString().ToLower())
                        )
                    {
                        if (InputObj != DBNull.Value)
                        {
                            if (
                                (DateTime)InputObj >= System.Data.SqlTypes.SqlDateTime.MinValue.Value
                                 && (DateTime)InputObj <= System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                            { return (DateTime)InputObj; }
                            else
                            { return DBNull.Value; }
                        }
                        else
                        { return DBNull.Value; }
                    }
                    if (
                        (DataType == SqlDbType.Image.ToString().ToLower())
                        || (DataType == SqlDbType.Binary.ToString().ToLower())
                        )
                    {
                        if (InputObj != DBNull.Value) return (byte[])InputObj;
                        else return DBNull.Value;
                    }
                    if (
                        (DataType == SqlDbType.Money.ToString().ToLower())
                        || (DataType == SqlDbType.SmallMoney.ToString().ToLower())
                        || (DataType == SqlDbType.Decimal.ToString().ToLower())
                        )
                    {
                        if (InputObj != DBNull.Value) return Convert.ToDecimal(InputObj);
                        else return DBNull.Value;
                    }
                    if (DataType == SqlDbType.Real.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return Convert.ToSingle(InputObj);
                        else return DBNull.Value;
                    }                        
                    if (DataType == SqlDbType.SmallInt.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return Convert.ToInt16(InputObj);
                        else return DBNull.Value;
                    }
                    if (DataType == SqlDbType.TinyInt.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return (byte)InputObj;
                        else return DBNull.Value;
                    }
                    if (DataType == SqlDbType.Float.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return Convert.ToDouble(InputObj);
                        else return DBNull.Value;
                    }
                    if (DataType == SqlDbType.Variant.ToString().ToLower())
                    {
                        if (InputObj != DBNull.Value) return InputObj;
                        else return DBNull.Value;
                    }
                    return InputObj;
                }
                catch
                { return InputObj; }
            }

            #endregion

        #endregion

        #region _Properties
        
        public Object pConnection 
        {
            get { return this.mConnection; }
        }

        public IDbTransaction pTransaction
        {
            get { return this.mTransaction; }
        }
        
        #endregion    
    }
}
