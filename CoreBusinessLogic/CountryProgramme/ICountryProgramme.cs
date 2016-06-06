using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._CountryProgramme
{
    public interface ICountryProgrammeService
    {
        bool AddCountryProgramme(CountryProgramme countryProg);
        bool EditCountryProgramme(CountryProgramme countryProg);
        bool DeleteCountryProgramme(Guid id);
        CountryProgramme GetCountryProgrammeById(Guid id);
        CountryProgramme GetCountryProgrammeForDisplay(Guid id);
        List<CountryProgramme> GetCountryProgrammes(string search = null, Guid? location = null, Guid? countryId = null);
        List<CountryProgrammeService.CountryProgView> GetCountryProgrammes1(string search = null);
        _Country.CountryService CountryObj { get; }
        _Currency.CurrencyService CurrencyObj { get; }
    }
}
