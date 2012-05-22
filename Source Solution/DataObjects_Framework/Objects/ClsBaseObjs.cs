using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using DataObjects_Framework;
using DataObjects_Framework.Common;
using DataObjects_Framework.Connection;
using DataObjects_Framework.Objects;
using DataObjects_Framework.Base;

namespace DataObjects_Framework.Objects
{
    /// <summary>
    /// Object storage for list of ClsBase objects
    /// </summary>
    public class ClsBaseObjs
    {
        #region _Variables

        public struct Str_Obj
        {
            public string Name;
            public ClsBase Obj;

            public Str_Obj(string pName, ClsBase pObj)
            {
                this.Name = pName;
                this.Obj = pObj;
            }
        }

        List<Str_Obj> mList_Obj = new List<Str_Obj>();

        #endregion

        #region _Methods

        /// <summary>
        /// Adds a ClsBase object to the collection
        /// </summary>
        /// <param name="Name">
        /// The name of the ClsBase object to be used for retrieval
        /// </param>
        /// <param name="Obj">
        /// The ClsBase object to be added
        /// </param>
        public void Add(string Name, ClsBase Obj)
        { this.mList_Obj.Add(new Str_Obj(Name, Obj)); }

        /// <summary>
        /// Gets the number of ClsBase objects stored in the collection
        /// </summary>
        /// <returns></returns>
        public Int32 Count()
        { return this.mList_Obj.Count; }

        #endregion

        #region _Properties

        /// <summary>
        /// Returns the object with the name specified
        /// </summary>
        /// <param name="Name">
        /// The name of the object to retrieve
        /// </param>
        /// <returns></returns>
        public ClsBase this[string Name]
        {
            get
            {
                //foreach (Str_Obj Obj in this.mList_Obj)
                //{
                //    if (Name == Obj.Name)
                //    { return Obj.Obj; }
                //}
                //return null;

                return this.mList_Obj.FirstOrDefault(X => X.Name == Name).Obj;
            }
        }

        /// <summary>
        /// Returns a string array of the Names of the ClsBase objects in the collection
        /// </summary>
        public string[] pName
        {
            get
            {
                List<string> List_Name = new List<string>();
                foreach (Str_Obj Obj in this.mList_Obj)
                { List_Name.Add(Obj.Name); }
                return List_Name.ToArray();
            }
        }

        /// <summary>
        /// Returns the ClsBase object collection
        /// </summary>
        public List<Str_Obj> pList_Obj
        {
            get
            { return this.mList_Obj; }
        }

        #endregion
    }
}
