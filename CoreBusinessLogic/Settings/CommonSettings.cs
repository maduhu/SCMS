using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.Settings
{
    public class CommonSettings : ISystemSettings
    {
        public CommonSettings()
        {
            AllowedImageExtensions=new List<string>();
            GridPageSize = 30;
        }

        public int MaxLoginRetries { get; set; }
        public string AdminEmail { get; set; }
        public bool HideMenusBasedOnPermissions { get; set; }
        public int PersonPhotoMaxSize { get; set; }
        public int SignaturePhotoMaxSize { get; set; }
        public string PersonPhotoUploadPath { get; set; }
        public List<string> AllowedImageExtensions { get; set; }
        public int GridPageSize { get; set; }
    }
}
