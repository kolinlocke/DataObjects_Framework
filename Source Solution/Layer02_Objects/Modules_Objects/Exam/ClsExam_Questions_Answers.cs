using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;

namespace Layer02_Objects.Modules_Objects.Exam
{
    public class ClsExam_Questions_Answers
    {
        DataRow mDr;
        
        public ClsExam_Questions_Answers(DataRow Dr_Answer)
        {
            this.mDr = Dr_Answer;
        }

        public Int64 pCt
        {
            get { return (Int64)Layer01_Methods.IsNull(this.mDr["Ct"], 0); }
        }

        public string pAnswer
        {
            get { return (string)Layer01_Methods.IsNull(this.mDr["Lkp_RecruitmentTestAnswersID_Desc"], ""); }
        }

        public bool pIsAnswer
        {
            get { return (bool)Layer01_Methods.IsNull(this.mDr["IsAnswer"], false); }
        }

        public bool pIsAnswered
        {
            set { this.mDr["IsAnswered"] = value; }
            get { return (bool)Layer01_Methods.IsNull(this.mDr["IsAnswered"], false); }
        }

    }
}
