using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjects_Framework;
using DataObjects_Framework.Objects;
using System.Runtime.Serialization;

namespace DataObjects_Framework.Common
{
    /// <summary>
    /// Constants and structs used in the framework are defined here.
    /// </summary>
    public class Do_Constants
    {
        /// <summary>
        /// Struct Str_Parameters, used in various query execution methods.
        /// </summary>
        [Serializable()]
        public struct Str_Parameters
        {
            public string Name;
            public object Value;
            public Int32 Size;
            public Byte Scale;
            public Byte Precision;
            public eParameterType Type;

            /// <summary>
            /// Constructor for Str_Parameters
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pValue"></param>
            public Str_Parameters(string pName, object pValue)
            {
                Name = pName;
                Value = pValue;
                Size = 0;
                Scale = 0;
                Precision = 0;
                Type = eParameterType.None;
            }
        }

        /// <summary>
        /// Struct Str_Sort
        /// </summary>
        public struct Str_Sort
        {
            public string Name;
            public bool IsDesc;

            /// <summary>
            /// Constructor for Str_Sort
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pIsDesc"></param>
            public Str_Sort(string pName, bool pIsDesc)
            {
                this.Name = pName;
                this.IsDesc = pIsDesc;
            }
        }

        /// <summary>
        /// Provides structure for custom Foreign Key definitions
        /// </summary>
        [Serializable()]
        public struct Str_ForeignKeyRelation
        {
            /// <summary>
            /// The primary key of the parent table
            /// </summary>
            public string Parent_Key;

            /// <summary>
            /// The matching foreign key for the target table detail
            /// </summary>
            public string Child_Key;

            /// <summary>
            /// Constructor for Str_ForeignKeyRelation
            /// </summary>
            /// <param name="Parent_Key"></param>
            /// <param name="Child_Key"></param>
            public Str_ForeignKeyRelation(string Parent_Key, string Child_Key)
            {
                this.Parent_Key = Parent_Key;
                this.Child_Key = Child_Key;
            }
        }

        [Serializable()]
        public struct Str_QuerySource
        {
            public String ObjectName;
            public String Fields;
            public String Condition;
        }

        /// <summary>
        /// Data Access Type settings enum for DataObjects_Framework
        /// </summary>
        public enum eDataAccessType : long
        {
            /// <summary>
            /// connect to a SQL Server with the provided connection string in the Do_Globals.gSettings.pConnectionString
            /// </summary>
            DataAccess_SqlServer = 0,

            /// <summary>
            /// connect to a WCF Server with the provided server address in the Do_Globals.gSettings.pWcfAddress
            /// </summary>
            DataAccess_WCF = 1
        }

        [Serializable()]
        public enum eParameterType : long
        {
            None,
            VarChar,
            Numeric,
            Int,
            Long,
            DateTime,
            Guid,
            Boolean,
            Binary
        }

        [Serializable()]
        public struct Str_Request_List
        {
            public String ObjectName;
            public String Fields;
            public QueryCondition Condition;
            public String Condition_String;
            public String Sort;
            public Int64 Top;
            public Int32 Page;
            public String ConnectionString;
        }

        [Serializable()]
        public struct Str_Request_Load
        {
            public String ObjectName;
            public List<String> ObjectKeys;
            public Keys Key;
            public String Condition;
            public List<Do_Constants.Str_ForeignKeyRelation> ForeignKeys;
            public String ConnectionString;
        }

        [Serializable()]
        public struct Str_Request_Save
        {
            public String Serialized_ObjectDataRow;
            public String TableName;
            public String SchemaName;
            public Boolean IsDelete;
            public List<String> CustomKeys;
            public String ConnectionString;
        }

        //[Serializable()]
        //public struct Str_Request_BuildQuerySource
        //{
        //    public String ObjectName;
        //    public String Fields;
        //    public String Condition;
        //    public String Sort;
        //}

        [Serializable()]
        public struct Str_Request_GetQuery
        {
            public String ObjectName;
            public Str_QuerySource ObjectQuerySource;
            public String Fields;
            public QueryCondition Condition;
            public String Condition_String;
            public String Sort;
            public Int64 Top;
            public Int32 Page;
            public String ConnectionString;
        }

        [Serializable()]
        public struct Str_Request_Execute
        {
            public String Query;
            public String ProcedureName;
            public List<QueryParameter> ProcedureParameters;
            public String ConnectionString;
        }

        [Serializable()]
        public struct Str_Request_PreparedQuery_Prepare
        {
            public String Query;
            public List<QueryParameter> Parameters;
            public String ConnectionString;
        }

        [Serializable()]
        public struct Str_Request_PreparedQuery_Parameters
        {
            public List<QueryParameter> Parameters;
        }

        [Serializable()]
        public struct Str_Request_SystemParameter
        {
            public String ParameterName;
            public String ParameterValue;
            public String ConnectionString;
        }

        [Serializable()]
        public struct Str_Request_Session
        {
            public String SessionID;
        }

        [Serializable()]
        public struct Str_Response_Save
        {
            public SimpleDataRow Sdr;
            public Boolean SaveResult;
        }
    }
}
