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
using DataObjects_Framework.BaseObjects;

namespace DataObjects_Framework.BaseObjects
{
	/// <summary>
	/// Internal, manages the defined list object
	/// </summary>
	internal class BaseListObject
	{
		#region _Variables

		Base_List mObj_Base;
		Interface_DataAccess mDa;

		string mName;
		Base mTemplate_Obj;
		List<Object> mTemplate_Obj_Constructors = new List<Object>();
		string mTemplate_ViewName;
		QueryCondition mTemplate_LoadCondition;
        List<Do_Constants.Str_ForeignKeyRelation> mTemplate_FetchKeys;
		List<Do_Constants.Str_ForeignKeyRelation> mTemplate_ForeignKeys;

		DataTable mDt_Obj;
		List<Str_Obj> mList_Obj = new List<Str_Obj>();

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
			public Base Obj;

			/// <summary>
			/// Constructor for Str_Obj
			/// </summary>
			/// <param name="pName"></param>
			/// <param name="pObj"></param>
			public Str_Obj(string pName, Base pObj)
			{
				this.Name = pName;
				this.Obj = pObj;
			}
		}

		#endregion

		#region _Constructor

		private BaseListObject() { }

		public BaseListObject(
			Base_List Obj_Base
			, string Name
			, Base Template_Obj
			, List<Object> Template_Obj_Constructors
			, string Template_ViewName
            , List<Do_Constants.Str_ForeignKeyRelation> Template_FetchKeys
			, List<Do_Constants.Str_ForeignKeyRelation> Template_ForeignKeys
			, QueryCondition Template_LoadCondition = null)
		{
			if (!(Template_Obj is Base)) { throw new Exception("Template_Obj must be derived from ClsBase."); }

			this.mObj_Base = Obj_Base;
			this.mDa = Obj_Base.pDa;

            this.mName = Name;
			this.mTemplate_Obj = Template_Obj;
			this.mTemplate_Obj_Constructors = Template_Obj_Constructors;
			this.mTemplate_ViewName = Template_ViewName;
            this.mTemplate_FetchKeys = Template_FetchKeys;
			this.mTemplate_ForeignKeys = Template_ForeignKeys;
			this.mTemplate_LoadCondition = Template_LoadCondition;
		}

		#endregion

		#region _Methods

		public void Load(Interface_DataAccess Da, Keys Keys)
		{
			if (Keys == null)
			{ this.mDt_Obj = Da.List_Empty(this.mTemplate_ViewName); }
			else
			{
				QueryCondition Qc = Da.CreateQueryCondition();
				foreach (string KeyName in Keys.pName)
				{
                    Do_Constants.Str_ForeignKeyRelation Fk = this.mTemplate_FetchKeys.FirstOrDefault(Item => Item.Parent_Key == KeyName);
                    Qc.Add(Fk.Child_Key, Keys[KeyName].ToString(), typeof(Int64).ToString(), "0");
                }

				if (this.mTemplate_LoadCondition != null)
				{
					foreach (QueryCondition.Str_QueryCondition Str_Qc in this.mTemplate_LoadCondition.pList)
					{ Qc.pList.Add(Str_Qc); }
				}

				DataTable Dt = Da.List(this.mTemplate_ViewName, Qc);
				this.mDt_Obj = Dt;
				foreach (DataRow Dr in Dt.Rows)
				{
					Int64 TmpKey = 0;

					StringBuilder Sb_Condition = new StringBuilder();
					Sb_Condition.Append(" 1 = 1 ");
					foreach (Do_Constants.Str_ForeignKeyRelation Fk in this.mTemplate_ForeignKeys)
					{ Sb_Condition.Append(" And " + Fk.Parent_Key + " = " + Dr[Fk.Child_Key] + " "); }

					DataRow[] ArrDr_Parent = this.mObj_Base.pDt_List.Select(Sb_Condition.ToString());
					if (ArrDr_Parent.Length > 0) { TmpKey = Do_Methods.Convert_Int64(ArrDr_Parent[0]["TmpKey"]); }
					else { throw new Exception("TmpKey not found."); }

                    Base Inner_Obj = null;

                    if (this.mTemplate_Obj_Constructors != null)
                    { Inner_Obj = (Base)Activator.CreateInstance(this.mTemplate_Obj.GetType(), this.mTemplate_Obj_Constructors.ToArray()); }
                    else
                    { Inner_Obj = (Base)Activator.CreateInstance(this.mTemplate_Obj.GetType()); }
                    
                    Inner_Obj.Load(Dr);
					this.mList_Obj.Add(new Str_Obj(TmpKey.ToString(), Inner_Obj));
				}
			}
		}

		public void Save(Interface_DataAccess Da) 
		{
            DataRow[] ArrDr = this.mObj_Base.pDt_List.Select("", "", DataViewRowState.CurrentRows);
			foreach (DataRow Dr_Parent in ArrDr)
			{
				Str_Obj Obj = this.mList_Obj.FirstOrDefault(Item => Item.Name == Dr_Parent["TmpKey"].ToString());
                if (Obj.Obj == null) { continue; }

				Obj.Obj.Save(Da);

				foreach (Do_Constants.Str_ForeignKeyRelation Fk in this.mTemplate_ForeignKeys)
				{ Dr_Parent[Fk.Parent_Key] = Obj.Obj.pDr[Fk.Child_Key]; }
			}

            ArrDr = this.mObj_Base.pDt_List.Select("", "", DataViewRowState.Deleted);
            foreach (DataRow Dr_Parent in ArrDr)
            {
                Str_Obj Obj = this.mList_Obj.FirstOrDefault(Item => Item.Name == Dr_Parent["TmpKey"].ToString());
                if (Obj.Obj == null) { continue; }
                Obj.Obj.Delete();
            }
		}

		public Base Add_Object(Int64 TmpKey)
		{
			DataRow Dr = this.mDt_Obj.NewRow();
			this.mDt_Obj.Rows.Add(Dr);

            Base Obj = null;

            if (this.mTemplate_Obj_Constructors != null) 
            { Obj = (Base)Activator.CreateInstance(this.mTemplate_Obj.GetType(), this.mTemplate_Obj_Constructors.ToArray()); }
            else 
            { Obj = (Base)Activator.CreateInstance(this.mTemplate_Obj.GetType()); }

            Obj.Load(Dr);
            this.mList_Obj.Add(new Str_Obj(TmpKey.ToString(), Obj));

			return Obj;
		}

		public void Refresh_Desc(List<Base_List.Str_Desc> List_Desc)
		{
			foreach (DataRow Dr_Parent in this.mObj_Base.pDt_List.Rows)
			{
				DataRow[] ArrDr_Obj = this.mDt_Obj.Select("TmpKey = " + Dr_Parent["TmpKey"].ToString());
				if (ArrDr_Obj.Length > 0)
				{
                    foreach (Base_List.Str_Desc Desc in List_Desc)
                    {
                        if (this.mObj_Base.pDt_List.Columns.Contains(Desc.FieldName_Parent))
                        {
                            if (this.mDt_Obj.Columns.Contains(Desc.FieldName_Child))
                            { Dr_Parent[Desc.FieldName_Parent] = ArrDr_Obj[0][Desc.FieldName_Child]; }
                        }
                    }					
				}
			}
		}

		#endregion

		#region _Properties

		public string pName
		{
			get { return this.mName; }
		}

		public DataTable pDt_Obj
		{
			get { return this.mDt_Obj; }
		}

		public List<Str_Obj> pList_Obj
		{
			get { return this.mList_Obj; }
		}

		#endregion
	}
}
