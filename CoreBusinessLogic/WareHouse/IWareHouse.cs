using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._WareHouse
{
    public interface IWareHouseService
    {
        bool AddWareHouse(WareHouse wareHouse);
        bool EditWareHouse(WareHouse wareHouse);
        bool DeleteWareHouse(Guid id);
        WareHouse GetWareHouse(Guid id);
        List<WareHouse> GetWareHouses(Guid countryProgId, string search = null);
        List<WareHouseService.WareHouseView> GetWareHouses1(string search = null);
        _Location.LocationService LocationObj { get; }
        _CountrySubOffice.CountrySubOfficeService CtrySubOffObj { get; }
    }
}
