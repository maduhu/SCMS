using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic.GoodsIssuedVoucher
{
    public interface IGoodsIssuedVoucherService
    {
        bool IsGIVSaved(Model.GoodsIssuedVoucher giventity);
        bool IsGIVEdited(Model.GoodsIssuedVoucher giventity);
        bool IsGIVSubmited(Model.GoodsIssuedVoucher giventity);
        bool IsGIVItemAdded(Model.GoodsIssuedVoucher giventity);
        bool IsGIVItemDeleted(Guid givItemId);
        bool IsGIVDeleted(Guid givId);
        string GenerateUniquNumber(CountryProgramme cp);
        List<WarehouseRelease> GetGIVROs();
        List<GivItemz> GetROItemsToAdd(Guid roId);
        List<Model.GoodsIssuedVoucher> GetGIVs();
    }
}
