using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Objects;

namespace Layer01_Common.Common
{
    public class Methods_Excel
    {
        public static System.IO.StringWriter CreateExcel_HTML(
            DataTable Dt
            , ClsExcel_Columns Columns)
        {
            System.IO.StringWriter Sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter Htw = new System.Web.UI.HtmlTextWriter(Sw);
            System.Web.UI.WebControls.Table Tb = new System.Web.UI.WebControls.Table();

            System.Web.UI.WebControls.TableRow Tbr_Header = new System.Web.UI.WebControls.TableRow();
            foreach (ClsExcel_Columns.Str_Columns? Obj in Columns.pObj)
            {
                System.Web.UI.WebControls.TableCell Tbc = new System.Web.UI.WebControls.TableCell();
                Tbc.Text = Obj.Value.FieldDesc;
                Tbc.Width = Obj.Value.Width;
                Tbr_Header.Cells.Add(Tbc);
            }

            Tb.Rows.Add(Tbr_Header);

            foreach (DataRow Dr in Dt.Rows)
            {
                System.Web.UI.WebControls.TableRow Tbr = new System.Web.UI.WebControls.TableRow();
                foreach (ClsExcel_Columns.Str_Columns? Obj in Columns.pObj)
                {
                    System.Web.UI.WebControls.TableCell Tbc = new System.Web.UI.WebControls.TableCell();
                    Tbc.Text = Dr[Obj.Value.FieldName].ToString();
                    Tbr.Cells.Add(Tbc);
                }
                Tb.Rows.Add(Tbr);
            }
            Tb.RenderControl(Htw);
            return Sw;
        }

    }
}
