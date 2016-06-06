using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.DutyType
{
    public interface IDutyTypeService
    {
        bool IsDutyTypeSaved(Model.DutyType entity);
        bool IsDutyTypeEdited(Model.DutyType entity);
        Model.DutyType GetDutyTypeById(Guid dutyId);
        List<Model.DutyType> GetDutyTypes(Guid cpId);
        bool IsDutyTypeDeleted(Guid dutyId);
    }
}
