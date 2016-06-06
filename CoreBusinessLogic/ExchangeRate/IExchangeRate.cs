using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._ExchangeRate
{
    public interface IExchangeRateService
    {
        bool AddExchangeRate(ExchangeRate exchangeRate);
        bool EditExchangeRate(ExchangeRate exchangeRate);
        bool DeleteExchangeRate(Guid id);
        ExchangeRate GetExchangeRate(Guid id);
        List<ExchangeRate> GetExchangeRates(Guid cpId, int search = 0);
        List<ExchangeRateService.ExchangeRateView> GetExchangeRates1(Guid cpId, int search = 0);
        _Currency.CurrencyService CurrencyObj { get; }
        decimal? GetForeignCurrencyValue(Currency fxCurrency, Currency localCurrency, decimal? amount, Guid cpId);
        decimal GetForeignCurrencyValue(Guid fxCurrencyId, Guid localCurrencyId, decimal amount, Guid cpId);
    }
}
