using System;
using SCMS.CoreBusinessLogic.Web;

namespace SCMS.UI.Areas.Admin.Models.Security
{
    public class PermissionRecordModel : BaseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}