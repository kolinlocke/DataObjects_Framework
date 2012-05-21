using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Layer01_Common;
using Layer01_Common.Common;
using Layer02_Objects;
using Layer02_Objects._System;
using Layer02_Objects.Modules_Base;
using Layer02_Objects.Modules_Base.Abstract;

namespace Layer02_Objects.Modules_Objects
{
    public class ClsApplicant : ClsBase
    {
        #region _Constructor

        public ClsApplicant()
        { this.Setup(null, "RecruitmentTestApplicant", ""); }

        #endregion
    }
}
