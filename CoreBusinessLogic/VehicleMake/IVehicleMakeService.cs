using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic
{
   public interface IVehicleMakeService
    {
       bool IsVMakeSaved(VehicleMake entity);
       bool IsVMakeDeleted(Guid makeId);
       VehicleMake GetMakeById(Guid makeId);
       bool IsMakeEdited(VehicleMake entity);
       List<VehicleMake> GetVMakes(Guid cpId);

       bool IsVModelSaved(Model.VehicleModel entity);
       bool IsModelEdited(Model.VehicleModel entity);
       Model.VehicleModel GetModelById(Guid modelId);
       bool IsVModelDeleted(Guid modelId);
    }
}
