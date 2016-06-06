using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._CountrySubOffice
{
    public interface ICountrySubOfficeService
    {
        bool AddCountrySubOffice(CountrySubOffice countrySubOffice);
        bool EditCountrySubOffice(CountrySubOffice countrySubOffice);
        bool DeleteCountrySubOffice(Guid id);
        CountrySubOffice GetCountrySubOffice(Guid id);
        List<CountrySubOffice> GetCountrySubOffices(Guid countryProgId, string search = null);
        List<CountrySubOfficeService.CountrySubOfficeView> GetCountrySubOffices1(Guid countryProgId, string search = null);
        _Location.LocationService LocationObj { get; }
        _CountryProgramme.CountryProgrammeService CountryProgObj { get; }
    }
}
