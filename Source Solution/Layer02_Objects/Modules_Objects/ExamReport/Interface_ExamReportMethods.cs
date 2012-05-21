using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Layer02_Objects.Modules_Objects.ExamReport
{
    public interface Interface_ExamReportMethods
    {
        DataTable GetReport(string Sort = "");
    }
}
