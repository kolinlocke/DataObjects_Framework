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
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Base;

namespace DataObjects_Framework.Base
{
    public class ClsBaseListDetail
    {
        #region _Variables

        string mName;
        ClsBase mObj_Base;
        ClsBase_List mObj_List;
        List<Do_Constants.Str_ForeignKeyRelation> mList_ForeignKey = new List<Do_Constants.Str_ForeignKeyRelation>();

        #endregion

        #region _Constructor

        private ClsBaseListDetail() { }

        public ClsBaseListDetail(
            ClsBase Obj_Base
            , string Name
            , ClsBase_List Obj_List
            , List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
        {
            this.mName = Name;
            this.mObj_Base = Obj_Base;
            this.mObj_List = Obj_List;
            this.mList_ForeignKey = CustomForeignKeys;
        }

        #endregion

        #region _Methods

        public void Load(Interface_DataAccess Da, ClsKeys Keys)
        {
            StringBuilder Sb_Condition = new StringBuilder();
            ClsKeys Load_Keys = null;

            if (this.mList_ForeignKey != null)
            {
                foreach (string Inner_KeyName in Keys.pName)
                {
                    Do_Constants.Str_ForeignKeyRelation Fk = this.mList_ForeignKey.FirstOrDefault(Item => Item.Parent_Key == Inner_KeyName);
                    if (Fk.Parent_Key == string.Empty) { throw new Exception("All foreign keys must match the parent keys."); }
                    Load_Keys.Add(Fk.Child_Key, Keys[Inner_KeyName]);
                }
            }
            else
            { Load_Keys = Keys; }

            this.mObj_List.Load(Load_Keys, this.mObj_Base);
        }

        public void Save(Interface_DataAccess Da)
        {
            foreach (DataRow Dr in this.mObj_List.pDt_List.Rows)
            {
                foreach (Do_Constants.Str_ForeignKeyRelation Fk in this.mList_ForeignKey)
                { Dr[Fk.Child_Key] = this.mObj_Base.pDr[Fk.Parent_Key]; }
            }

            this.mObj_List.Save(Da);
        }

        #endregion

        #region _Properties

        public string pName
        {
            get { return this.mName; }
        }

        public ClsBase_List pObj_List
        {
            get { return this.mObj_List; }
        }

        #endregion
    }
}
