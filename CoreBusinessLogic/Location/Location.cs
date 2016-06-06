using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Location
{
    public class LocationService : ILocationService
    {
        private _Country.CountryService countryObj;

        public LocationService()
        {
            countryObj = new _Country.CountryService();
        }

        public class LocationView
        {
            public Guid Id;
            public string Name;
            public string ShortName;
            public string Description;
            public Guid CountryId;
            public string Country;
            public string CountryShortName;
        }

        private static void ClearLocationSessionData()
        {
            SessionData.CurrentSession.LocationList = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Location GetLocation(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.Locations.Where(c => c.Id.Equals(id)).FirstOrDefault<Location>();
            }
        }

        public List<Location> GetLocations(Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                return context.Locations.Where(l => l.CountryProgrammeId == countryProgId).OrderBy(l => l.Name).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public List<Location> GetLocations(string search = null, Guid? countryId = null)
        {
            //List<Location> locationList = new List<Location>();
            using (var dbContext = new SCMSEntities())
            {
                var locationList = from location in dbContext.Locations
                                   select location;

                if (!string.IsNullOrEmpty(search))
                {
                    locationList = from location in locationList
                                   where location.Name.Contains(search) || location.ShortName.Contains(search)
                                   orderby location.Name
                                   select location;
                }

                if (countryId != null)
                {
                    locationList = from location in locationList
                                   join country in dbContext.Countries
                                   on location.CountryId equals country.Id
                                   where location.CountryId == countryId
                                   orderby location.Name
                                   select location;
                }
                return locationList.ToList<Location>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<LocationView> GetLocations1(Guid countryProgId, string search = null)
        {
            List<LocationView> locationList = new List<LocationView>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    locationList = (from location in dbContext.Locations
                                    join country in dbContext.Countries
                                    on location.CountryId equals country.Id
                                    into locationView
                                    from lView in locationView.DefaultIfEmpty()
                                    where location.CountryProgrammeId == countryProgId
                                    select new LocationView
                                   {
                                       Id = location.Id,
                                       Name = location.Name,
                                       ShortName = location.ShortName,
                                       Description = location.Description,
                                       CountryId = location.CountryId,
                                       Country = lView.Name,
                                       CountryShortName = lView.ShortName
                                   }).OrderBy(l => l.Name).ToList<LocationView>();
                }
                else
                {
                    locationList = (from location in dbContext.Locations
                                    join country in dbContext.Countries
                                   on location.CountryId equals country.Id
                                   into locationView
                                    from lView in locationView.DefaultIfEmpty()
                                    where (location.Name.Contains(search) || location.ShortName.Contains(search))
                                    && location.CountryProgrammeId == countryProgId
                                    select new LocationView
                                   {
                                       Id = location.Id,
                                       Name = location.Name,
                                       ShortName = location.ShortName,
                                       Description = location.Description,
                                       CountryId = location.CountryId,
                                       Country = lView.Name,
                                       CountryShortName = lView.ShortName
                                   }).OrderBy(l => l.Name).ToList<LocationView>();
                }
            }
            return locationList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool AddLocation(Location location)
        {
            bool isSaved = false;

            if (location.Id.Equals(Guid.Empty))
            {
                location.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.Locations.Add(location);
                if (dbContext.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearLocationSessionData();
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool EditLocation(Location location)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.Locations.Attach(location);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(location, System.Data.EntityState.Modified);

                if (context.SaveChanges() > 0)
                {
                    isSaved = true;
                    ClearLocationSessionData();
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteLocation(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                Location location = dbContext.Locations.Single(c => c.Id.Equals(id));
                dbContext.Locations.Remove(location);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                    ClearLocationSessionData();
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

        public Location GetLocationByName(string name, Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                return context.Locations.FirstOrDefault(l => l.CountryProgrammeId == countryProgId && l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
