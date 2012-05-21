using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.VisualBasic;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using DataObjects_Framework;
using DataObjects_Framework._System;
using DataObjects_Framework.DataAccess;
using DataObjects_Framework.Modules_Base;
using DataObjects_Framework.Modules_Base.Abstract;
using DataObjects_Framework.Modules_Base.Objects;

namespace DataObjects_Framework.Modules_Base.Objects
{
	/// <summary>
	/// Internal, manages the defined table detail
	/// </summary>
	public class ClsBaseTableDetail
	{
		#region _Variables

		string mHeaderName;
		string mTableName;
		string mViewName;
		List<string> mList_Key = new List<string>();
		ClsBase mObj_Base;

		string mOtherLoadCondition;
		DataTable mDt;

		Interface_DataAccess mDa;

		#endregion

		#region _Constructor

		private ClsBaseTableDetail() { }

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
		/// Detail Table Name
		/// </param>
		/// <param name="ViewName">
		/// Detail View Name
		/// </param>
		/// <param name="OtherLoadCondition">
		/// Additional conditions for fetching
		/// </param>
		public ClsBaseTableDetail(
			ClsBase Obj_Base
			, string HeaderName
			, string TableName
			, string ViewName = ""
			, string OtherLoadCondition = "")
		{
			if (ViewName == "") ViewName = TableName;

			this.mHeaderName = HeaderName;
			this.mTableName = TableName;
			this.mViewName = ViewName;
			this.mOtherLoadCondition = OtherLoadCondition;
			this.mObj_Base = Obj_Base;
			this.mDa = Obj_Base.pDa;

			DataTable Dt_Def = this.mDa.GetTableDef(this.mTableName);
			DataRow[] ArrDr = Dt_Def.Select("IsPk = 1");
			foreach (DataRow Dr in ArrDr)
			{ this.mList_Key.Add((string)Dr["ColumnName"]); }
		}

		#endregion

		#region _Methods

		/// <summary>
		/// Loads the detail table
		/// </summary>
		/// <param name="Da">
		/// An open DataAccess from calling method
		/// </param>
		/// <param name="Keys">
		/// Key Object to use
		/// </param>
		public void Load(Interface_DataAccess Da, ClsKeys Keys)
		{ this.mDt = Da.Load_TableDetails(this.mViewName, Keys, this.mOtherLoadCondition); }

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
					foreach (string Header_Key in this.mObj_Base.pHeader_Key)
					{
						Int64 Inner_ID = (Int64)Layer01_Methods.IsNull(this.mObj_Base.pDr[Header_Key], 0);
						Dr[Header_Key] = Inner_ID;
					}
					Da.SaveDataRow(Dr, this.mTableName);
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
				{ Da.SaveDataRow(Dr, this.mTableName, "", true); }
			}
		}

		#endregion

		#region _Properties

		/// <summary>
		/// Gets the defined table name for this detail table
		/// </summary>
		public string pTableName
		{
			get
			{ return this.mTableName; }
		}

		/// <summary>
		/// Get/Set Property, gets or sets the datatable object for this detail table
		/// </summary>
		public DataTable pDt
		{
			get
			{ return this.mDt; }
			set
			{ this.mDt = value; }
		}

		#endregion
	}
}
