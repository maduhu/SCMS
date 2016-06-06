using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.CoreBusinessLogic._Country;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic._Supplier
{
    public class SupplierService : ISupplierService
    {
        private ICountryService countryService;

        public SupplierService(ICountryService countryService)
        {
            this.countryService = countryService;
        }

        public class SupplierServiceView
        {
            /// <summary>
            /// Supplier entity 
            /// </summary>
            public Supplier Supplier;

            /// <summary>
            /// Country name
            /// </summary>
            public string Country;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Supplier GetSupplier(Guid id)
        {
            using (var dbContext = new SCMSEntities())
            {
                return dbContext.Suppliers.Where(c => c.Id.Equals(id)).FirstOrDefault<Supplier>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<Supplier> GetSuppliers(Guid cpId, string search = null)
        {
            List<Supplier> supplierList = new List<Supplier>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    supplierList = dbContext.Suppliers.Where(s => s.CountryProgrammeId == cpId).OrderBy(s => s.Name).ToList<Supplier>();
                }
                else
                {
                    supplierList = dbContext.Suppliers
                        .Where(c => c.Name.Contains(search))
                        .OrderBy(c => c.Name).ToList<Supplier>();
                }
            }
            return supplierList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<SupplierServiceView> GetSuppliers1(Guid cpId, string search = null)
        {
            List<SupplierServiceView> locationList = new List<SupplierServiceView>();
            using (var dbContext = new SCMSEntities())
            {
                if (string.IsNullOrEmpty(search))
                {
                    locationList = (from supplier in dbContext.Suppliers
                                    join country in dbContext.Countries
                                    on supplier.CountryId equals country.Id
                                    where supplier.CountryProgrammeId == cpId
                                    select new SupplierServiceView
                                   {
                                       Supplier = supplier,
                                       Country = country.Name
                                   }).OrderBy(s => s.Supplier.Name).ToList();
                }
                else
                {
                    locationList = (from supplier in dbContext.Suppliers
                                    join country in dbContext.Countries
                                   on supplier.CountryId equals country.Id
                                    where supplier.Name.Contains(search)
                                    && supplier.CountryProgrammeId == cpId
                                    select new SupplierServiceView
                                   {
                                       Supplier = supplier,
                                       Country = country.Name
                                   }).OrderBy(s => s.Supplier.Name).ToList();
                }
            }
            return locationList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public bool AddSupplier(Supplier supplier)
        {
            bool isSaved = false;

            if (supplier.Id.Equals(Guid.Empty))
            {
                supplier.Id = Guid.NewGuid();
            }

            using (var dbContext = new SCMSEntities())
            {
                dbContext.Suppliers.Add(supplier);
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
        /// <param name="supplier"></param>
        /// <returns></returns>
        public bool EditSupplier(Supplier supplier)
        {
            bool isSaved = false;
            using (var context = new SCMSEntities())
            {
                context.Suppliers.Attach(supplier);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(supplier, System.Data.EntityState.Modified);

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
        public bool DeleteSupplier(Guid id)
        {
            bool isDeleted = false;
            using (var dbContext = new SCMSEntities())
            {
                Supplier supplier = dbContext.Suppliers.Single(c => c.Id.Equals(id));
                dbContext.Suppliers.Remove(supplier);
                if (dbContext.SaveChanges() > 0)
                {
                    isDeleted = true;
                }
            }
            return isDeleted;
        }

        public ICountryService CountryService
        {
            get
            {
                return countryService;
            }
        }

        public Supplier GetSupplierByName(string name, Guid countryProgId)
        {
            using (var context = new SCMSEntities())
            {
                return context.Suppliers.FirstOrDefault(s => s.CountryProgrammeId == countryProgId && s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));                
            }
        }
    }
}
