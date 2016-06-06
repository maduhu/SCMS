using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCMS.CoreBusinessLogic.InsuranceType
{
   public interface IInsuranceTypeService
    {
       bool IsInsuranceTypeSaved(Model.InsuranceType entity);
       bool IsInsuranceTypeEdited(Model.InsuranceType entity);
       List<Model.InsuranceType> GetInsuranceTypes(Guid cpId);
       bool IsInsuranceTypeDeleted(Guid makeId);
       Model.InsuranceType GetInsuranceTypeById(Guid insuranceId);
    }
}
