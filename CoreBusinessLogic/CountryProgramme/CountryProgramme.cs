using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._CountryProgramme
{
    public class CountryProgrammeService : ICountryProgrammeService
    {
        private _Country.CountryService countryObj;
        private _Currency.CurrencyService currencyObj;

        public CountryProgrammeService()
        {
            countryObj = new _Country.CountryService();
            currencyObj = new _Currency.CurrencyService();
        }

        public class CountryProgView
        {
            public Guid Id;
            public string ProgrammeName;
            public string Address;
            public string PrimaryPhone;
            public string SecondaryPhone;
            public string Fax;
            public string Email;
            public string WebAddress;
            public Guid CountryId;
            public string Country;
            public string CountryShortName;
            public Currency currency;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CountryProgramme GetCountryProgrammeById(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.CountryProgrammes.Where(c => c.Id.Equals(id)).FirstOrDefault<CountryProgramme>();
            }
        }

        public CountryProgramme GetCountryProgrammeForDisplay(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                var cp = dbContext.CountryProgrammes.Where(c => c.Id.Equals(id)).FirstOrDefault<CountryProgramme>();
                if (cp != null)
                {
                    var curr = cp.Currency;
                    var country = cp.Country;
                    cp.FirstDesignation = dbContext.Designations.FirstOrDefault(c => c.CountryProgrammeId == cp.Id);
                    cp.FirstLocation = dbContext.Locations.FirstOrDefault(l => l.CountryProgrammeId == cp.Id);
                    cp.FirstSubOffice = dbContext.CountrySubOffices.FirstOrDefault(c => c.CountryProgrammeId == cp.Id);
                }
                return cp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="location"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public List<CountryProgramme> GetCountryProgrammes(string search = null, Guid? location = null, Guid? countryId = null)
        {
            using (var dbContext = new SCMSEntities())
            {
                var countryProgList = from ctryProg in dbContext.CountryProgrammes
                                      select ctryProg;

                if (!string.IsNullOrEmpty(search))
                {
                    countryProgList = from ctryProg in countryProgList
                                      where ctryProg.ProgrammeName.Contains(search) || ctryProg.Address.Contains(search)
                                        || ctryProg.Email.Contains(search) || ctryProg.Fax.Contains(search) 
                                        || ctryProg.WebAddress.Contains(search)
                                      orderby ctryProg.ProgrammeName
                                      select ctryProg;
                }
                if (location != null)
                {
                    countryProgList = from ctryProg in countryProgList
                                      join loca in dbContext.Locations
                                      on ctryProg.CountryId equals loca.CountryId
                                      where loca.Id == location
                                      orderby ctryProg.ProgrammeName
                                      select ctryProg;
                }

                if (countryId != null)
                {
                    countryProgList = from ctryProg in countryProgList
                                      join ctry in dbContext.Countries
                                      on ctryProg.CountryId equals ctry.Id
                                      where ctry.Id == countryId
                                      orderby ctryProg.ProgrammeName
                                      select ctryProg;
                }
                return countryProgList.ToList<CountryProgramme>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<CountryProgView> GetCountryProgrammes1(string search = null)
        {
            List<CountryProgView> countryProgViewList = new List<CountryProgView>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    countryProgViewList = (from countryProg in dbContext.CountryProgrammes
                                   join country in dbContext.Countries
                                   on countryProg.CountryId equals country.Id
                                   into countryProgView
                                    from cpView in countryProgView.DefaultIfEmpty()
                                    select new CountryProgView
                                   {
                                       Id = countryProg.Id,
                                       ProgrammeName = countryProg.ProgrammeName,
                                       Address = countryProg.Address,
                                       PrimaryPhone = countryProg.PrimaryPhone,
                                       SecondaryPhone = countryProg.SecondaryPhone,
                                       Fax = countryProg.Fax,
                                       Email = countryProg.Email,
                                       WebAddress = countryProg.WebAddress,
                                       CountryId = countryProg.CountryId,
                                       Country = cpView.Name,
                                       CountryShortName = cpView.ShortName,
                                       currency = countryProg.Currency
                                   }).ToList<CountryProgView>();
                }
                else
                {
                    countryProgViewList = (from countryProg in dbContext.CountryProgrammes
                                    join country in dbContext.Countries
                                   on countryProg.CountryId equals country.Id
                                   into countryProgView
                                    from cpView in countryProgView.DefaultIfEmpty()
                                    where countryProg.ProgrammeName.Contains(search) || countryProg.Address.Contains(search)
                                                || countryProg.Email.Contains(search) || countryProg.Fax.Contains(search)
                                                || countryProg.WebAddress.Contains(search)
                                    select new CountryProgView
                                   {
                                       Id = countryProg.Id,
                                       ProgrammeName = countryProg.ProgrammeName,
                                       Address = countryProg.Address,
                                       PrimaryPhone = countryProg.PrimaryPhone,
                                       SecondaryPhone = countryProg.SecondaryPhone,
                                       Fax = countryProg.Fax,
                                       Email = countryProg.Email,
                                       WebAddress = countryProg.WebAddress,
                                       CountryId = countryProg.CountryId,
                                       Country = cpView.Name,
                                       CountryShortName = cpView.ShortName,
                                       currency = countryProg.Currency
                                   }).ToList<CountryProgView>();
                }
            }
            return countryProgViewList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countryProg"></param>
        /// <returns></returns>
        public bool AddCountryProgramme(CountryProgramme countryProg)
        {
            bool isSaved = false;

            if (countryProg.Id.Equals(Guid.Empty))
            {
                countryProg.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.CountryProgrammes.Add(countryProg);
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
        /// <param name="countryProg"></param>
        /// <returns></returns>
        public bool EditCountryProgramme(CountryProgramme countryProg)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.CountryProgrammes.Attach(countryProg);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(countryProg, System.Data.EntityState.Modified);

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
        public bool DeleteCountryProgramme(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                CountryProgramme countryProg = dbContext.CountryProgrammes.Single(c => c.Id.Equals(id));
                dbContext.CountryProgrammes.Remove(countryProg);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                }
            }
            return isDeleted;
        }

        public _Country.CountryService CountryObj
        {
            get
            {
                return countryObj;
            }
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
