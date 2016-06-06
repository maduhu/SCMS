using FluentValidation;
using SCMS.UI.Areas.Admin.Models.Role;

namespace SCMS.UI.Areas.Admin.Validators.Role
{
    public class RoleValidator : AbstractValidator<RoleModel>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Name).Length(2, 255);
            RuleFor(x => x.SystemName).Length(2, 255);
        }
    }
}