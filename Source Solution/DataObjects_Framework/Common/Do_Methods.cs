using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Microsoft;
using Microsoft.VisualBasic;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.Base;
using System.Security.Cryptography;

namespace DataObjects_Framework.Common
{
    /// <summary>
    /// Common Methods used by the framework are defined here.
    /// </summary>
    public class Do_Methods
    {
        /// <summary>
        /// Checks the input objects and returns a default if it is a null, this includes for DBNull checking
        /// </summary>
        /// <param name="Obj_Input">
        /// The input object
        /// </param>
        /// <param name="Obj_NullOutput">
        /// The default output
        /// </param>
        /// <returns></returns>
        public static object IsNull(object Obj_Input, object Obj_NullOutput)
        {
            if (Obj_Input == null || Information.IsDBNull(Obj_Input))
            { return Obj_NullOutput; }
            else
            { return Obj_Input; }
        }

        /// <summary>
        /// Generates a character from the inputted number, such as 1:A, 26:Z, 27:A1, etc.
        /// </summary>
        /// <param name="Input">
        /// The number to be converted
        /// </param>
        /// <returns></returns>
        public static string GenerateChr(Int32 Input)
        {
            Int32 Ct = 0;
            Int32 TmpRes = 0;
            string OutputChr = "";

            while ((26 ^ Ct) < Input)
            { Ct++; }

            if (Ct > 0)
            { Ct--; }

            while (Input > 0)
            {
                TmpRes = Input / (26 ^ Ct);
                if ((Input % 26) == 0 || Ct > 0)
                { TmpRes -= Ct; }
                OutputChr = OutputChr + Strings.Chr(TmpRes + 64);
                Input -= ((26 ^ Ct) * TmpRes);
                Ct--;
            }

            return OutputChr;
        }

        /// <summary>
        /// Adds a new row to the specified data table with the supplied Str_Parameter List.
        /// </summary>
        /// <param name="Dt">
        /// The target data table
        /// </param>
        /// <param name="Sp">
        /// Contains the data to add
        /// </param>
        public static void AddDataRow(ref DataTable Dt, List<Common.Do_Constants.Str_Parameters> Sp)
        {
            DataRow Nr = Dt.NewRow();
            foreach (Common.Do_Constants.Str_Parameters Obj in Sp)
            { Nr[Obj.Name] = Obj.Value; }
            Dt.Rows.Add(Nr);
        }

        /// <summary>
        /// Adds a new row to the specified data table with the supplied fields and values.
        /// </summary>
        /// <param name="Dt">
        /// The target data table
        /// </param>
        /// <param name="Fields">
        /// String array for Fields
        /// </param>
        /// <param name="Values">
        /// Object array for Values, must contain the same number of elements as Fields
        /// </param>
        public static void AddDataRow(ref DataTable Dt, string[] Fields, object[] Values)
        {
            DataRow Nr = Dt.NewRow();
            for (Int32 Ct = 0; Ct <= (Fields.Length - 1); Ct++)
            { Nr[Fields[Ct]] = Values[Ct]; }
            Dt.Rows.Add(Nr);
        }

        /// <summary>
        /// Converts a data table to a 2 dimensional array
        /// </summary>
        /// <param name="Dt">
        /// The source data table
        /// </param>
        /// <param name="Fields">
        /// The field list to use, if null all of the fields will be used
        /// </param>
        /// <param name="RowStart">
        /// Conversion will start at the specified index
        /// </param>
        /// <param name="RowEnd">
        /// Conversion will end at the specified index
        /// </param>
        /// <returns>
        /// Returns a 2 dimensional array of objects
        /// </returns>
        public static object[,] ConvertDataTo2DimArray(
            DataTable Dt
            , string[] Fields
            , Int32 RowStart = -1
            , Int32 RowEnd = -1)
        {
            if (Dt.Rows.Count == 0)
            { return null; }

            Int32 i, ctr;            
            Int32 RowCt = 0;
            Int32 RowCtEnd = 0;

            if (RowStart > -1)
            { RowCt = RowStart; }
            else
            { RowStart = 0; }

            if (RowEnd > -1)
            { RowCtEnd = RowEnd; }
            else
            { RowCtEnd = Dt.Rows.Count - 1; }

            if (RowCt >= Dt.Rows.Count)
            { RowCt = Dt.Rows.Count - 1; }

            if (RowCtEnd >= Dt.Rows.Count)
            { RowCtEnd = Dt.Rows.Count - 1; }

            Int32 RowLength = RowCtEnd - RowCt;
            Int32 ColumnLength = 0;
            if (Fields != null)
            { ColumnLength = Fields.Length; }
            else
            { ColumnLength = Dt.Columns.Count; }

            object[,] ReturnValue = new object[RowLength,ColumnLength];

            while (RowCt <= RowCtEnd)
            {
                Int32 RV_RowCt = RowCt - RowStart;

                if (Fields != null)
                {
                    for (ctr = 0; ctr <= (Fields.Length - 1); ctr++)
                    {
                        Int32 RV_ColumnCt = ctr;
                        for (i = 0; i <= Dt.Columns.Count - 1; i++)
                        {
                            if (Fields[ctr].Trim() == "")
                            {
                                ReturnValue[RV_RowCt, RV_ColumnCt] = "";
                                break;
                            }
                            if (Fields[ctr].ToUpper() == Dt.Columns[i].ColumnName.ToUpper())
                            {
                                if (Dt.Rows[RowCt][i] == DBNull.Value)
                                { ReturnValue[RV_RowCt, RV_ColumnCt] = ""; }
                                else
                                { ReturnValue[RV_RowCt, RV_ColumnCt] = Dt.Rows[RowCt][i]; }
                            }
                        }
                    }
                }
                else
                {
                    for (i = 0; i <= Dt.Columns.Count - 1; i++)
                    {
                        Int32 RV_ColumnCt = i;
                        if (Dt.Rows[RowCt][i] == DBNull.Value)
                        { ReturnValue[RV_RowCt, RV_ColumnCt] = ""; }
                        else
                        { ReturnValue[RV_RowCt, RV_ColumnCt] = Dt.Rows[RowCt][i]; }
                    }
                }
                RowCt++;
            }
            return ReturnValue;
        }

        public static void LogWrite(string Msg, string FilePath)
        {
            Msg = "[" + DateTime.Now.ToString() + "] " + Msg;
            string FileName = "System Logs [" + DateTime.Now.ToString("yyyy.MM.dd") + "].log";
            TextWriter(FileName, FilePath, new string[] { Msg });
        }

        public static void TextWriter(string FileName, string Path, string[] StrLine)
        {
            if (!System.IO.Directory.Exists(Path))
            { System.IO.Directory.CreateDirectory(Path); }

            System.IO.FileStream FileStream;
            FileInfo Fi = new FileInfo(SetFolderPath(Path) + FileName);
            if (!Fi.Exists)
            { FileStream = Fi.Create(); }
            else
            { FileStream = Fi.Open(FileMode.Append, FileAccess.Write); }

            string Str = "";
            if (StrLine != null)
            { Str = string.Join(" ", StrLine); }

            System.IO.StreamWriter FileWriter = new StreamWriter(FileStream);
            FileWriter.WriteLine(Str);
            FileWriter.Flush();
            FileWriter.Close();
        }

        public static string SetFolderPath(string Path)
        {
            Path = Path.Trim();
            if(Strings.Right(Path,1) != @"\")
            { Path += @"\"; }
            return Path;
        }

        /// <summary>
        /// Converts the capitalization of all the fields the specified datarow to the specified case if it is a string data type.
        /// </summary>
        /// <param name="Dr">
        /// The target data row.
        /// </param>
        /// <param name="IsUpperCase">
        /// If true, converts to Upper Case, else to Lower Case.
        /// </param>
        public static void ConvertCaps(DataRow Dr, bool IsUpperCase = true)
        {
            foreach (DataColumn Dc in Dr.Table.Columns)
            {
                if (Dc.DataType.Name == typeof(string).Name)
                {
                    if (IsUpperCase) { Dr[Dc] = ((string)IsNull(Dr[Dc], "")).ToUpper(); }
                    else { Dr[Dc] = ((string)IsNull(Dr[Dc], "")).ToLower(); }
                }
            }
        }

        /// <summary>
        /// Converts the capitalization of all the fields of all the data rows of the specified data table to the specified case if it is a string data type.
        /// </summary>
        /// <param name="Dt">
        /// The target data table.
        /// </param>
        /// <param name="IsUpperCase">
        /// If true, converts to Upper Case, else to Lower Case.
        /// </param>
        public static void ConvertCaps(DataTable Dt, bool IsUpperCase = true)
        {
            foreach (DataRow Dr in Dt.Rows)
            {
                DataRow Inner_Dr = Dr;
                ConvertCaps(Inner_Dr, IsUpperCase);
            }
        }

        /// <summary>
        /// Converts an object to a double data type without exceptions.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static double Convert_Double(object Value, double DefaultValue = 0)
        {
            string ValueString = string.Empty;
            try
            { ValueString = Value.ToString(); }
            catch { }

            double ReturnValue;
            if (!double.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }

            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a Int32 data type without exceptions.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static Int32 Convert_Int32(object Value, Int32 DefaultValue = 0)
        {
            string ValueString = string.Empty;
            try
            { ValueString = Value.ToString(); }
            catch { }

            Int32 ReturnValue;
            if (!Int32.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; } 
            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a Int64 data type without exceptions.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static Int64 Convert_Int64(object Value, Int64 DefaultValue = 0)
        {
            string ValueString = string.Empty;
            try
            { ValueString = Value.ToString(); }
            catch { }

            Int64 ReturnValue;
            if (!Int64.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }
            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a Nullable Date Time data type without exceptions.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static DateTime? Convert_DateTime(object Value, DateTime? DefaultValue = null)
        {
            string ValueString = string.Empty;
            try
            { ValueString = Value.ToString(); }
            catch { }

            DateTime ReturnValue_Ex;
            DateTime? ReturnValue;
            if (DateTime.TryParse(ValueString, out ReturnValue_Ex))
            { ReturnValue = ReturnValue_Ex; }
            else
            { ReturnValue = null; }

            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a string data type without exceptions.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static string Convert_String(object Value, string DefaultValue = "")
        { return (string)IsNull(Value, DefaultValue); }

        /// <summary>
        /// Converts an object to a boolean data type without exceptions.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static bool Convert_Boolean(object Value, bool DefaultValue = false)
        { 
            string ValueString = string.Empty;
            try
            { ValueString = Value.ToString(); }
            catch { }

            bool ReturnValue;
            if (!bool.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }
            return ReturnValue;
        }

        public static string TextFiller(string TextInput , string Filler, Int32 TextLength)
        {
            string Rv = Strings.Right(Strings.StrDup(TextLength, Filler) + Strings.LTrim(Strings.Left(TextInput, TextLength)), TextLength);
            return Rv;
        }

        public static string PrepareFilterText(string Field, string DataType, string Filter)
        {
            string Rv = "";
            switch (DataType)
            { 
                case "String":
                    Rv = @"[" + Field + @"] LIKE '" + Filter + @"%'";
                    break;
                default:
                    string Tmp_Str = Filter;
                    if (ParseFilterText(ref Tmp_Str, DataType))
                    { Rv = @"[" + Field + @"] " + Filter + @""; }
                    break;
            }

            return Rv;
        }

        public static bool ParseFilterText(ref string FilterText, string DataType = "String")
        {
            string ParsedTextToken = "";
            string[] ArrParsedText = FilterText.Split(' ');

            for (Int32 Ct = 0; Ct <= (ArrParsedText.Length - 1); Ct++)
            {
                String Str = ArrParsedText[Ct];
                switch (Str)
                {
                    case ">": 
                    case "<":
                    case "=":
                    case "<=":
                    case ">=":
                    case "<>":
                        ParsedTextToken += "Boolean";
                        break;
                    default:
                        if (Information.IsNumeric(Str))
                        { ParsedTextToken += "Numeric"; }
                        else if (Information.IsDate(Str))
                        {
                            Str = @"'" + Strings.Format(Convert.ToDateTime(Str), "yyyy-MM-dd") + @"'";
                            ParsedTextToken += "DateTime";
                        }
                        else
                        { ParsedTextToken += "String"; }
                        break;
                }
            }

            FilterText = String.Join(" ", ArrParsedText);

            switch (ParsedTextToken)
            { 
                case "BooleanNumeric":
                    if (
                        DataType == typeof(Int16).Name
                        || DataType == typeof(Int32).Name
                        || DataType == typeof(Int64).Name
                        || DataType == typeof(decimal).Name
                        || DataType == typeof(double).Name
                        || DataType == typeof(Single).Name)
                    { return true; }
                    break;
                case "BooleanDateTime":
                    if (DataType == typeof(DateTime).Name)
                    { return true; }
                    break;
                case "Numeric":
                    if (
                        DataType == typeof(Int16).Name
                        || DataType == typeof(Int32).Name
                        || DataType == typeof(Int64).Name
                        || DataType == typeof(decimal).Name
                        || DataType == typeof(double).Name
                        || DataType == typeof(Single).Name)
                    { return true; }
                    break;
                case "DateTime":
                    if (DataType == typeof(DateTime).Name)
                    {
                        FilterText = " = " + FilterText;
                        return true; 
                    }
                    break;
                case "String":
                    if (DataType == typeof(string).Name)
                    {
                        FilterText = " = " + FilterText;
                        return true; 
                    }
                    break;
                default:
                    return false;
            }

            return false;
        }

        public static string GetMD5(string aString)
        {
            byte[] byteStr = Encoding.UTF8.GetBytes(aString);
            MD5 md5Provider = new MD5CryptoServiceProvider();

            return Encoding.UTF8.GetString(md5Provider.ComputeHash(byteStr));
        }

        public static DateTime GetServerDate(ClsConnection_SqlServer Da)
        {
            DataTable Dt = Da.ExecuteQuery("Select GetDate() As ServerDate").Tables[0];
            if (Dt.Rows.Count > 0) return (DateTime)Dt.Rows[0][0];
            else return DateTime.Now;
        }

        public static DateTime GetServerDate()
        {
            ClsConnection_SqlServer Da = new ClsConnection_SqlServer();
            try
            {
                Da.Connect();
                return GetServerDate(Da);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            { Da.Close(); }
        }

        //Removed
        //public static string GetSeriesNo(string Name)
        //{
        //    string Rv = "";
        //    DataTable Dt;
        //    string TableName;
        //    string FieldName;
        //    string Prefix;
        //    Int32 Digits;

        //    Dt = new ClsBase().pDa.GetQuery("System_DocumentSeries", "", "ModuleName = '" + Name + "'");
        //    if (Dt.Rows.Count > 0)
        //    {
        //        TableName = (string)Do_Methods.IsNull(Dt.Rows[0]["TableName"], "");
        //        FieldName = (string)Do_Methods.IsNull(Dt.Rows[0]["FieldName"], "");
        //        Prefix = (string)Do_Methods.IsNull(Dt.Rows[0]["Prefix"], "");
        //        Digits = (Int32)Do_Methods.IsNull(Dt.Rows[0]["Digits"], "");
        //    }
        //    else
        //    { return Rv; }

        //    List<DataObjects_Framework.Common.Do_Constants.Str_Parameters> Sp = new List<DataObjects_Framework.Common.Do_Constants.Str_Parameters>();
        //    Sp.Add(new DataObjects_Framework.Common.Do_Constants.Str_Parameters("@TableName", TableName));
        //    Sp.Add(new DataObjects_Framework.Common.Do_Constants.Str_Parameters("@FieldName", FieldName));
        //    Sp.Add(new DataObjects_Framework.Common.Do_Constants.Str_Parameters("@Prefix", Prefix));
        //    Sp.Add(new DataObjects_Framework.Common.Do_Constants.Str_Parameters("@Digits", Digits));

        //    Dt = new ClsConnection_SqlServer().ExecuteQuery("usp_GetSeriesNo", Sp).Tables[0];
        //    if (Dt.Rows.Count > 0)
        //    { Rv = (string)Dt.Rows[0][0]; }

        //    return Rv;
        //}

        //public static bool CheckSeriesDuplicate(
        //    string TableName
        //    , string SeriesField
        //    , ClsKeys Keys
        //    , string SeriesNo)
        //{
        //    bool Rv = false;
        //    DataTable Dt;

        //    System.Text.StringBuilder Sb_Query_Key = new StringBuilder();
        //    string Query_Key = "";
        //    string Query_And = "";

        //    foreach (string Inner_Key in Keys.pName)
        //    {
        //        Sb_Query_Key.Append(Query_And + " " + Inner_Key + " = " + Keys[Inner_Key]);
        //        Query_And = " And ";
        //    }

        //    Query_Key = " 1 = 1 ";
        //    if (Sb_Query_Key.ToString() != "")
        //    { Query_Key = "(Not (" + Sb_Query_Key.ToString() + "))"; }

        //    Dt = new ClsBase().pDa.GetQuery(
        //        "[" + TableName + "]"
        //        , "Count(1) As [Ct]"
        //        , Query_Key + " And " + SeriesField + " = '" + SeriesNo + "'");
        //    if (Dt.Rows.Count > 0)
        //    {
        //        if ((Int32)Dt.Rows[0][0] > 0)
        //        { Rv = true; }
        //    }

        //    //True means duplicates have been found
        //    return Rv;
        //}

    }
}
