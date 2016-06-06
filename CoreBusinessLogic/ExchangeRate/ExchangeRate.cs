using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._ExchangeRate
{
    public class ExchangeRateService : IExchangeRateService
    {
        private _Currency.CurrencyService currencyObj;

        public ExchangeRateService()
        {
            currencyObj = new _Currency.CurrencyService();
        }

        public class ExchangeRateView
        {
            public Guid Id;
            public double? Rate;
            public int Month;
            public int Year;
            public string MainCurrencyName;
            public string MainCurrencyShortName;
            public Guid MainCurrencyId;
            public string LocalCurrencyName;
            public string LocalCurrencyShortName;
            public Guid LocalCurrencyId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExchangeRate GetExchangeRate(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.ExchangeRates.Where(c => c.Id.Equals(id)).FirstOrDefault<ExchangeRate>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ExchangeRate> GetExchangeRates(Guid cpId, int search = 0)
        {
            List<ExchangeRate> exchangeRateList = new List<ExchangeRate>();
            using (var dbContext = new SCMSEntities())
            {
                if (search.Equals(0))
                {
                    exchangeRateList = dbContext.ExchangeRates.ToList<ExchangeRate>();
                }
                else
                {
                    exchangeRateList = dbContext.ExchangeRates
                        .Where(c => c.CountryProgrammeId == cpId && (c.Year.Equals(search) || c.Month.Equals(search)
                         || c.Rate.Equals(search)))
                        .OrderBy(c => c.Year).ToList<ExchangeRate>();
                }
            }
            return exchangeRateList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<ExchangeRateView> GetExchangeRates1(Guid cpId, int search = 0)
        {
            List<ExchangeRateView> exchangeRateList = new List<ExchangeRateView>();
            using (var dbContext = new SCMSEntities())
            {
                if (search.Equals(0))
                {
                    List<ExchangeRateView> exView1 = (from excRate in dbContext.ExchangeRates
                                                      join curr in dbContext.Currencies
                                                      on excRate.MainCurrencyId equals curr.Id
                                                      where excRate.CountryProgrammeId == cpId
                                                      select new ExchangeRateView
                                                     {
                                                         Id = excRate.Id,
                                                         Rate = excRate.Rate,
                                                         Month = excRate.Month,
                                                         Year = excRate.Year,
                                                         MainCurrencyName = curr.Name,
                                                         MainCurrencyShortName = curr.ShortName,
                                                         MainCurrencyId = curr.Id,
                                                         LocalCurrencyName = string.Empty,
                                                         LocalCurrencyShortName = string.Empty,
                                                         LocalCurrencyId = Guid.Empty
                                                     }).ToList();

                    List<ExchangeRateView> exView2 = (from excRate in dbContext.ExchangeRates
                                                      join curr in dbContext.Currencies
                                                      on excRate.LocalCurrencyId equals curr.Id
                                                      where excRate.CountryProgrammeId == cpId
                                                      select new ExchangeRateView
                                                      {
                                                          Id = excRate.Id,
                                                          Rate = excRate.Rate,
                                                          Month = excRate.Month,
                                                          Year = excRate.Year,
                                                          MainCurrencyName = string.Empty,
                                                          MainCurrencyShortName = string.Empty,
                                                          MainCurrencyId = Guid.Empty,
                                                          LocalCurrencyName = curr.Name,
                                                          LocalCurrencyShortName = curr.ShortName,
                                                          LocalCurrencyId = curr.Id
                                                      }).ToList();

                    exchangeRateList = (from ex1 in exView1
                                        from ex2 in exView2
                                        where ex1.Id == ex2.Id
                                        select new ExchangeRateView
                                   {
                                       Id = ex1.Id,
                                       Rate = ex1.Rate,
                                       Month = ex1.Month,
                                       Year = ex1.Year,
                                       MainCurrencyName = string.IsNullOrWhiteSpace(ex1.MainCurrencyName) ?
                                       ex2.MainCurrencyName : ex1.MainCurrencyName,
                                       MainCurrencyShortName = string.IsNullOrWhiteSpace(ex1.MainCurrencyShortName) ?
                                       ex2.MainCurrencyShortName : ex1.MainCurrencyShortName,
                                       MainCurrencyId = ex1.MainCurrencyId.Equals(Guid.Empty) ?
                                       ex2.MainCurrencyId : ex1.MainCurrencyId,
                                       LocalCurrencyName = string.IsNullOrWhiteSpace(ex1.LocalCurrencyName) ?
                                       ex2.LocalCurrencyName : ex1.LocalCurrencyName,
                                       LocalCurrencyShortName = string.IsNullOrWhiteSpace(ex1.LocalCurrencyShortName) ?
                                       ex2.LocalCurrencyShortName : ex1.LocalCurrencyShortName,
                                       LocalCurrencyId = ex1.LocalCurrencyId.Equals(Guid.Empty) ?
                                       ex2.LocalCurrencyId : ex1.LocalCurrencyId
                                   }).OrderByDescending(e => e.Year).ThenByDescending(e => e.Month).ThenBy(e => e.MainCurrencyShortName).ThenBy(e => e.LocalCurrencyShortName).ToList();

                    return exchangeRateList;
                }
            }
            return exchangeRateList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ExchangeRate"></param>
        /// <returns></returns>
        public bool AddExchangeRate(ExchangeRate exchangeRate)
        {
            bool isSaved = false;

            if (exchangeRate.Id.Equals(Guid.Empty))
            {
                exchangeRate.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.ExchangeRates.Add(exchangeRate);
                if (dbContext.SaveChanges() > 0)
                {
                    isSaved = true;
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchangeRate"></param>
        /// <returns></returns>
        public bool EditExchangeRate(ExchangeRate exchangeRate)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.ExchangeRates.Attach(exchangeRate);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(exchangeRate, System.Data.EntityState.Modified);

                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteExchangeRate(Guid id)
        {
            bool isDeleted = false;
            using (var context = new SCMSEntities())
            {
                ExchangeRate exchangeRate = context.ExchangeRates.Single(c => c.Id.Equals(id));
                context.ExchangeRates.Remove(exchangeRate);
                if (context.SaveChanges() > 0)
                {
                    isDeleted = true;
                }
            }
            return isDeleted;
        }

        public _Currency.CurrencyService CurrencyObj
        {
            get
            {
                return currencyObj;
            }
        }

        /// <summary>
        /// Added By Frank. Calculate currency value in foreign currency
        /// </summary>
        /// <param name="fxCurrency">Foreign currency</param>
        /// <param name="localCurrency">Local currency</param>
        /// <param name="amount">Amount in local currency</param>
        /// <returns>The converted value</returns>
        public decimal? GetForeignCurrencyValue(Currency fxCurrency, Currency localCurrency, decimal? amount, Guid cpId)
        {
            try
            {
                if (fxCurrency.Id == localCurrency.Id)
                    return amount;

                using (var context = new SCMSEntities())
                {
                    var exRate = context.ExchangeRates.OrderByDescending(e => e.Year).OrderByDescending(e => e.Month).FirstOrDefault(e => e.MainCurrencyId == fxCurrency.Id && e.LocalCurrencyId == localCurrency.Id && e.CountryProgrammeId == cpId);
                    if (exRate != null)
                    {
                        return amount / (decimal?)exRate.Rate;
                    }
                    exRate = context.ExchangeRates.OrderByDescending(e => e.Year).OrderByDescending(e => e.Month).FirstOrDefault(e => e.LocalCurrencyId == fxCurrency.Id && e.MainCurrencyId == localCurrency.Id && e.CountryProgrammeId == cpId);
                    if (exRate != null)
                    {
                        return amount * (decimal?)exRate.Rate;
                    }
                    var exRate1 = (from e1 in context.ExchangeRates
                                   join e2 in context.ExchangeRates
                                   on e1.MainCurrencyId equals e2.MainCurrencyId
                                   where e1.LocalCurrencyId == localCurrency.Id && e2.LocalCurrencyId == fxCurrency.Id && e1.CountryProgrammeId == cpId && e2.CountryProgrammeId == cpId
                                   orderby e1.Year descending, e1.Month descending
                                   select e1).ToList<ExchangeRate>();
                    if (exRate1 != null && exRate1.Count > 0)
                    {
                        decimal? tmpAmt = amount / (decimal?)exRate1[0].Rate;
                        foreach (var ex in exRate1)
                        {
                            var exRate2 = context.ExchangeRates.OrderByDescending(e => e.Year).OrderByDescending(e => e.Month).FirstOrDefault(e => e.MainCurrencyId == ex.MainCurrencyId && e.LocalCurrencyId == fxCurrency.Id && e.CountryProgrammeId == cpId);
                            if (exRate2 != null)
                                return tmpAmt * (decimal?)exRate2.Rate;
                        }
                    }                    
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public decimal GetForeignCurrencyValue(Guid fxCurrencyId, Guid localCurrencyId, decimal amount, Guid cpId)
        {
            try
            {
                using (var context = new SCMSEntities())
                {
                    var fxCurrency = context.Currencies.FirstOrDefault(c => c.Id == fxCurrencyId);
                    var localCurrency = context.Currencies.FirstOrDefault(c => c.Id == localCurrencyId);
                    return (decimal)this.GetForeignCurrencyValue(fxCurrency, localCurrency, amount, cpId);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
