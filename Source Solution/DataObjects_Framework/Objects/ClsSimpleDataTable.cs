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
    public class ClsSimpleDataTable
    {
        List<ClsSimpleDataColumn> mList_DataColumn = new List<ClsSimpleDataColumn>();

        List<ClsSimpleDataRow> mList_DataRow = new List<ClsSimpleDataRow>();

        public ClsSimpleDataTable() { }

        public ClsSimpleDataTable(DataTable Dt)
        {
            foreach (DataColumn Dc in Dt.Columns)
            {
                this.mList_DataColumn.Add(
                    new ClsSimpleDataColumn()
                    {
                        ColumnName = Dc.ColumnName
                        ,
                        DataType = Dc.DataType
                    });
            }

            foreach (DataRow Dr in Dt.Rows)
            {
                this.mList_DataRow.Add(new ClsSimpleDataRow(this.mList_DataColumn, Dr));
            }
        }

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

        public string Serialize()
        { return Do_Methods.SerializeObject_Json(typeof(ClsSimpleDataTable), this); }
    }

    [DataContract()]
    public class ClsSimpleDataColumn
    {
        public ClsSimpleDataColumn() { }

        [DataMember()]
        public string ColumnName;

        [DataMember()]
        public Type DataType;
    }

    [DataContract()]
    public class ClsSimpleDataRow
    {
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
        
        public string Serialize()
        {
            return Do_Methods.SerializeObject_Json(typeof(ClsSimpleDataRow), this);
        }

        public Object this[string Name]
        {
            get { return this.mList_Item.FirstOrDefault(X => X.DataColumn.ColumnName == Name).Value; }
        }

        public Object this[Int32 Index]
        {
            get { return this.mList_Item[Index].Value; }
        }
    }
}
