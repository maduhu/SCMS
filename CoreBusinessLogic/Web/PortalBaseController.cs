using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Model;
using SCMS.CoreBusinessLogic.ActionFilters;
using System.Data.Objects;
using System.Data;

namespace SCMS.CoreBusinessLogic.Web
{
    [MyException]
    [SignedInAuthorize]
    public class PortalBaseController : BaseController
    {
        protected readonly IPermissionService permissionService;
        protected readonly IUserContext userContext;
        protected readonly CountryProgramme countryProg;
        protected readonly Currency mbCurrency;
        protected readonly SystemUser currentUser;
        protected readonly Staff currentStaff;

        public PortalBaseController(IUserContext userContext, IPermissionService permissionService)
        {
            this.userContext = userContext;
            this.permissionService = permissionService;
            if (userContext.CurrentUser != null)
            {
                countryProg = userContext.CurrentUser.Staff.CountrySubOffice.CountryProgramme;
                currentUser = userContext.CurrentUser;
                mbCurrency = countryProg.Currency;
                currentStaff = currentUser.Staff;
                ViewBag.LoggedInUser = currentStaff.Person.FirstName + " " + currentStaff.Person.OtherNames;
                ViewBag.CountryProgramme = countryProg.ProgrammeName;
                ViewBag.CountryProg = countryProg.ShortName;
            }
        }
    }
}
