using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Areas.Admin.Validators;
using SCMS.UI.Areas.Admin.Validators.Role;


namespace SCMS.UI.Areas.Admin.Models.Role
{
    [Validator(typeof(RoleValidator))]
    public class RoleModel : BaseModel
    {
        public Guid Id { get; set; }

        [AllowHtml]
        public string Name { get; set; }

        public bool Active { get; set; }

        public bool IsSystemRole { get; set; }

        public string SystemName { get; set; }

        public string Description { get; set; }
    }

    public static class RoleExtensions
    {
        public static RoleModel ToModel(this SCMS.Model.Role role)
        {
            return new RoleModel
                       {
                           Id = role.Id,
                           Name = role.Name,
                           Active = role.Active,
                           IsSystemRole = role.IsSystemRole,
                           SystemName = role.SystemName,
                           Description = role.Description
                       };
        }

        public static Model.Role ToEntity(this RoleModel roleModel)
        {
            return new Model.Role
                       {
                           Id = roleModel.Id,
                           Name = roleModel.Name,
                           Active = roleModel.Active,
                           IsSystemRole = roleModel.IsSystemRole,
                           SystemName = roleModel.SystemName,
                           Description = roleModel.Description
                       };
        }
    }
}