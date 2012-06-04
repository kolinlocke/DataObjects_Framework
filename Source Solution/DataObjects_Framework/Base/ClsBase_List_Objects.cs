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

namespace DataObjects_Framework.Base
{
    /// <summary>
    /// Base Class for Data Objects, a list of Data Objects will be loaded instead.
    /// </summary>
    public abstract class ClsBase_List_Objects : ClsBase_List
    {
        #region _Variables

        //ClsBase mTemplate_Obj;
        //string mTemplate_ViewName;
        Str_Template mTemplate;
        List<Object> mObj_ClsBase_Constructors = new List<Object>();

        /// <summary>
        /// Storage for the list of data objects loaded.
        /// </summary>
        protected List<Str_Obj> mList_Obj = new List<Str_Obj>();

        public struct Str_Template
        {
            public ClsBase Obj;
            public string ViewName;
            public List<string> Keys;
        }

        /// <summary>
        /// Struct Str_Obj
        /// </summary>
        public struct Str_Obj
        {
            /// <summary>
            /// Name of the ClsBase object
            /// </summary>
            public string Name;

            /// <summary>
            /// The ClsBase object
            /// </summary>
            public ClsBase Obj;

            /// <summary>
            /// Constructor for Str_Obj
            /// </summary>
            /// <param name="pName"></param>
            /// <param name="pObj"></param>
            public Str_Obj(string pName, ClsBase pObj)
            {
                this.Name = pName;
                this.Obj = pObj;
            }
        }

        #endregion

        #region _Methods

        /// <summary>
        /// Sets the data object definition, 
        /// must be set preferably in the constructor of the derived object
        /// </summary>
        /// <param name="Template_Obj">
        /// Object to use as a template for the list, must be derived from ClsBase
        /// </param>
        /// <param name="TableName">
        /// Table Name of the data object will be using
        /// </param>
        /// <param name="ViewName">
        /// View Name of the data object 
        /// this will be used Me.Load() if supplied
        /// </param>
        /// <param name="Qc_LoadCondition">
        /// Additional conditions when fetching the data object
        /// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        public virtual void Setup(
            Str_Template Template
            , string TableName
            , string ViewName = ""
            , ClsQueryCondition Qc_LoadCondition = null
            , List<string> CustomKeys = null
            )
        {
            if (!(Template.Obj is ClsBase)) { throw new Exception("Template.Obj must be derived from ClsBase."); }
            this.mTemplate = Template;
            base.Setup(TableName, ViewName, Qc_LoadCondition, CustomKeys);
        }

        /// <summary>
        /// Key object to use, if null, it implies to create a new data object.
        /// </summary>
        /// <param name="Keys"></param>
        public override void Load(ClsKeys Keys = null)
        {
            base.Load(Keys);
            this.Load_Objects(Keys);
        }
        
        void Load_Objects(ClsKeys Keys)
        {
            if (Keys != null)
            {
                ClsQueryCondition Qc = this.mDa.CreateQueryCondition();
                foreach (string KeyName in Keys.pName)
                { Qc.Add(KeyName, Keys[KeyName].ToString(), typeof(Int64).ToString(), "0"); }

                DataTable Dt = this.mDa.List(this.mTemplate.ViewName, Qc);
                foreach (DataRow Dr in Dt.Rows)
                {
                    ClsBase Inner_Obj = (ClsBase)Activator.CreateInstance(this.mTemplate.Obj.GetType(), this.mObj_ClsBase_Constructors.ToArray());
                    Inner_Obj.Load(Dr);
                    this.mList_Obj.Add(new Str_Obj(Do_Methods.Convert_Int64(Dr["TmpKey"]).ToString(), Inner_Obj));
                }
            }
        }

        /// <summary>
        /// Saves changes to the List
        /// </summary>
        /// <param name="Da">
        /// An open Data_Access Objects that is reused from the calling method
        /// </param>
        /// <returns></returns>
        public override bool Save(DataAccess.Interface_DataAccess Da = null)
        {
            bool Rv = base.Save(Da);
            this.Begin_Save_Objects();
            return Rv;
        }

        void Begin_Save_Objects()
        {
            DataRow[] ArrDr = this.mDt_List.Select("", "", DataViewRowState.CurrentRows);
            foreach (DataRow Inner_Dr in ArrDr)
            {
                ClsBase Inner_Obj = this.mList_Obj.FirstOrDefault(Item => Item.Name == Do_Methods.Convert_String(Inner_Dr["TmpKey"])).Obj;
                if (Inner_Obj != null)
                {
                    this.Save_Objects(Inner_Obj);
                    Inner_Obj.Save(); 
                }
            }
        }
        
        /// <summary>
        /// Can be overrided to add additional 
        /// </summary>
        /// <param name="Obj"></param>
        protected virtual void Save_Objects(ClsBase Obj) { }

        /// <summary>
        /// Not Implemented. Use Add_Object().
        /// </summary>
        public override DataRow Add_Item()
        { throw new NotImplementedException(); }

        /// <summary>
        /// Adds a new ClsBase object with the related data row to the collection.
        /// </summary>
        /// <returns></returns>
        public ClsBase Add_Object()
        {
            DataRow Dr = base.Add_Item();
            ClsBase Obj = (ClsBase)Activator.CreateInstance(this.mTemplate.Obj.GetType(), this.mObj_ClsBase_Constructors.ToArray());
            Obj.Load(Dr);
            this.mList_Obj.Add(new Str_Obj(Do_Methods.Convert_Int64(Dr["TmpKey"]).ToString(), Obj));

            return Obj;
        }

        #endregion

        #region _Properties

        /// <summary>
        /// Gets the list of data objects loaded.
        /// </summary>
        public List<Str_Obj> pList_Obj
        {
            get { return this.mList_Obj; }
        }

        #endregion
    }
}
