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
    public abstract class ClsBase_List_Objects : ClsBase_List
    {
        #region _Variables

        ClsBase mObj_ClsBase;
        List<Object> mObj_ClsBase_Constructors = new List<Object>();
        protected List<Str_Obj> mList_Obj = new List<Str_Obj>();

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

        public virtual void Setup(
            ClsBase Obj_ClsBase
            , string TableName
            , string ViewName = ""
            , List<string> CustomKeys = null
            , ClsQueryCondition Qc_LoadCondition = null) 
        {
            if (!(Obj_ClsBase is ClsBase)) { throw new Exception("Obj_ClsBase must be derived from ClsBase."); }
            this.mObj_ClsBase = Obj_ClsBase;
            base.Setup(TableName, ViewName, CustomKeys, Qc_LoadCondition);
        }

        public override void Load(string Condition)
        {
            base.Load(Condition);
            this.Load_Objects();
        }

        public override void Load(ClsQueryCondition Condition)
        {
            base.Load(Condition);
            this.Load_Objects();
        }

        void Load_Objects()
        {
            foreach (DataRow Dr in this.mDt_List.Rows)
            {
                ClsBase Inner_Obj = (ClsBase)Activator.CreateInstance(this.mObj_ClsBase.GetType(), this.mObj_ClsBase_Constructors.ToArray());
                Inner_Obj.Load(Dr);
                this.mList_Obj.Add(new Str_Obj(Do_Methods.Convert_Int64(Dr["TmpKey"]).ToString(), Inner_Obj));
            }
        }

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
                ClsBase Inner_Obj = this.mList_Obj.FirstOrDefault(X => X.Name == Do_Methods.Convert_String(Inner_Dr["TmpKey"])).Obj;
                if (Inner_Obj != null)
                {
                    this.Save_Objects(Inner_Obj);
                    Inner_Obj.Save(); 
                }
            }
        }
        
        protected virtual void Save_Objects(ClsBase Obj) { }

        public ClsBase Add_Object()
        {
            DataRow Dr = this.mDt_List.NewRow();
            Dr["TmpKey"] = ClsBase.GetNewTmpKey(this.mDt_List);
            this.mDt_List.Rows.Add(Dr);

            ClsBase Obj = (ClsBase)Activator.CreateInstance(this.mObj_ClsBase.GetType(), this.mObj_ClsBase_Constructors.ToArray());
            Obj.Load(Dr);
            this.mList_Obj.Add(new Str_Obj(Do_Methods.Convert_Int64(Dr["TmpKey"]).ToString(), Obj));

            return Obj;
        }

        #endregion
    }
}
