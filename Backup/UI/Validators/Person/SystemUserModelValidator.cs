using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using SCMS.UI.Models.Person;

namespace SCMS.UI.Validators.Person
{
    public class SystemUserModelValidator : AbstractValidator<SystemUserModel>
    {
        public SystemUserModelValidator()
        {
            RuleFor(x => x.FirstName).Length(1, 100);
            RuleFor(x => x.OtherNames).Length(1, 250);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.OldPassword).NotEmpty().When(p => p.Password.IsNotNullOrWhiteSpace()).WithMessage("Old Password is required.");
            RuleFor(x => x.Password).NotEmpty().When(p => p.ConfirmPassword.IsNotNullOrWhiteSpace());
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(
                "Password and Confirm Password must be equal")
                .When(x => x.Password.IsNotNullOrWhiteSpace());

            RuleFor(x => x.IdNumber).Length(0, 50);
            RuleFor(x => x.OfficialPhone).Length(0, 60);
        }
    }
}