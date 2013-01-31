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
	/// Internal, manages the defined table detail
	/// </summary>
	internal class BaseTableDetail
	{
		#region _Variables

		string mHeaderName;
		string mTableName;
		string mViewName;
		List<string> mList_Key = new List<string>();
        List<Do_Constants.Str_ForeignKeyRelation> mList_ForeignKey = new List<Do_Constants.Str_ForeignKeyRelation>();
		Base mObj_Base;
        bool mIsCustomKeys = false;

		string mOtherLoadCondition;
		DataTable mDt;

		Interface_DataAccess mDa;

		#endregion

		#region _Constructor

		private BaseTableDetail() { }

		/// <summary>
		/// Constructor for this class
		/// </summary>
		/// <param name="Obj_Base">
		/// Parent Object Base
		/// </param>
		/// <param name="HeaderName">
		/// Parent Table Name, (apparently not used at all)
		/// </param>
		/// <param name="TableName">
		/// Table Detail Table Name
		/// </param>
		/// <param name="ViewName">
		/// Table Detail View Name
		/// </param>
		/// <param name="OtherLoadCondition">
		/// Additional conditions for fetching
		/// </param>
        /// <param name="CustomKeys">
        /// Custom Key definition
        /// </param>
        /// <param name="CustomForeignKeys">
        /// Custom Foreign Key definition
        /// </param>
        public BaseTableDetail(
            Base Obj_Base
            , string HeaderName
            , string TableName
            , string ViewName = ""
            , string OtherLoadCondition = ""
            , List<string> CustomKeys = null
            , List<Do_Constants.Str_ForeignKeyRelation> CustomForeignKeys = null)
        {
            if (ViewName == "") ViewName = TableName;

            this.mHeaderName = HeaderName;
            this.mTableName = TableName;
            this.mViewName = ViewName;
            this.mOtherLoadCondition = OtherLoadCondition;
            this.mList_ForeignKey = CustomForeignKeys;
            this.mObj_Base = Obj_Base;
            this.mDa = Obj_Base.pDa;

            this.mIsCustomKeys = CustomKeys != null;
            if (CustomKeys == null)
            {
                DataTable Dt_Def = this.mDa.GetTableDef(this.mTableName);
                DataRow[] ArrDr = Dt_Def.Select("IsPk = 1");
                foreach (DataRow Dr in ArrDr)
                { this.mList_Key.Add((string)Dr["ColumnName"]); }
            }
            else
            { this.mList_Key = CustomKeys; }
        }

		#endregion

		#region _Methods

		/// <summary>
		/// Loads the table detail data
		/// </summary>
		/// <param name="Da">
		/// An open DataAccess object from calling method
		/// </param>
		/// <param name="Keys">
		/// Key Object to use
		/// </param>
        public void Load(Interface_DataAccess Da, Keys Keys)
        { this.mDt = Da.Load_TableDetails(this.mViewName, Keys, this.mOtherLoadCondition, this.mList_ForeignKey); }

        /// <summary>
		/// Saves the changes to the detail table
		/// </summary>
		/// <param name="Da">
		/// An open DataAccess from calling method
		/// </param>
		public void Save(Interface_DataAccess Da)
		{
			DataRow[] ArrDr = this.mDt.Select("", "", DataViewRowState.CurrentRows);
			foreach (DataRow Dr in ArrDr)
			{
				if (Dr.RowState == DataRowState.Added || Dr.RowState == DataRowState.Modified)
				{
                    /*
					foreach (string Header_Key in this.mObj_Base.pHeader_Key)
					{
                        Int64 Inner_ID = Do_Methods.Convert_Int64(this.mObj_Base.pDr[Header_Key]);
						Dr[Header_Key] = Inner_ID;
					}
                    */

                    if (!this.mIsCustomKeys) 
                    {
                        foreach (string Header_Key in this.mObj_Base.pHeader_Key)
                        {
                            Int64 Inner_ID = Do_Methods.Convert_Int64(this.mObj_Base.pDr[Header_Key]);
                            Dr[Header_Key] = Inner_ID;
                        }
                    }
                    else 
                    {
                        foreach (Do_Constants.Str_ForeignKeyRelation Inner_Keys in this.mList_ForeignKey)
                        {
                            Int64 Inner_ID = Do_Methods.Convert_Int64(this.mObj_Base.pDr[Inner_Keys.Parent_Key]);
                            Dr[Inner_Keys.Child_Key] = Inner_ID;
                        }
                    }
                    
                    Da.SaveDataRow(Dr, this.mTableName, "", false, this.mIsCustomKeys ? this.mList_Key : null);
				}
			}

			ArrDr = this.mDt.Select("", "", DataViewRowState.Deleted);
			foreach (DataRow Dr in ArrDr)
			{
				DataRow Nr = Dr.Table.NewRow();
				foreach (DataColumn Dc in Dr.Table.Columns)
				{
					Nr[Dc.ColumnName] = Dr[Dc.ColumnName, DataRowVersion.Original];
				}

				bool IsPKComplete = true;
				foreach (string Key in this.mList_Key)
				{
					if (Information.IsDBNull(Dr[Key]))
					{
						IsPKComplete = false;
						break;
					}
				}

                if (IsPKComplete)
                { Da.SaveDataRow(Dr, this.mTableName, "", true, this.mIsCustomKeys ? this.mList_Key : null); }
			}
		}

		#endregion

		#region _Properties

		/// <summary>
		/// Gets the defined table name for this detail table
		/// </summary>
		public string pTableName
		{
            get { return this.mTableName; }
		}

		/// <summary>
		/// Get/Set Property, gets or sets the datatable object for this detail table
		/// </summary>
		public DataTable pDt
		{
            get { return this.mDt; }
            set { this.mDt = value; }
		}

		#endregion
	}
}
