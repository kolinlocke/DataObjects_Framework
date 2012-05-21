using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Layer02_Objects._System.CurrentUser
{
    public interface Interface_CurrentUserMethods
    {
        string GetAdministratorPassword();

        bool CheckAccess(Layer02_Constants.eSystem_Modules System_ModulesID, Layer02_Constants.eAccessLib AccessType, Int64 UserID);
    }
}
