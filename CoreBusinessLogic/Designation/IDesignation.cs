using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Designation
{
    public interface IDesignationService
    {
        bool AddDesignation(Designation designation);
        bool EditDesignation(Designation designation);
        bool DeleteDesignation(Guid id);
        Designation GetDesignation(Guid id);
        List<Designation> GetDesignations(Guid countryProgId, string search = null);
    }
}
