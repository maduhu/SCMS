using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Currency
{
    public class CurrencyService : ICurrencyService
    {
        private static void ClearCurrencySessionData()
        {
            SessionData.CurrentSession.CurrencyList = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Currency GetCurrency(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return SessionData.CurrentSession.CurrencyList.FirstOrDefault(c => c.Id.Equals(id));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Currency> GetCurrencies(Guid countryProgId, string search = null)
        {
            List<Currency> currencyList = new List<Currency>();
            if (string.IsNullOrEmpty(search))
            {
                return SessionData.CurrentSession.CurrencyList.OrderBy(c => c.ShortName).ToList();
            }
            else
            {
                return SessionData.CurrentSession.CurrencyList
                    .Where(c => (c.Name.Contains(search) || c.ShortName.Contains(search) || c.Symbol.Contains(search)))
                    .OrderBy(c => c.Name).ToList<Currency>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool AddCurrency(Currency currency)
        {
            bool isSaved = false;

            if (currency.Id.Equals(Guid.Empty))
            {
                currency.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.Currencies.Add(currency);
                if(dbContext.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearCurrencySessionData();
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool EditCurrency(Currency currency)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.Currencies.Attach(currency);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(currency, System.Data.EntityState.Modified);
                
                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearCurrencySessionData();
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public bool DeleteCurrency(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                Currency currency = new Currency { Id = id };
                dbContext.Currencies.Attach(currency);
                dbContext.Currencies.Remove(currency);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                    ClearCurrencySessionData();
                }
            }
            return isDeleted;
        }
    }
}
