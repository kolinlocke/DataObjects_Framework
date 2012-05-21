using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Layer02_Objects.Modules_Objects.ExamReport
{
    [Serializable()]
    public class ClsExamReport
    {
        #region _Variables

        Interface_ExamReportMethods Erm = new ClsExamReportMethods_SqlServer();

        #endregion

        #region _Methods

        public DataTable GetReport(string Sort = "")
        { return this.Erm.GetReport(Sort); }

        #endregion
    }
}
