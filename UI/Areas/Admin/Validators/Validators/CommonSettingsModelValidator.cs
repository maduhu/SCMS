using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using SCMS.UI.Areas.Admin.Models.Settings;

namespace SCMS.UI.Areas.Admin.Validators.Validators
{
    public class CommonSettingsModelValidator : AbstractValidator<CommonSettingsModel>
    {
        public CommonSettingsModelValidator()
        {
            RuleFor(x => x.AdminEmail).NotEmpty();
            RuleFor(x => x.AdminEmail).EmailAddress();
            RuleFor(x => x.MaxLoginRetries).GreaterThan(2);
            RuleFor(x => x.SignaturePhotoMaxSize).GreaterThan(0);
            RuleFor(x => x.PersonPhotoMaxSize).GreaterThan(0);
            RuleFor(x => x.PersonPhotoUploadPath).NotEmpty();
            RuleFor(x => x.ImageTypes).NotEmpty();
        }
    }
}