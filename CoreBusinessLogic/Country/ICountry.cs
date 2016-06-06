using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Country
{
    public interface ICountryService
    {
        bool AddCountry(Country country);
        bool EditCountry(Country country);
        bool DeleteCountry(Guid id);
        Country GetCountry(Guid id);
        List<Country> GetCountries(string search = null);
        List<CountryService.CountryView> GetCountries1(string search = null);
        _Currency.CurrencyService CurrencyObj { get; }
    }
}
