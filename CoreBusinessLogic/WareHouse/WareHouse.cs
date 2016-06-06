using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.GeneralHelper;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._WareHouse
{
    public class WareHouseService : IWareHouseService
    {
        private _Location.LocationService locationObj;
        private _CountrySubOffice.CountrySubOfficeService ctrySubOffObj;

        public WareHouseService()
        {
            locationObj = new _Location.LocationService();
            ctrySubOffObj = new _CountrySubOffice.CountrySubOfficeService();
        }

        public class WareHouseView
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
            /// WareHouse Entity
            /// </summary>
            public WareHouse wareHouse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WareHouse GetWareHouse(Guid id)
        {
            return SessionData.CurrentSession.WarehouseList.Where(c => c.Id.Equals(id)).FirstOrDefault<WareHouse>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<WareHouse> GetWareHouses(Guid countryProgId, string search = null)
        {
            if (string.IsNullOrEmpty(search))
                return SessionData.CurrentSession.WarehouseList.OrderBy(c => c.Name).ToList<WareHouse>();
            else
                return SessionData.CurrentSession.WarehouseList.Where(w => w.Name.Contains(search)).OrderBy(c => c.Name).ToList<WareHouse>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<WareHouseView> GetWareHouses1(string search = null)
        {
            var wareHouseList = SessionData.CurrentSession.WarehouseList.OrderBy(c => c.Name).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                wareHouseList = wareHouseList.Where(w => w.Name.Contains(search));
            }

            return (from whse in wareHouseList
                    select new WareHouseView
                    {
                        countrySubOffice = whse.CountrySubOffice,
                        location = whse.Location,
                        country = whse.Location.Country,
                        wareHouse = whse
                    }
                    ).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wareHouse"></param>
        /// <returns></returns>
        public bool AddWareHouse(WareHouse wareHouse)
        {
            bool isSaved = false;

            if (wareHouse.Id.Equals(Guid.Empty))
            {
                wareHouse.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.WareHouses.Add(wareHouse);
                if (dbContext.SaveChanges() > 0)
                {
                    SessionData.CurrentSession.WarehouseList = null;
                    isSaved = true;
                }
            }
            return isSaved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wareHouse"></param>
        /// <returns></returns>
        public bool EditWareHouse(WareHouse wareHouse)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.WareHouses.Attach(wareHouse);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(wareHouse, System.Data.EntityState.Modified);

                if (context.SaveChanges() > 0)
                {
                    SessionData.CurrentSession.WarehouseList = null;
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
        public bool DeleteWareHouse(Guid id)
        {
            bool isDeleted = false;
            using (var context = new SCMSEntities())
            {
                WareHouse wareHouse = context.WareHouses.Single(c => c.Id.Equals(id));
                context.WareHouses.Remove(wareHouse);
                if (context.SaveChanges() > 0)
                {
                    SessionData.CurrentSession.WarehouseList = null;
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

        public _CountrySubOffice.CountrySubOfficeService CtrySubOffObj
        {
            get
            {
                return ctrySubOffObj;
            }
        }
    }
}
