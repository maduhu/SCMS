using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.WB
{
    public interface IWayBillService
    {
        bool SaveWB(Model.WayBill entity);
        bool ReceiveWB(Model.WayBill entity, List<Model.WarehouseReleaseItem> writemList);
        string GenerateUniquNumber(CountryProgramme cp);
        List<Model.CountrySubOffice> GetSubOffices(Guid CPId);
        List<Model.WayBill> GetWayBillsNotReceived(Guid CPId);
        
    }
}
