using System;
using System.Collections.Generic;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Areas.Admin.Models.Role;

namespace SCMS.UI.Areas.Admin.Models.Security
{
    public class PermissionMappingModel : BaseModel
    {
        public PermissionMappingModel()
        {
            AvailablePermissions = new List<PermissionRecordModel>();
            AvailableRoles = new List<RoleModel>();
            Allowed = new Dictionary<string, IDictionary<Guid, bool>>();
        }
        public IList<PermissionRecordModel> AvailablePermissions { get; set; }
        public IList<RoleModel> AvailableRoles { get; set; }

        //[permission system name] / [customer role id] / [allowed]
        public IDictionary<string, IDictionary<Guid, bool>> Allowed { get; set; }
    }
}