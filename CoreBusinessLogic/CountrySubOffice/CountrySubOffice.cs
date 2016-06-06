using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._CountrySubOffice
{
    public class CountrySubOfficeService : ICountrySubOfficeService
    {
        private _Location.LocationService locationObj;
        private _CountryProgramme.CountryProgrammeService countryProgObj;

        public CountrySubOfficeService()
        {
            locationObj = new _Location.LocationService();
            countryProgObj = new _CountryProgramme.CountryProgrammeService();
        }

        public class CountrySubOfficeView
        {
            /// <summary>
            /// CountrySubOffice Entity
            /// </summary>
            public CountrySubOffice countrySubOffice;

            /// <summary>
            /// Location Entity
            /// </summary>
            public Location location;

            /// <summary>
            /// Country Entity based on location
            /// </summary>
            public Country country;

            /// <summary>
            /// CountryProgramme Entity
            /// </summary>
            public CountryProgramme countryProgramme;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CountrySubOffice GetCountrySubOffice(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.CountrySubOffices.Where(c => c.Id.Equals(id)).FirstOrDefault<CountrySubOffice>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<CountrySubOffice> GetCountrySubOffices(Guid countryProgId, string search = null)
        {
            List<CountrySubOffice> countrySubOfficeList = new List<CountrySubOffice>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    countrySubOfficeList = dbContext.CountrySubOffices.Where(c => c.CountryProgrammeId == countryProgId).OrderBy(c => c.Name).ToList<CountrySubOffice>();
                }
                else
                {
                    countrySubOfficeList = dbContext.CountrySubOffices
                        .Where(c => c.Address.Contains(search))
                        .OrderBy(c => c.Address).OrderBy(c => c.Name).ToList<CountrySubOffice>();
                }
            }
            return countrySubOfficeList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<CountrySubOfficeView> GetCountrySubOffices1(Guid countryProgId, string search = null)
        {
            List<CountrySubOfficeView> countrySubOfficeList = new List<CountrySubOfficeView>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    countrySubOfficeList = (from csuboffice in dbContext.CountrySubOffices
                                   join location in dbContext.Locations
                                   on csuboffice.LocationId equals location.Id into locationLeftJoin
                                   from location in locationLeftJoin.DefaultIfEmpty()
                                   join ctryProg in dbContext.CountryProgrammes
                                   on csuboffice.CountryProgrammeId equals ctryProg.Id
                                   where csuboffice.CountryProgrammeId == countryProgId
                                   orderby csuboffice.Name
                                    select new CountrySubOfficeView
                                   {
                                       countrySubOffice = csuboffice,
                                       location = location,
                                       country = location.Country,
                                       countryProgramme = ctryProg
                                   }).ToList();
                }
                else
                {
                    countrySubOfficeList = (from csuboffice in dbContext.CountrySubOffices
                                    join location in dbContext.Locations
                                    on csuboffice.LocationId equals location.Id into locationLeftJoin
                                            from location in locationLeftJoin.DefaultIfEmpty()
                                    join ctryProg in dbContext.CountryProgrammes
                                    on csuboffice.CountryProgrammeId equals ctryProg.Id
                                    where csuboffice.Address.Contains(search) && csuboffice.CountryProgrammeId == countryProgId
                                    orderby csuboffice.Name
                                    select new CountrySubOfficeView
                                    {
                                        countrySubOffice = csuboffice,
                                        location = location,
                                        country = location.Country,
                                        countryProgramme = ctryProg
                                    }).ToList();
                }
            }
            return countrySubOfficeList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctrySubOffice"></param>
        /// <returns></returns>
        public bool AddCountrySubOffice(CountrySubOffice ctrySubOffice)
        {
            bool isSaved = false;

            if (ctrySubOffice.Id.Equals(Guid.Empty))
            {
                ctrySubOffice.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.CountrySubOffices.Add(ctrySubOffice);
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
        /// <param name="CountrySubOffice"></param>
        /// <returns></returns>
        public bool EditCountrySubOffice(CountrySubOffice ctrySubOffice)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.CountrySubOffices.Attach(ctrySubOffice);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(ctrySubOffice, System.Data.EntityState.Modified);

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
        public bool DeleteCountrySubOffice(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                CountrySubOffice ctrySubOffice = dbContext.CountrySubOffices.Single(c => c.Id.Equals(id));
                dbContext.CountrySubOffices.Remove(ctrySubOffice);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                }
            }
            return isDeleted;
        }

        public _Location.LocationService LocationObj
        {
            get
            {
                return locationObj;
            }
        }

        public _CountryProgramme.CountryProgrammeService CountryProgObj
        {
            get
            {
                return countryProgObj;
            }
        }
    }
}
