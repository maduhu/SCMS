using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using SCMS.UI.Areas.Admin.Models.SystemUser;

namespace SCMS.UI.Areas.Admin.Validators.SystemUser
{
    public class SystemUserModelValidator : AbstractValidator<SystemUserModel>
    {
        public SystemUserModelValidator()
        {
            RuleFor(x => x.FirstName).Length(1, 100);
            RuleFor(x => x.OtherNames).Length(1, 250);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().When(p => p.Id == Guid.Empty);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(
                "Password and Confirm Password must be equal")
                .When(x => x.Password.IsNotNullOrWhiteSpace());           

            RuleFor(x => x.IdNumber).Length(0, 50);
            RuleFor(x => x.OfficialPhone).Length(0, 60);
            RuleFor(x => x.SelectedDesignationId).NotEmpty();
            RuleFor(x => x.SelectedCountrySubOfficeId).NotNull();
        }
    }
}