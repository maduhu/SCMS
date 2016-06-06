using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._UnitOfMeasure
{
    public interface IUnitOfMeasureService
    {
        bool AddUnitOfMeasure(UnitOfMeasure unitOfMeasure);
        bool EditUnitOfMeasure(UnitOfMeasure unitOfMeasure);
        bool DeleteUnitOfMeasure(Guid id);
        UnitOfMeasure GetUnitOfMeasure(Guid id);
        List<UnitOfMeasure> GetUnitsOfMeasure(Guid cpId, string search = null);
    }
}
