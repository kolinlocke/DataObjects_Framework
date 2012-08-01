using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public struct Str_Parameters
        {
            public string Name;
            public object Value;

            /// <summary>
            /// Constructor for Str_Parameters
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pValue"></param>
            public Str_Parameters(string pName, object pValue)
            {
                Name = pName;
                Value = pValue;
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

        public enum eDataAccessType : long
        { 
            DataAccess_SqlServer = 0
            , DataAccess_WCF = 1
        }
    }
}
