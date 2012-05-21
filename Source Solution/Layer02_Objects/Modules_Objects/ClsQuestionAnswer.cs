using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer02_Objects;
using Layer02_Objects._System;
using Layer02_Objects.Modules_Base;
using Layer02_Objects.Modules_Base.Abstract;

namespace Layer02_Objects.Modules_Objects
{
    public class ClsQuestionAnswer : ClsBase_List
    {
        #region _Constructor

        public ClsQuestionAnswer()
        {
            Layer01_Common.Objects.ClsQueryCondition Qc = base.pDa.CreateQueryCondition();
            Qc.Add("IsDeleted", "= 0", typeof(bool).ToString(), "0");
            //this.Setup(null, "RecruitmentTestQuestionAnswers", "", Qc);
            this.Setup(null, "RecruitmentTestQuestionAnswers", "uvw_RecruitmentTestQuestionAnswers", Qc);
        }

        #endregion
    }
}
