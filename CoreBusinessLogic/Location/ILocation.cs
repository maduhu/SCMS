using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Location
{
    public interface ILocationService
    {
        bool AddLocation(Location location);
        bool EditLocation(Location location);
        bool DeleteLocation(Guid id);
        Location GetLocation(Guid id);
        Location GetLocationByName(string name, Guid countryProgId);
        List<Location> GetLocations(string search = null, Guid? countryId = null);
        List<LocationService.LocationView> GetLocations1(Guid countryProgId, string search = null);
        _Country.CountryService CountryObj { get; }
        List<Location> GetLocations(Guid countryProgId);
    }
}
