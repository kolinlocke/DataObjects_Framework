﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;

namespace DataObjects_Framework.Objects
{
    /// <summary>
    /// Used for adding conditions for various fetching methods,
    /// generally used to avoid Sql Injection attacks
    /// </summary>
    [Serializable]
    public class ClsQueryCondition
    {
        #region _Variables

        /// <summary>
        /// Struct Query Condition
        /// </summary>
        [Serializable]
        public struct Str_QueryCondition 
        {
            /// <summary>
            /// The Name of the condition
            /// </summary>
            public string Name;

            /// <summary>
            /// The Field Name of the condition on the data source
            /// </summary>
            public string FieldName;

            /// <summary>
            /// The operator of the condition
            /// </summary>
            public string Operator;

            /// <summary>
            /// The condition value
            /// </summary>
            public object Value;

            /// <summary>
            /// The data type of the condition field
            /// </summary>
            public string DataType;

            /// <summary>
            /// The default value of the condition field if its null
            /// </summary>
            public string DefaultValue;

            /// <summary>
            /// Constructor for Str_QueryCondition
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pFieldName"></param>
            /// <param name="pOperator"></param>
            /// <param name="pValue"></param>
            /// <param name="pDataType"></param>
            /// <param name="pDefaultValue"></param>
            public Str_QueryCondition(
                string pName
                , string pFieldName
                , string pOperator
                , object pValue
                , string pDataType
                , string pDefaultValue)
            {
                this.Name = pName;
                this.FieldName = pFieldName;
                this.Operator = pOperator;
                this.Value = pValue;
                this.DataType = pDataType;
                this.DefaultValue = pDefaultValue;
            }
        }

        Int64 mCt = 0;

        /// <summary>
        /// The condition object collection
        /// </summary>
        protected List<Str_QueryCondition> mQc = new List<Str_QueryCondition>();

        #endregion

        #region _Methods

        /// <summary>
        /// Add a condition
        /// </summary>
        /// <param name="Name">
        /// Field Name on the data source
        /// </param>
        /// <param name="Operator">
        /// Operator to use (SQL valid operators)
        /// </param>
        /// <param name="Value">
        /// The condition value
        /// </param>
        /// <param name="DataType">
        /// Field Data Type
        /// </param>
        /// <param name="DefaultValue">
        /// Default value to use with ISNULL function
        /// </param>
        public void Add(string Name, string Operator, object Value, string DataType = "", string DefaultValue = "")
        {
            if (Operator.Trim() == "")
            {
                if (DataType == "") DataType = typeof(string).ToString();

                if (DataType.ToUpper() == typeof(string).ToString().ToUpper() || DataType.ToUpper() == typeof(string).Name.ToUpper())
                {
                    Operator = "Like";
                    Value = Value + @"%";
                }
                else Operator = "=";
            }

            if (DataType.ToUpper() == typeof(DateTime).ToString().ToUpper() || DataType.ToUpper() == typeof(DateTime).Name.ToUpper())
            { if (!Information.IsDate(Value)) Value = DBNull.Value; }

            bool IsFound = false;
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                if (Obj.Name == Name)
                {
                    IsFound = true;
                    break;
                }
            }

            string FieldName = Name;
            if (IsFound)
            {
                this.mCt++;
                Name = Name + this.mCt.ToString();
            }

            if (Operator.ToUpper() == "LIKE")
            {
                try
                {
                    if (Strings.Left(Value.ToString(), 1) == @"'"
                    || Strings.Left(Value.ToString(), 1) == @"""")
                    { Value = Strings.Mid(Value.ToString(), 2); }
                }
                catch { }

                try
                {
                    if (Strings.Right(Value.ToString(), 1) == @"'"
                    || Strings.Right(Value.ToString(), 1) == @"""")
                    { Value = Strings.Mid(Value.ToString(), 1, Value.ToString().Length - 1); }
                }
                catch { }
            }

            this.mQc.Add(new Str_QueryCondition(Name, FieldName, Operator, Value, DataType, DefaultValue));
        }

        /// <summary>
        /// Add a condition
        /// </summary>
        /// <param name="Name">
        /// Field Name on the data source
        /// </param>
        /// <param name="Condition">
        /// The condition value
        /// </param>
        /// <param name="DataType">
        /// Field Data Type
        /// </param>
        /// <param name="DefaultValue">
        /// Default value to use with ISNULL function
        /// </param>
        public void Add(string Name, string Condition, string DataType, string DefaultValue = "")
        {
            string Condition_Operator ="";
            string Condition_Value = "";

            if (DataType.ToLower() == typeof(string).Name.ToLower())
            { Condition_Value = Condition; }
            else
            {
                string[] Arr_ParsedText = Condition.Split(' ');

                try
                {
                    switch (Arr_ParsedText[0].ToUpper())
                    {
                        case ">":
                        case "<":
                        case "=":
                        case "<=":
                        case ">=":
                        case "<>":
                        case "LIKE":
                            Condition_Operator = Arr_ParsedText[0];
                            break;
                        case "!=":
                            Condition_Operator = "<>";
                            break;
                    }
                }
                catch { }

                /*
                if (Condition_Operator != "")
                { Array.Clear(Arr_ParsedText, 0, 1); }                
                Condition_Value = string.Join("", Arr_ParsedText);
                */

                if (Condition_Operator != "")
                { Condition_Value = Strings.Mid(Condition, Strings.InStr(Condition, " ") + 1); }
                else
                { Condition_Value = Condition; }

            }
            this.Add(Name, Condition_Operator, Condition_Value, DataType, DefaultValue);
        }

        /// <summary>
        /// Returns the number of conditions in this instance
        /// </summary>
        /// <returns></returns>
        public Int32 Count()
        { return this.mQc.Count; }

        /// <summary>
        /// Returns the parameterized condition string, to be used with data fetching.
        /// Used internally.
        /// </summary>
        /// <returns></returns>
        public virtual string GetQueryCondition()
        {
            string Query_Condition = " 1 = 1 ";
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                string Field;
                string Condition;

                if (Obj.DataType.ToUpper() == typeof(DateTime).ToString().ToUpper() || Obj.DataType.ToUpper() == typeof(DateTime).Name.ToUpper())
                {
                    Field = @"dbo.udf_ConvertDate([" + Obj.FieldName + @"])";
                    Condition = @"dbo.udf_ConvertDate(@Condition_" + Obj.Name + @")";
                }
                else
                {
                    Field = @"[" + Obj.FieldName + @"]";
                    Condition = @"@Condition_" + Obj.Name;
                }

                if (Obj.DefaultValue != "")
                {
                    Object DefaultValue = "";
                    if (Obj.DataType.ToUpper() == typeof(DateTime).ToString().ToUpper()
                        || Obj.DataType.ToUpper() == typeof(DateTime).Name.ToUpper()
                        || Obj.DataType.ToUpper() == typeof(string).ToString().ToUpper()
                        || Obj.DataType.ToUpper() == typeof(string).Name.ToUpper())
                    { DefaultValue = @"'" + Obj.DefaultValue + @"'"; }
                    else
                    { DefaultValue = Obj.DefaultValue; }
                    Field = @"IsNull(" + Field + @"," + DefaultValue + @")";
                }
                Query_Condition += " And " + Field + " " + Obj.Operator + " " + Condition;
            }
            return Query_Condition;
        }

        /// <summary>
        /// Returns the SqlParameter Collection associated in the conditions in this instance.
        /// </summary>
        /// <returns></returns>
        public SqlParameter[] GetSqlParameters()
        {
            List<SqlParameter> List_Sp = new List<SqlParameter>();
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                string Name = Obj.Name;
                string FieldName = Obj.FieldName;
                object Value = Obj.Value;
                string DataType = Obj.DataType.ToUpper();

                SqlParameter Sp = new SqlParameter();
                Sp.ParameterName = "Condition_" + Name;
                
                if (
                    DataType == typeof(string).Name.ToUpper() 
                    || DataType == typeof(string).ToString().ToUpper()
                    )
                {
                    Sp.SqlDbType = SqlDbType.VarChar;
                    Sp.Size = 8000;
                }
                else if (
                    DataType == typeof(Int16).Name.ToUpper() 
                    || DataType == typeof(Int16).ToString().ToUpper()
                    )
                { 
                    Int16 Out;
                    Int16.TryParse(Value.ToString(), out Out);
                    Value = Out;

                    Sp.SqlDbType = SqlDbType.SmallInt; 
                }
                else if (
                    DataType == typeof(Int32).Name.ToUpper() 
                    || DataType == typeof(Int32).ToString().ToUpper()
                    )
                {
                    Int32 Out;
                    Int32.TryParse(Value.ToString(), out Out);
                    Value = Out;

                    Sp.SqlDbType = SqlDbType.Int; 
                }
                else if (
                    DataType == typeof(Int64).Name.ToUpper() 
                    || DataType == typeof(Int64).ToString().ToUpper()
                    )
                {
                    Int64 Out;
                    Int64.TryParse(Value.ToString(), out Out);
                    Value = Out;

                    Sp.SqlDbType = SqlDbType.BigInt; 
                }
                else if (
                    DataType == typeof(decimal).Name.ToUpper() 
                    || DataType == typeof(decimal).ToString().ToUpper() 
                    || DataType == typeof(double).Name.ToUpper() 
                    || DataType == typeof(double).ToString().ToUpper()
                    )
                {
                    Double Out;
                    Double.TryParse(Value.ToString(), out Out);
                    Value = Out;

                    Sp.SqlDbType = SqlDbType.Decimal;
                    Sp.Precision = 18;
                    Sp.Scale = 4;
                }
                else if (
                    DataType == typeof(bool).Name.ToUpper() 
                    || DataType == typeof(bool).ToString().ToUpper()
                    )
                {
                    Boolean Out;
                    Boolean.TryParse(Value.ToString(), out Out);
                    Value = Out;

                    Sp.SqlDbType = SqlDbType.Bit; 
                }
                else if (
                    DataType == typeof(DateTime).Name.ToUpper() 
                    || DataType == typeof(DateTime).ToString().ToUpper()
                    )
                {
                    DateTime Out;
                    DateTime.TryParse(Value.ToString(), out Out);
                    Value = Out;

                    Sp.SqlDbType = SqlDbType.DateTime; 
                }

                Sp.Value = Value;

                List_Sp.Add(Sp);
            }
            return List_Sp.ToArray();
        }

        public List<ClsParameter> GetParameters()
        {
            List<ClsParameter> List_Sp = new List<ClsParameter>();
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                string Name = Obj.Name;
                string FieldName = Obj.FieldName;
                object Value = Obj.Value;
                string DataType = Obj.DataType.ToUpper();

                ClsParameter Sp = new ClsParameter();
                Sp.Name = "Condition_" + Name;

                if (
                    DataType == typeof(string).Name.ToUpper()
                    || DataType == typeof(string).ToString().ToUpper()
                    )
                {
                    Sp.Type = Do_Constants.eParameterType.VarChar;
                    Sp.Size = 8000;
                }
                else if (
                    DataType == typeof(Int16).Name.ToUpper()
                    || DataType == typeof(Int16).ToString().ToUpper()
                    )
                {
                    Value = Do_Methods.Convert_Int32(Value);
                    Sp.Type = Do_Constants.eParameterType.Int;
                }
                else if (
                    DataType == typeof(Int32).Name.ToUpper()
                    || DataType == typeof(Int32).ToString().ToUpper()
                    )
                {
                    Value = Do_Methods.Convert_Int32(Value);
                    Sp.Type = Do_Constants.eParameterType.Int;
                }
                else if (
                    DataType == typeof(Int64).Name.ToUpper()
                    || DataType == typeof(Int64).ToString().ToUpper()
                    )
                {
                    Value = Do_Methods.Convert_Int64(Value);
                    Sp.Type = Do_Constants.eParameterType.Long;
                }
                else if (
                    DataType == typeof(decimal).Name.ToUpper()
                    || DataType == typeof(decimal).ToString().ToUpper()
                    || DataType == typeof(double).Name.ToUpper()
                    || DataType == typeof(double).ToString().ToUpper()
                    )
                {
                    Value = Do_Methods.Convert_Double(Value);
                    Sp.Type = Do_Constants.eParameterType.Numeric;
                    Sp.Precision = 18;
                    Sp.Scale = 4;
                }
                else if (
                    DataType == typeof(bool).Name.ToUpper()
                    || DataType == typeof(bool).ToString().ToUpper()
                    )
                {
                    Value = Do_Methods.Convert_Boolean(Value);
                    Sp.Type = Do_Constants.eParameterType.Boolean;
                }
                else if (
                    DataType == typeof(DateTime).Name.ToUpper()
                    || DataType == typeof(DateTime).ToString().ToUpper()
                    )
                {
                    Value = Do_Methods.Convert_DateTime(Value);
                    Sp.Type = Do_Constants.eParameterType.DateTime;
                }

                Sp.Value = Value;

                List_Sp.Add(Sp);
            }
            return List_Sp;
        }

        /// <summary>
        /// Returns the non-parameterized condition string, to be used with data fetching.
        /// </summary>
        /// <returns></returns>
        public string GetQueryCondition_String()
        {
            string Query_Condition = " 1 = 1 ";
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                string Field;
                string Condition;

                if (Obj.DataType.ToUpper() == typeof(string).ToString().ToUpper()
                    || Obj.DataType.ToUpper() == typeof(string).Name.ToUpper()
                    || Obj.DataType.ToUpper() == typeof(DateTime).ToString().ToUpper()
                    || Obj.DataType.ToUpper() == typeof(DateTime).Name.ToUpper())
                {
                    Field = @"[" + Obj.FieldName + @"]";
                    Condition = @"'" + Obj.Value + @"'";
                }
                else
                {
                    Field = @"[" + Obj.FieldName + @"]";
                    Condition = Obj.Value.ToString();
                }

                if (Obj.DefaultValue != "")
                {
                    Object DefaultValue = "";
                    if (Obj.DataType.ToUpper() == typeof(DateTime).ToString().ToUpper()
                        || Obj.DataType.ToUpper() == typeof(DateTime).Name.ToUpper()
                        || Obj.DataType.ToUpper() == typeof(string).ToString().ToUpper()
                        || Obj.DataType.ToUpper() == typeof(string).Name.ToUpper())
                    { DefaultValue = @"'" + Obj.DefaultValue + @"'"; }
                    else
                    { DefaultValue = Obj.DefaultValue; }
                    Field = @"ISNULL(" + Field + @"," + DefaultValue + @")";
                }
                Query_Condition += " And " + Field + " " + Obj.Operator + " " + Condition;
            }
            return Query_Condition;
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Gets the condition object value based on the name supplied
        /// </summary>
        /// <param name="Name">
        /// The name of the condition object to get the value from
        /// </param>
        /// <returns></returns>
        public object pValue_Get(string Name)
        {
            foreach (Str_QueryCondition Obj in this.mQc)
            { if (Name == Obj.Name) return Obj.Value; }
            return DBNull.Value;
        }

        /// <summary>
        /// Sets the new condition object value based on the name supplied
        /// </summary>
        /// <param name="Name">
        /// The name of the object to set its value
        /// </param>
        /// <param name="Value">
        /// The new value to set
        /// </param>
        public void pValue_Set(string Name, object Value)
        {
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                if (Name == Obj.Name)
                {
                    Str_QueryCondition Inner_Obj = Obj;
                    Inner_Obj.Value = Value;
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the condition object based on the name supplied
        /// </summary>
        /// <param name="Name">
        /// The name of the condition object to get
        /// </param>
        /// <returns></returns>
        public Str_QueryCondition pObj_Get(string Name)
        {
            foreach (Str_QueryCondition Obj in this.mQc)
            { if (Name == Obj.Name) return Obj; }
            return new Str_QueryCondition();
        }

        /// <summary>
        /// Sets a new condition object based on the name supplied
        /// </summary>
        /// <param name="Name">
        /// The name of the condition object to overwrite
        /// </param>
        /// <param name="Value">
        /// The new condition object to assign
        /// </param>
        public void pObj_Set(string Name, Str_QueryCondition Value)
        {
            foreach (Str_QueryCondition Obj in this.mQc)
            {
                if (Name == Obj.Name)
                {
                    Str_QueryCondition Inner_Obj = Obj;
                    Inner_Obj = Value;
                    return;
                }                
            }
        }
        
        /// <summary>
        /// Gets the condition object collection
        /// </summary>
        public List<Str_QueryCondition> pList
        {
            get { return this.mQc; }
        }

        #endregion
    }
}
