using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public interface IUserContext
    {
        SystemUser CurrentUser { get; set; }

        T Resolve<T>();

        bool IsAuthenticated { get; set; }

        bool HasPermission(PermissionRecord permissionRecord);
        bool HasPermission(String permissionName);
    }
}
