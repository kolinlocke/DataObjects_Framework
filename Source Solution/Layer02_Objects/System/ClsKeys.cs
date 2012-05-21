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
using DataObjects_Framework;
using DataObjects_Framework.Modules_Base;
using DataObjects_Framework.Modules_Base.Abstract;
using DataObjects_Framework.Modules_Base.Objects;
using DataObjects_Framework._System;

namespace DataObjects_Framework._System
{
    /// <summary>
    /// Used for loading Data Objects
    /// </summary>
    public class ClsKeys
    {
        #region _Variables

        public struct Str_Keys
        {
            public string Name;
            public Int64 Value;
            public Str_Keys(string pName, Int64 pValue)
            {
                Name = pName;
                Value = pValue;
            }
        }

        List<Str_Keys> mObj = new List<Str_Keys>();
        
        #endregion

        #region _Methods

        /// <summary>
        /// Adds a Key Definition
        /// </summary>
        /// <param name="Name">
        /// Name of the key, most likely the primary key of the table used by the Data Object
        /// </param>
        /// <param name="Value">
        /// The value of the key, most likely the ID of the table
        /// </param>
        public void Add(string Name, Int64 Value = 0)
        {
            this.mObj.Add(new Str_Keys(Name, Value));
        }

        /// <summary>
        /// Returns the number of keys contained in this object
        /// </summary>
        /// <returns></returns>
        public Int32 Count()
        { return this.mObj.Count(); }
        
        #endregion

        #region _Properties

        /// <summary>
        /// Default Property, gets/sets the Key Value with the Key Name supplied
        /// </summary>
        /// <param name="Name">
        /// Name of the key
        /// </param>
        /// <returns></returns>
        public Int64 this[string Name]
        {            
            get
            {
                /*
                foreach (Str_Keys Obj in this.mObj)
                {
                    if (Name == Obj.Name) return Obj.Value;
                }
                return 0;
                */

                return this.mObj.FirstOrDefault(X => X.Name == Name).Value;                
            }
            set
            {
                /*
                foreach (Str_Keys Obj in this.mObj)
                {
                    if (Name == Obj.Name)
                    {
                        Str_Keys Inner_Obj = Obj;
                        Inner_Obj.Value = value;
                        return;
                    }
                }
                */

                Str_Keys Obj = this.mObj.FirstOrDefault(X => X.Name == Name);
                Obj.Value = value;
            }
        }

        /// <summary>
        /// Default Property, gets/sets the Key Value with the index supplied
        /// </summary>
        /// <param name="Index">
        /// Index of the key
        /// </param>
        /// <returns></returns>
        public Int64 this[Int32 Index]
        {
            get
            { return this.mObj[Index].Value; }
            set
            {
                Str_Keys Inner_Obj = this.mObj[Index];
                Inner_Obj.Value = value;
            }
        }

        /// <summary>
        /// Returns the array of the keys defined
        /// </summary>
        public string[] pName
        {
            get
            {
                List<string> Name = new List<string>();
                foreach (Str_Keys Obj in this.mObj)
                { Name.Add(Obj.Name); }
                return Name.ToArray();
            }
        }
        
        #endregion
    }
}
