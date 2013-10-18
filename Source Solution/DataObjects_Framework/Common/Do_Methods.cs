using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft;
using Microsoft.VisualBasic;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.BaseObjects;
using DataObjects_Framework.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;
using DataObjects_Framework.PreparedQueryObjects;

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
        public static void AddDataRow(ref DataTable Dt, List<QueryParameter> Sp)
        {
            DataRow Nr = Dt.NewRow();
            foreach (QueryParameter Obj in Sp)
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

            object[,] ReturnValue = new object[RowLength, ColumnLength];

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
            if (Strings.Right(Path, 1) != @"\")
            { Path += @"\"; }
            return Path;
        }

        public static void ConvertCaps(DataRow Dr)
        { ConvertCaps(Dr, true); }

        /// <summary>
        /// Converts the capitalization of all the fields the specified datarow to the specified case if it is a string data type.
        /// </summary>
        /// <param name="Dr">
        /// The target data row.
        /// </param>
        /// <param name="IsUpperCase">
        /// If true, converts to Upper Case, else to Lower Case.
        /// </param>
        public static void ConvertCaps(DataRow Dr, bool IsUpperCase)
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

        public static void ConvertCaps(DataTable Dt)
        { ConvertCaps(Dt, true); }

        /// <summary>
        /// Converts the capitalization of all the fields of all the data rows of the specified data table to the specified case if it is a string data type.
        /// </summary>
        /// <param name="Dt">
        /// The target data table.
        /// </param>
        /// <param name="IsUpperCase">
        /// If true, converts to Upper Case, else to Lower Case.
        /// </param>
        public static void ConvertCaps(DataTable Dt, bool IsUpperCase)
        {
            foreach (DataRow Dr in Dt.Rows)
            {
                DataRow Inner_Dr = Dr;
                ConvertCaps(Inner_Dr, IsUpperCase);
            }
        }

        /// <summary>
        /// Converts an object to a double data type without exceptions.
        /// If conversion fails, returns 0.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <returns></returns>
        public static double Convert_Double(object Value)
        { return Convert_Double(Value, 0); }

        /// <summary>
        /// Converts an object to a double data type without exceptions.
        /// If conversion fails, returns the specified default value.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static double Convert_Double(Object Value, double DefaultValue)
        {
            string ValueString = string.Empty;
            if (Value != null)
            {
                try { ValueString = Value.ToString(); }
                catch { }
            }

            double ReturnValue;
            if (!double.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }

            return ReturnValue;
        }

        public static Int16 Convert_Int16(Object Value)
        { return Convert_Int16(Value, 0); }

        public static Int16 Convert_Int16(Object Value, Int16 DefaultValue)
        {
            String ValueString = Convert_String(Value);

            Int16 ReturnValue;
            if (!Int16.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }
            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a Int32 data type without exceptions.
        /// If conversion fails, returns 0.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <returns></returns>
        public static Int32 Convert_Int32(Object Value)
        { return Convert_Int32(Value, 0); }

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
        public static Int32 Convert_Int32(Object Value, Int32 DefaultValue)
        {
            String ValueString = Convert_String(Value);

            Int32 ReturnValue;
            if (!Int32.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }
            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a Int64 data type without exceptions.
        /// If conversion fails, returns 0.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <returns></returns>
        public static Int64 Convert_Int64(Object Value)
        { return Convert_Int64(Value, 0); }

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
        public static Int64 Convert_Int64(Object Value, Int64 DefaultValue)
        {
            String ValueString = Convert_String(Value);

            Int64 ReturnValue;
            if (!Int64.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }
            return ReturnValue;
        }

        public static DateTime? Convert_DateTime(Object Value)
        { return Convert_DateTime(Value, null); }

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
        public static DateTime? Convert_DateTime(Object Value, DateTime? DefaultValue)
        {
            string ValueString = string.Empty;
            if (Value != null)
            {
                try { ValueString = Value.ToString(); }
                catch { }
            }

            DateTime ReturnValue_Ex;
            DateTime? ReturnValue;
            if (DateTime.TryParse(ValueString, out ReturnValue_Ex))
            { ReturnValue = ReturnValue_Ex; }
            else
            { ReturnValue = DefaultValue; }

            return ReturnValue;
        }

        public static String Convert_String(Object Value)
        { return Convert_String(Value, ""); }

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
        public static String Convert_String(Object Value, String DefaultValue)
        { return Convert.ToString(IsNull(Value, DefaultValue)); }

        /// <summary>
        /// Converts an object to a boolean data type without exceptions.
        /// If conversion fails, returns false.
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Boolean Convert_Boolean(Object Value)
        { return Convert_Boolean(Value, false); }

        /// <summary>
        /// Converts an object to a boolean data type without exceptions.
        /// If conversion fails, returns the specified default value.
        /// </summary>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns></returns>
        public static bool Convert_Boolean(Object Value, bool DefaultValue)
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

        public static Byte Convert_Byte(Object Value)
        { return Convert_Byte(Value, 0); }

        public static Byte Convert_Byte(Object Value, Byte DefaultValue)
        {
            string ValueString = string.Empty;
            try
            { ValueString = Value.ToString(); }
            catch { }

            Byte ReturnValue;
            if (!Byte.TryParse(ValueString, out ReturnValue))
            { ReturnValue = DefaultValue; }
            return ReturnValue;
        }

        /// <summary>
        /// Converts an object to a specified data type.
        /// If conversion fails, returns the default value.
        /// </summary>
        /// <typeparam name="T">
        /// Accepts Int16, Int32, Int64, Decimal, Float, Double, DateTime, Boolean, Byte.
        /// </typeparam>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <returns>
        /// Returns the converted value according to specified type.
        /// </returns>
        public static T Convert_Value<T>(Object Value)
        {
            dynamic Dynamic_DefaultValue;

            if (
                (typeof(T) == typeof(Int16) || typeof(T) == typeof(Int16?))
                || (typeof(T) == typeof(Int32) || typeof(T) == typeof(Int32?))
                || (typeof(T) == typeof(Int64) || typeof(T) == typeof(Int64?))
                || (typeof(T) == typeof(Decimal) || typeof(T) == typeof(Decimal?))
                || (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
                || (typeof(T) == typeof(Double) || typeof(T) == typeof(Double?))
                || (typeof(T) == typeof(Byte) || typeof(T) == typeof(Byte?))
                )
            { Dynamic_DefaultValue = 0; }
            else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            { Dynamic_DefaultValue = 0; }
            else if (typeof(T) == typeof(Boolean) || typeof(T) == typeof(Boolean?))
            { Dynamic_DefaultValue = false; }
            else if (typeof(T) == typeof(String))
            { Dynamic_DefaultValue = String.Empty; }
            else
            { throw new Exception("Type is not included in the specified types."); }

            return Convert_Value<T>(Value, Dynamic_DefaultValue);
        }

        /// <summary>
        /// Converts an object to a specified data type.
        /// If conversion fails, returns the specified default value.
        /// </summary>
        /// <typeparam name="T">
        /// Accepts Int16, Int32, Int64, Decimal, Float, Double, DateTime, Boolean, Byte.
        /// </typeparam>
        /// <param name="Value">
        /// The value to be converted.
        /// </param>
        /// <param name="DefaultValue">
        /// The value to be used if the conversion fails.
        /// </param>
        /// <returns>
        /// Returns the converted value according to specified type.
        /// </returns>
        public static T Convert_Value<T>(Object Value, T DefaultValue)
        {
            T ReturnValue = default(T);
            String ValueString = Convert_String(Value);
            Str_ParseResult Pr = new Str_ParseResult();

            if (typeof(T) == typeof(Int16) || typeof(T) == typeof(Int16?))
            { Pr = Convert_Value_ParseInt16(ValueString); }
            else if (typeof(T) == typeof(Int32) || typeof(T) == typeof(Int32?))
            { Pr = Convert_Value_ParseInt32(ValueString); }
            else if (typeof(T) == typeof(Int64) || typeof(T) == typeof(Int64?))
            { Pr = Convert_Value_ParseInt64(ValueString); }
            else if (typeof(T) == typeof(Decimal) || typeof(T) == typeof(Decimal?))
            { Pr = Convert_Value_ParseDecimal(ValueString); }
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
            { Pr = Convert_Value_ParseFloat(ValueString); }
            else if (typeof(T) == typeof(Double) || typeof(T) == typeof(Double?))
            { Pr = Convert_Value_ParseDouble(ValueString); }
            else if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            { Pr = Convert_Value_ParseDateTime(ValueString); }
            else if (typeof(T) == typeof(Boolean) || typeof(T) == typeof(Boolean?))
            { Pr = Convert_Value_ParseBoolean(ValueString); }
            else if (typeof(T) == typeof(Byte) || typeof(T) == typeof(Byte?))
            { Pr = Convert_Value_ParseByte(ValueString); }
            else if (typeof(T) == typeof(String))
            {
                dynamic Dynamic_DefaultValue = DefaultValue;
                Pr = new Str_ParseResult() { IsParsed = true, ParsedValue = Convert_String(Value, Dynamic_DefaultValue) };
            }
            else
            { throw new Exception("Type is not included in the specified types."); }

            if (Pr.IsParsed)
            { ReturnValue = (T)Pr.ParsedValue; }
            else
            { ReturnValue = DefaultValue; }

            return ReturnValue;
        }

        struct Str_ParseResult
        {
            public Boolean IsParsed;
            public Object ParsedValue;
        }

        static Str_ParseResult Convert_Value_ParseInt16(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Int16 OutValue;
            Pr.IsParsed = Int16.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseInt32(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Int32 OutValue;
            Pr.IsParsed = Int32.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseInt64(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Int64 OutValue;
            Pr.IsParsed = Int64.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseDecimal(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Decimal OutValue;
            Pr.IsParsed = Decimal.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseDouble(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Double OutValue;
            Pr.IsParsed = Double.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseFloat(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            float OutValue;
            Pr.IsParsed = float.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseDateTime(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            DateTime OutValue;
            Pr.IsParsed = DateTime.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseBoolean(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Boolean OutValue;
            Pr.IsParsed = Boolean.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        static Str_ParseResult Convert_Value_ParseByte(String ValueString)
        {
            Str_ParseResult Pr = new Str_ParseResult();

            Byte OutValue;
            Pr.IsParsed = Byte.TryParse(ValueString, out OutValue);
            Pr.ParsedValue = OutValue;

            return Pr;
        }

        public static T ParseEnum<T>(String Value)
            where T : struct, IComparable, IFormattable
        { return ParseEnum<T>(Value, default(T)); }

        public static T ParseEnum<T>(string Value, T DefaultValue)
            where T : struct, IComparable, IFormattable
        {
            if (Enum.IsDefined(typeof(T), Value))
            { return (T)Enum.Parse(typeof(T), Value, true); }
            return DefaultValue;
        }

        public static string TextFiller(String TextInput, String Filler, Int32 TextLength)
        {
            string Rv = Strings.Right(Strings.StrDup(TextLength, Filler) + Strings.LTrim(Strings.Left(TextInput, TextLength)), TextLength);
            return Rv;
        }

        public static string PrepareFilterText(String Field, Type DataType, String Filter)
        {
            String Rv = "";

            if (DataType.Name == typeof(String).Name)
            { Rv = @"[" + Field + @"] LIKE '" + Filter + @"%'"; }
            else
            {
                string Tmp_Str = Filter;
                if (ParseFilterText(ref Tmp_Str, DataType))
                { Rv = @"[" + Field + @"] " + Filter + @""; }
            }

            return Rv;
        }

        public static Boolean ParseFilterText(ref String FilterText)
        { return ParseFilterText(ref FilterText, typeof(String)); }

        public static Boolean ParseFilterText(ref String FilterText, Type DataType)
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
                        DataType.Name == typeof(Int16).Name
                        || DataType.Name == typeof(Int32).Name
                        || DataType.Name == typeof(Int64).Name
                        || DataType.Name == typeof(Decimal).Name
                        || DataType.Name == typeof(Double).Name
                        || DataType.Name == typeof(Single).Name)
                    { return true; }
                    break;
                case "BooleanDateTime":
                    if (DataType.Name == typeof(DateTime).Name)
                    { return true; }
                    break;
                case "Numeric":
                    if (
                        DataType.Name == typeof(Int16).Name
                        || DataType.Name == typeof(Int32).Name
                        || DataType.Name == typeof(Int64).Name
                        || DataType.Name == typeof(Decimal).Name
                        || DataType.Name == typeof(Double).Name
                        || DataType.Name == typeof(Single).Name)
                    { return true; }
                    break;
                case "DateTime":
                    if (DataType.Name == typeof(DateTime).Name)
                    {
                        FilterText = " = " + FilterText;
                        return true;
                    }
                    break;
                case "String":
                    if (DataType.Name == typeof(String).Name)
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

        public static string GetMD5(String aString)
        {
            byte[] byteStr = Encoding.UTF8.GetBytes(aString);
            MD5 md5Provider = new MD5CryptoServiceProvider();

            return Encoding.UTF8.GetString(md5Provider.ComputeHash(byteStr));
        }

        public static DateTime GetServerDate(Connection_SqlServer Da)
        {
            DataTable Dt = Da.ExecuteQuery("Select GetDate() As ServerDate").Tables[0];
            if (Dt.Rows.Count > 0) return (DateTime)Dt.Rows[0][0];
            else return DateTime.Now;
        }

        public static DateTime GetServerDate()
        {
            Connection_SqlServer Da = new Connection_SqlServer();
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

        public static Interface_DataAccess CreateDataAccess()
        {
            return new Base().pDa;
        }

        public static Interface_Connection CreateConnection()
        {
            return CreateDataAccess().CreateConnection();
        }

        public static string SerializeObject_Json(Type TargetObjectType, Object TargetObject)
        {
            //DataContractJsonSerializer Js = new DataContractJsonSerializer(TargetObjectType);
            //MemoryStream Ms = new MemoryStream();
            //Js.WriteObject(Ms, TargetObject);

            //Ms.Position = 0;
            //StreamReader Sr = new StreamReader(Ms);
            //string SerializedData = Sr.ReadToEnd();
            //return SerializedData;

            //[-]

            String SerializedData = JsonConvert.SerializeObject(TargetObject);
            return SerializedData;
        }

        public static Object DeserializeObject_Json(Type TargetObjectType, String SerializedData)
        {
            //DataContractJsonSerializer Js = new DataContractJsonSerializer(TargetObjectType);
            //MemoryStream Ms = new MemoryStream();

            //byte[] Bytes_SerializedData = System.Text.Encoding.Unicode.GetBytes(SerializedData);
            //Ms.Write(Bytes_SerializedData, 0, Bytes_SerializedData.Length);

            //return Js.ReadObject(Ms);

            //[-]

            Object DeserializedObject = JsonConvert.DeserializeObject(SerializedData, TargetObjectType);
            return DeserializedObject;
        }

        public static String SerializeObject_Json(Object TargetObject)
        {
            String SerializedData = JsonConvert.SerializeObject(TargetObject);
            return SerializedData;
        }

        /// <summary>
        /// Deserialize a JSON String into an object
        /// </summary>
        /// <typeparam name="T">The type of the object to be deserialized</typeparam>
        /// <param name="SerializedData">The serialized string</param>
        /// <returns></returns>
        public static T DeserializeObject_Json<T>(String SerializedData)
        {
            T DeserializedObject = (T)JsonConvert.DeserializeObject(SerializedData, typeof(T));
            return DeserializedObject;
        }

        public static void SerializeObjectToFile_Json(Object TargetObject, String FileName)
        {
            if (File.Exists(FileName))
            { File.Delete(FileName); }

            using (FileStream Fs = new FileStream(FileName, FileMode.Create))
            using (StreamWriter Sw = new StreamWriter(Fs))
            using (JsonWriter Jw = new JsonTextWriter(Sw))
            {
                Jw.Formatting = Formatting.Indented;
                JsonSerializer Js = new JsonSerializer();
                Js.Serialize(Jw, TargetObject);
            }
        }

        public static Object DeserializeObjectFromFile_Json(Type TargetObjectType, String FileName)
        {
            if (!File.Exists(FileName))
            { return null; }

            Object DeserializedObject = null;
            using (StreamReader Sr = new StreamReader(FileName))
            using (JsonReader Jr = new JsonTextReader(Sr))
            {
                try
                {
                    JsonSerializer Js = new JsonSerializer();
                    DeserializedObject = Js.Deserialize(Jr, TargetObjectType);
                }
                catch { }
            }

            return DeserializedObject;
        }

        public static T DeserializeObjectFromFile_Json<T>(String FileName)
        {
            if (!File.Exists(FileName))
            { return default(T); }

            T DeserializedObject = default(T);
            using (StreamReader Sr = new StreamReader(FileName))
            using (JsonReader Jr = new JsonTextReader(Sr))
            {
                try
                {
                    JsonSerializer Js = new JsonSerializer();
                    DeserializedObject = (T)Js.Deserialize(Jr, typeof(T));
                }
                catch { }
            }

            return DeserializedObject;
        }

        public static string SerializeObject(Object TargetObject)
        {
            IFormatter F = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            MemoryStream Ms = new MemoryStream();
            F.Serialize(Ms, TargetObject);

            Ms.Position = 0;
            StreamReader Sr = new StreamReader(Ms);
            string SerializedData = Sr.ReadToEnd();
            return SerializedData;
        }

        public static Object DeserializeObject(String SerializedData)
        {
            Object Rv = null;
            IFormatter F = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (MemoryStream Ms = new MemoryStream())
            {
                byte[] Bytes_SerializedData = System.Text.Encoding.UTF8.GetBytes(SerializedData);
                Ms.Write(Bytes_SerializedData, 0, Bytes_SerializedData.Length);
                Ms.Position = 0;
                Rv = F.Deserialize(Ms);
            }

            return Rv;
        }

        public static String GenerateGuid(List<String> List)
        {
            String NewGuid = null;
            Boolean IsValid = false;

            while (!IsValid)
            {
                NewGuid = Guid.NewGuid().ToString();
                if (!List.Exists(X => X == NewGuid))
                {
                    IsValid = true;
                }
            }

            return NewGuid;
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
