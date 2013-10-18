using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using DataObjects_Framework;
using DataObjects_Framework.Common;

namespace DataObjects_Framework.Objects
{
	/// <summary>
	/// A simplified Data Set Object
	/// </summary>
    [DataContract()]
    public class SimpleDataSet
    {
        #region _Variables

        List<SimpleDataTable> mList_DataTable = new List<SimpleDataTable>();

        #endregion

        #region _Constructor

		/// <summary>
		/// Constructor for ClsSimpleDataSet object
		/// </summary>
        public SimpleDataSet() { }

		/// <summary>
		/// Constructor for ClsSimpleDataSet object with the supplied DataSet
		/// Creates a new ClsSimpleDataSet with data populated from the dataset
		/// </summary>
		/// <param name="Ds">
		/// The data set to be used
		/// </param>
        public SimpleDataSet(DataSet Ds) 
        {
            foreach (DataTable Dt in Ds.Tables)
            {
                SimpleDataTable Sdt = new SimpleDataTable(Dt);
                this.mList_DataTable.Add(Sdt);
            }
        }
        
        #endregion

        #region _Methods

		/// <summary>
		/// Returns the serialized format for this object
		/// </summary>
		/// <returns></returns>
        public string Serialize()
        { return Do_Methods.SerializeObject_Json(typeof(SimpleDataSet), this); }

		/// <summary>
		/// Returns a new ClsSimpleDataSet object from a serialized data
		/// </summary>
		/// <param name="SerializeData">
		/// The data to be deserialized
		/// </param>
		/// <returns></returns>
        public static SimpleDataSet Deserialize(string SerializeData)
        { return (SimpleDataSet)Do_Methods.DeserializeObject_Json(typeof(SimpleDataSet), SerializeData); }

		/// <summary>
		/// Returns a data set converted from this object
		/// </summary>
		/// <returns></returns>
        public DataSet ToDataSet()
        {
            DataSet Ds = new DataSet();
            foreach (SimpleDataTable Sdt in this.mList_DataTable)
            {
                DataTable Dt = Sdt.ToDataTable();
                Ds.Tables.Add(Dt);
            }

            return Ds;
        }

        #endregion

        #region _Properties

		/// <summary>
		/// List of ClsSimpleDataTable objects contained in this object
		/// </summary>
        [DataMember()]
        public List<SimpleDataTable> pList_DataTable
        {
            get { return this.mList_DataTable; }
        }

        #endregion
    }

	/// <summary>
	/// A simplified Data Table object
	/// </summary>
    [DataContract()]
    public class SimpleDataTable
    {
        #region _Variables

        List<SimpleDataColumn> mList_DataColumn = new List<SimpleDataColumn>();

        List<SimpleDataRow> mList_DataRow = new List<SimpleDataRow>();

        #endregion

        #region _Constructor

		/// <summary>
		/// Constructor for ClsSimpleDataTable
		/// </summary>
        public SimpleDataTable() { }

		/// <summary>
		/// Constructor for ClsSimpleDataTable
		/// Populates data from the supplied data table
		/// </summary>
		/// <param name="Dt"></param>
        public SimpleDataTable(DataTable Dt)
        {
            foreach (DataColumn Dc in Dt.Columns)
            {
                this.mList_DataColumn.Add(
                    new SimpleDataColumn() { ColumnName = Dc.ColumnName, DataType = Dc.DataType });
            }

            foreach (DataRow Dr in Dt.Rows)
            {
                this.mList_DataRow.Add(new SimpleDataRow(this.mList_DataColumn, Dr));
            }
        }

        #endregion

        #region _Methods

		/// <summary>
		/// Returns the serialized format for this object
		/// </summary>
		/// <returns></returns>
        public string Serialize()
        { return Do_Methods.SerializeObject_Json(typeof(SimpleDataTable), this); }

		/// <summary>
		/// Returns a new ClsSimpleDataTable object from a serialized data
		/// </summary>
		/// <param name="SerializedData">
		/// The data to be deserialized
		/// </param>
		/// <returns></returns>
        public static SimpleDataTable Deserialize(string SerializedData)
        { return (SimpleDataTable)Do_Methods.DeserializeObject_Json(typeof(SimpleDataTable), SerializedData); }

		/// <summary>
		/// Returns a new ClsSimpleDataRow object based on the columns defined in this object
		/// </summary>
		/// <returns></returns>
        public SimpleDataRow NewRow()
        { return new SimpleDataRow(this.mList_DataColumn); }

		/// <summary>
		/// Returns a new ClsSimpleDataRow object with data populated from the supplied data row
		/// </summary>
		/// <param name="Dr">
		/// The data row source 
		/// </param>
		/// <returns></returns>
        public SimpleDataRow NewRow(DataRow Dr)
        { return new SimpleDataRow(this.mList_DataColumn, Dr); }

		/// <summary>
		/// Returns a data table converted from this object
		/// </summary>
		/// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable Dt = new DataTable();
            foreach (SimpleDataColumn Sdc in this.mList_DataColumn)
            { Dt.Columns.Add(Sdc.ColumnName, Sdc.DataType); }

            foreach (SimpleDataRow Sdr in this.mList_DataRow)
            {
                DataRow Dr_New = Dt.NewRow();
                Dt.Rows.Add(Dr_New);
                foreach (SimpleDataColumn Sdc in this.mList_DataColumn)
                { Dr_New[Sdc.ColumnName] = Do_Methods.IsNull(Sdr[Sdc.ColumnName], DBNull.Value); }
            }

            return Dt;
        }

        #endregion

        #region _Properties

		/// <summary>
		/// A list of ClsSimpleDataColumn defined in this object
		/// </summary>
        [DataMember()]
        public List<SimpleDataColumn> pList_DataColumn
        {
            get { return this.mList_DataColumn; }
        }

		/// <summary>
		/// A list of ClsSimpleDataRow contained in this object
		/// </summary>
        [DataMember()]
        public List<SimpleDataRow> pList_DataRow
        {
            get { return this.mList_DataRow; }
        }

        #endregion
    }
	
    [DataContract()]
    public class SimpleDataColumn
    {
        #region _Constructor

        public SimpleDataColumn() { }

        #endregion

        #region _Variables

        [DataMember()]
        public string ColumnName;

        [DataMember()]
        public Type DataType;

        #endregion
    }

    [DataContract()]
    public class SimpleDataRow
    {
        #region _Variables

        [DataContract()]
        public struct Str_Item
        {
            [DataMember()]
            public SimpleDataColumn DataColumn;

            [DataMember()]
            public Object Value;
        }

        [DataMember()]
        List<Str_Item> mList_Item = new List<Str_Item>();

        #endregion

        #region _Constructor

        public SimpleDataRow() { }

        public SimpleDataRow(List<SimpleDataColumn> List_DataColumn, DataRow Dr)
        {
            foreach (DataColumn Dc in Dr.Table.Columns)
            {
                SimpleDataColumn Sdc = List_DataColumn.FirstOrDefault(X => X.ColumnName == Dc.ColumnName);
                if (Sdc != null)
                {
                    this.mList_Item.Add(new Str_Item() { DataColumn = Sdc, Value = Dr[Dc.ColumnName] });
                }
            }
        }

        public SimpleDataRow(List<SimpleDataColumn> List_DataColumn)
        {
            foreach (SimpleDataColumn Sdc in List_DataColumn)
            {
                this.mList_Item.Add(new Str_Item() { DataColumn = Sdc });
            }
        }

        public SimpleDataRow(DataRow Dr)
        {
            foreach (DataColumn Dc in Dr.Table.Columns)
            {
                SimpleDataColumn Sdc = new SimpleDataColumn();
                Sdc.ColumnName = Dc.ColumnName;
                Sdc.DataType = Dc.DataType;
                this.mList_Item.Add(
                    new Str_Item() { DataColumn = Sdc, Value = Dr[Dc.ColumnName] });
            }
        }

        #endregion

        #region _Methods

        public string Serialize()
        {
            return Do_Methods.SerializeObject_Json(typeof(SimpleDataRow), this);
        }

        public static SimpleDataRow Deserialize(String SerializedData)
        { return (SimpleDataRow)Do_Methods.DeserializeObject_Json(typeof(SimpleDataRow), SerializedData); }

        public DataRow ToDataRow()
        {
            DataTable Dt = new DataTable();
            foreach (SimpleDataColumn Sdc in (from O in this.mList_Item select O.DataColumn))
            { Dt.Columns.Add(Sdc.ColumnName, Sdc.DataType); }

            DataRow Dr = Dt.NewRow();
            Dt.Rows.Add(Dr);

            foreach (Str_Item Item in this.mList_Item)
            { Dr[Item.DataColumn.ColumnName] = Do_Methods.IsNull(Item.Value, DBNull.Value); }

            return Dr;
        }

        #endregion

        #region _Properties

        public Object this[string Name]
        {
            get { return this.mList_Item.FirstOrDefault(X => X.DataColumn.ColumnName == Name).Value; }
        }

        public Object this[Int32 Index]
        {
            get { return this.mList_Item[Index].Value; }
        }

        #endregion
    }
}
