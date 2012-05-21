using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Connection;
using Layer02_Objects;
using Layer02_Objects.DataAccess;

namespace Layer02_Objects.Modules_Objects.Exam
{
    public interface Interface_ExamMethods 
    {
        DataSet GenerateExam(Int64 QuestionLimit, Int64 CategoryID);

        DataSet LoadExam(Int64 ExamID);

        Int64 ComputeScore(DataSet Ds_Exam);
    }
}
