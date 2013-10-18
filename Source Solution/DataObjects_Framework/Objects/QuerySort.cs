using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects_Framework.Objects
{
    [Serializable()]
    public class QuerySort
    {
        #region _Variables

        List<Str_QuerySort> mSort = new List<Str_QuerySort>();

        [Serializable()]
        public struct Str_QuerySort
        {
            public String FieldName;
            public Boolean IsAscending;
        }

        #endregion

        #region _Methods

        public void Add(String Name, Boolean IsAscending)
        {
            if (!this.mSort.Exists(O => O.FieldName == Name))
            { return; }

            this.mSort.Add(new Str_QuerySort() { FieldName = Name, IsAscending = IsAscending }); 
        }

        public String GetSort_String()
        {
            StringBuilder Sb_Sort = new StringBuilder();
            Boolean IsStart = false;
            Char Comma = ' ';
            foreach (Str_QuerySort Item in this.mSort)
            {
                Sb_Sort.Append(Comma + " " + Item.FieldName + " " + (Item.IsAscending ? "Ascending" : "Descending") + " ");

                if (!IsStart)
                { 
                    IsStart = true;
                    Comma = ',';
                }
            }

            return Sb_Sort.ToString();
        }

        #endregion

    }
}
