using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;
using Layer01_Common.Connection;
using Layer01_Common.Objects;

namespace Layer02_Objects.Modules_Objects.ExamReport 
{
    public class ClsExamReportMethods_SqlServer : Interface_ExamReportMethods
    {
        public DataTable GetReport(string Sort = "")
        {
            DataTable Dt  = null;

            Layer02_Objects.Modules_Base.Abstract.ClsBase Obj_Base = new Modules_Base.Abstract.ClsBase();
            Layer02_Objects.DataAccess.Interface_DataAccess Da = Obj_Base.pDa;
            Dt = Da.GetQuery("uvw_RecruitmentTestExams_Scores_Desc", "", "", Sort);

            return Dt;
        }
    }
}
