using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;
using SCMS.CoreBusinessLogic._Country;

namespace SCMS.CoreBusinessLogic._Supplier
{
    public interface ISupplierService
    {
        bool AddSupplier(Supplier supplier);
        bool EditSupplier(Supplier supplier);
        bool DeleteSupplier(Guid id);
        Supplier GetSupplier(Guid id);
        List<Supplier> GetSuppliers(Guid cpId, string search = null);
        List<SupplierService.SupplierServiceView> GetSuppliers1(Guid cpId, string search = null);
        ICountryService CountryService { get; }
        Supplier GetSupplierByName(string name, Guid countryProgId);
    }
}
