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
    [DataContract()]
    public class ClsSimpleDataSet
    {
        #region _Variables

        List<ClsSimpleDataTable> mList_DataTable = new List<ClsSimpleDataTable>();

        #endregion

        #region _Constructor

        public ClsSimpleDataSet() { }

        public ClsSimpleDataSet(DataSet Ds) 
        {
            foreach (DataTable Dt in Ds.Tables)
            {
                ClsSimpleDataTable Sdt = new ClsSimpleDataTable(Dt);
                this.mList_DataTable.Add(Sdt);
            }
        }
        
        #endregion

        #region _Methods

        public string Serialize()
        { return Do_Methods.SerializeObject_Json(typeof(ClsSimpleDataSet), this); }

        public static ClsSimpleDataSet Deserialize(string SerializeData)
        { return (ClsSimpleDataSet)Do_Methods.DeserializeObject_Json(typeof(ClsSimpleDataSet), SerializeData); }

        public DataSet ToDataSet()
        {
            DataSet Ds = new DataSet();
            foreach (ClsSimpleDataTable Sdt in this.mList_DataTable)
            {
                DataTable Dt = Sdt.ToDataTable();
                Ds.Tables.Add(Dt);
            }

            return Ds;
        }

        #endregion

        #region _Properties

        [DataMember()]
        public List<ClsSimpleDataTable> pList_DataTable
        {
            get { return this.mList_DataTable; }
        }

        #endregion
    }

    [DataContract()]
    public class ClsSimpleDataTable
    {
        #region _Variables

        List<ClsSimpleDataColumn> mList_DataColumn = new List<ClsSimpleDataColumn>();

        List<ClsSimpleDataRow> mList_DataRow = new List<ClsSimpleDataRow>();

        #endregion

        #region _Constructor

        public ClsSimpleDataTable() { }

        public ClsSimpleDataTable(DataTable Dt)
        {
            foreach (DataColumn Dc in Dt.Columns)
            {
                this.mList_DataColumn.Add(
                    new ClsSimpleDataColumn() { ColumnName = Dc.ColumnName, DataType = Dc.DataType });
            }

            foreach (DataRow Dr in Dt.Rows)
            {
                this.mList_DataRow.Add(new ClsSimpleDataRow(this.mList_DataColumn, Dr));
            }
        }

        #endregion

        #region _Methods

        public string Serialize()
        { return Do_Methods.SerializeObject_Json(typeof(ClsSimpleDataTable), this); }

        public static ClsSimpleDataTable Deserialize(string SerializedData)
        { return (ClsSimpleDataTable)Do_Methods.DeserializeObject_Json(typeof(ClsSimpleDataTable), SerializedData); }

        public ClsSimpleDataRow NewRow()
        { return new ClsSimpleDataRow(this.mList_DataColumn); }

        public ClsSimpleDataRow NewRow(DataRow Dr)
        { return new ClsSimpleDataRow(this.mList_DataColumn, Dr); }

        public DataTable ToDataTable()
        {
            DataTable Dt = new DataTable();
            foreach (ClsSimpleDataColumn Sdc in this.mList_DataColumn)
            { Dt.Columns.Add(Sdc.ColumnName, Sdc.DataType); }

            foreach (ClsSimpleDataRow Sdr in this.mList_DataRow)
            {
                DataRow Dr_New = Dt.NewRow();
                Dt.Rows.Add(Dr_New);
                foreach (ClsSimpleDataColumn Sdc in this.mList_DataColumn)
                { Dr_New[Sdc.ColumnName] = Sdr[Sdc.ColumnName]; }
            }

            return Dt;
        }

        #endregion

        #region _Properties

        [DataMember()]
        public List<ClsSimpleDataColumn> pList_DataColumn
        {
            get { return this.mList_DataColumn; }
        }

        [DataMember()]
        public List<ClsSimpleDataRow> pList_DataRow
        {
            get { return this.mList_DataRow; }
        }

        #endregion
    }

    [DataContract()]
    public class ClsSimpleDataColumn
    {
        #region _Constructor

        public ClsSimpleDataColumn() { }

        #endregion

        #region _Variables

        [DataMember()]
        public string ColumnName;

        [DataMember()]
        public Type DataType;

        #endregion
    }

    [DataContract()]
    public class ClsSimpleDataRow
    {
        #region _Variables

        [DataContract]
        public struct Str_Item
        {
            [DataMember]
            public ClsSimpleDataColumn DataColumn;

            [DataMember]
            public Object Value;
        }

        [DataMember()]
        List<Str_Item> mList_Item = new List<Str_Item>();

        #endregion

        #region _Constructor

        public ClsSimpleDataRow() { }

        internal ClsSimpleDataRow(List<ClsSimpleDataColumn> List_DataColumn, DataRow Dr)
        {
            foreach (DataColumn Dc in Dr.Table.Columns)
            {
                ClsSimpleDataColumn Sdc = List_DataColumn.FirstOrDefault(X => X.ColumnName == Dc.ColumnName);
                if (Sdc != null)
                {
                    this.mList_Item.Add(new Str_Item() { DataColumn = Sdc, Value = Dr[Dc.ColumnName] });
                }
            }
        }

        internal ClsSimpleDataRow(List<ClsSimpleDataColumn> List_DataColumn)
        {
            foreach (ClsSimpleDataColumn Sdc in List_DataColumn)
            {
                this.mList_Item.Add(new Str_Item() { DataColumn = Sdc });
            }
        }

        internal ClsSimpleDataRow(DataRow Dr)
        {
            foreach (DataColumn Dc in Dr.Table.Columns)
            {
                ClsSimpleDataColumn Sdc = new ClsSimpleDataColumn();
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
            return Do_Methods.SerializeObject_Json(typeof(ClsSimpleDataRow), this);
        }

        public static ClsSimpleDataRow Deserialize(String SerializedData)
        { return (ClsSimpleDataRow)Do_Methods.DeserializeObject_Json(typeof(ClsSimpleDataRow), SerializedData); }

        public DataRow ToDataRow()
        {
            DataTable Dt = new DataTable();
            foreach (ClsSimpleDataColumn Sdc in (from O in this.mList_Item select O.DataColumn))
            { Dt.Columns.Add(Sdc.ColumnName, Sdc.DataType); }

            DataRow Dr = Dt.NewRow();
            Dt.Rows.Add(Dr);

            foreach (Str_Item Item in this.mList_Item)
            { Dr[Item.DataColumn.ColumnName] = Item.Value; }

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
