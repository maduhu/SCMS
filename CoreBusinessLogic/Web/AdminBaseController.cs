using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.Security;
using SCMS.Model;
using System.Data.Objects;
using System.Data;

namespace SCMS.CoreBusinessLogic.Web
{
    //[AdminAuthorize]
    public class AdminBaseController : BaseController
    {
        protected readonly IUserContext userContext;
        protected readonly CountryProgramme countryProg;
        protected readonly Currency mbCurrency;
        protected readonly SystemUser currentUser;
        protected readonly Staff currentStaff;

        public AdminBaseController(IUserContext userContext)
        {
            this.userContext = userContext;
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
