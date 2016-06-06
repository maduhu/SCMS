using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using FluentValidation.Attributes;
using SCMS.CoreBusinessLogic.Web;
using SCMS.UI.Areas.Admin.Validators.Validators;

namespace SCMS.UI.Areas.Admin.Models.Settings
{
    
    public class SettingsModel : BaseModel
    {
        public SettingsModel()
        {
            CommonSettings=new CommonSettingsModel();
        }

        public CommonSettingsModel CommonSettings { get; set; }
    }

    [Validator(typeof(CommonSettingsModelValidator))]
    public class CommonSettingsModel
    {
        public int GridPageSize { get; set; }

        public int MaxLoginRetries { get; set; }
        
        public string AdminEmail { get; set; }
        
        public bool HideMenusBasedOnPermissions { get; set; }

        public int PersonPhotoMaxSize { get; set; }

        public int SignaturePhotoMaxSize { get; set; }

        public string PersonPhotoUploadPath { get; set; }

        public List<string> AllowedImageExtensions { get; set; }

        public String ImageTypes { get; set; }

        public void SetImageTypesFromModel()
        {
            AllowedImageExtensions = new List<string>();
            if (ImageTypes.IsNotNullOrWhiteSpace())
            {
                ImageTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .ForEach(p => AllowedImageExtensions.Add(p));
            }
        }


        public void SetImageTypesFromSettings()
        {
            if (AllowedImageExtensions.IsNotNullOrEmpty())
                ImageTypes = AllowedImageExtensions.StringJoin("", ",");
        }
    }
}