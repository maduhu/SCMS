using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Currency
{
    public interface ICurrencyService
    {
        bool AddCurrency(Currency currency);
        bool EditCurrency(Currency currency);
        bool DeleteCurrency(Guid id);
        Currency GetCurrency(Guid id);
        List<Currency> GetCurrencies(Guid countryProgId, string search = null);
    }
}
