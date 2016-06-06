using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Country
{
    public class CountryService : ICountryService
    {
        private _Currency.CurrencyService currencyObj;

        public CountryService()
        {
            currencyObj = new _Currency.CurrencyService();
        }

        public class CountryView
        {
            public Guid Id;
            public string Name;
            public string ShortName;
            public Guid? CurrencyId;
            public string Currency;
            public string CurrencyShortName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Country GetCountry(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.Countries.Where(c => c.Id.Equals(id)).FirstOrDefault<Country>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Country> GetCountries(string search = null)
        {
            List<Country> countryList = new List<Country>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    countryList = dbContext.Countries.OrderBy(c => c.Name).ToList<Country>();
                }
                else
                {
                    countryList = dbContext.Countries
                        .Where(c => c.Name.Contains(search) || c.ShortName.Contains(search))
                        .OrderBy(c => c.Name).ToList<Country>();
                }
            }
            return countryList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<CountryView> GetCountries1(string search = null)
        {
            List<CountryView> countryList = new List<CountryView>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    countryList = (from country in dbContext.Countries
                                   join currency in dbContext.Currencies
                                   on country.CurrencyId equals currency.Id
                                   into countryView
                                   from ctryView in countryView.DefaultIfEmpty()
                                   select new CountryView
                                   {
                                       Id = country.Id,
                                       Name = country.Name,
                                       ShortName = country.ShortName,
                                       CurrencyId = country.CurrencyId,
                                       Currency = ctryView.Name,
                                       CurrencyShortName = ctryView.ShortName
                                   }).OrderBy(c => c.Name).ToList<CountryView>();
                }
                else
                {
                    countryList = (from country in dbContext.Countries
                                   join currency in dbContext.Currencies
                                   on country.CurrencyId equals currency.Id
                                   into countryView
                                   from ctryView in countryView.DefaultIfEmpty()
                                   where country.Name.Contains(search) || country.ShortName.Contains(search)
                                   select new CountryView
                                   {
                                       Id = country.Id,
                                       Name = country.Name,
                                       ShortName = country.ShortName,
                                       CurrencyId = country.CurrencyId,
                                       Currency = ctryView.Name,
                                       CurrencyShortName = ctryView.ShortName
                                   }).OrderBy(c => c.Name).ToList<CountryView>();
                }
            }
            return countryList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public bool AddCountry(Country country)
        {
            bool isSaved = false;

            if (country.Id.Equals(Guid.Empty))
            {
                country.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.Countries.Add(country);
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
        /// <param name="country"></param>
        /// <returns></returns>
        public bool EditCountry(Country country)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.Countries.Attach(country);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(country, System.Data.EntityState.Modified);

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
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool DeleteCountry(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                Country country = dbContext.Countries.Single(c => c.Id.Equals(id));
                dbContext.Countries.Remove(country);
                if (dbContext.SaveChanges() > 0)
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
    }
}
