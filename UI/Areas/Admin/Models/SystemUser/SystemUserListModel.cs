using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCMS.CoreBusinessLogic.Paging;
using SCMS.CoreBusinessLogic.Web;

namespace SCMS.UI.Areas.Admin.Models.SystemUser
{
    public class SystemUserListModel : BaseModel
    {
        public SystemUserListModel()
        {
            AvailableRoles=new List<SelectListItem>();
        }

        public string Email { get; set; }
        public bool? Active { get; set; }
        public bool? Locked { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; }
        public Guid[] SelectedRoleIds { get; set; }

        public IPagedList<Model.SystemUser> Users { get; set; }
        public string PagingUrl { get; set; }
        public string PagingUrlPageKey { get; set; }
    }
}