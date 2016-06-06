using System.Collections.Generic;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.Security
{
    public interface IPermissionProvider
    {
        IEnumerable<PermissionRecord> GetPermissions();

        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}
