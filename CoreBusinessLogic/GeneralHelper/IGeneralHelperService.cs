using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GeneralHelper
{
    public interface IGeneralHelperService
    {
        void LoadSessionData(CountryProgramme countryProg, Staff currentStaff);
    }
}
