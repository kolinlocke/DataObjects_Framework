using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Layer01_Common;
using Layer01_Common.Common;

namespace Layer01_Common.Objects
{
    public class ClsExcel_Columns
    {

        #region _Variables

        public struct Str_Columns
        {
            public string FieldName;
            public string FieldDesc;
            public string NumberFormat;
            public Int32 Width;

            public Str_Columns(
                string pFieldName
                , string pFieldDesc
                , string pNumberFormat
                , Int32 pWidth)
            {
                this.FieldName = pFieldName;
                this.FieldDesc = pFieldDesc;
                this.NumberFormat = pNumberFormat;
                this.Width = pWidth;
            }
        }

        List<Str_Columns?> mObj = new System.Collections.Generic.List<Str_Columns?>();

        #endregion

        #region _Methods

        public void Add(
            string FieldName
            , string FieldDesc = ""
            , string NumberFormat = ""
            , Int32 Width = 100)
        {
            if (this.pObj_Get(FieldName) != null)
            { return; }
            this.mObj.Add(new Str_Columns(FieldName, FieldDesc, NumberFormat, Width));
        }

        public void GetColumns(string Name)
        {
            DataTable Dt =
                Methods_Query.GetQuery(
                    @"udf_System_BindDefinition('" + Name + @"')"
                    , ""
                    , "IsNull(IsHidden,0) = 0"
                    , "OrderIndex");

            foreach (DataRow Dr in Dt.Rows)
            {
                string FieldName = (string)Layer01_Methods.IsNull(Dr["Name"], "");
                string FieldDesc = (string)Layer01_Methods.IsNull(Dr["Desc"], "");
                string NumberFormat = (string)Layer01_Methods.IsNull(Dr["NumberFormat"], "");

                if (NumberFormat != "")
                {
                    NumberFormat = NumberFormat.Replace("{", "");
                    NumberFormat = NumberFormat.Replace("}", "");
                    string[] Arr = NumberFormat.Split(':');

                    try
                    { NumberFormat = Arr[1]; }
                    catch { }

                    Regex Reg = new Regex(@"N[0-9]", RegexOptions.IgnoreCase);
                    if (Reg.Match(NumberFormat).Success)
                    {
                        Int32 X = 0;
                        try
                        { X = Convert.ToInt32(NumberFormat.Substring(2, 1)); }
                        catch { }

                        NumberFormat = "#,##0";
                        if (X != 0)
                        { NumberFormat.PadRight(X, '0'); }                        
                    }

                    this.Add(FieldName, FieldDesc, NumberFormat);
                }
            }
        }

        #endregion

        #region _Properties

        public Str_Columns? pObj_Get(string Name)
        {
            Str_Columns? Rv = null;
            foreach (Str_Columns Obj in this.mObj)
            {
                if (Obj.FieldName == Name)
                { 
                    Rv = Obj;
                    break;
                }
            }

            return Rv;
        }

        public List<Str_Columns?> pObj
        {
            get { return this.mObj; }
        }

        public string[] pFieldName
        {
            get
            {
                List<string> Rv = new List<string>();
                foreach (Str_Columns Obj in this.mObj)
                { Rv.Add(Obj.FieldName); }
                return Rv.ToArray();
            }
        }

        public string[] pFieldDesc
        {
            get
            {
                List<string> Rv = new List<string>();
                foreach (Str_Columns Obj in this.mObj)
                { Rv.Add(Obj.FieldDesc); }
                return Rv.ToArray();
            }
        }

        public string[] pNumberFormat
        {
            get
            {
                List<string> Rv = new List<string>();
                foreach (Str_Columns Obj in this.mObj)
                { Rv.Add(Obj.NumberFormat); }
                return Rv.ToArray();
            }
        }

        #endregion

    }
}
