using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._ItemClassification
{
    public interface IItemClassificationService
    {
        bool AddItemClassification(ItemClassification itemClassification);
        bool EditItemClassification(ItemClassification itemClassification);
        bool DeleteItemClassification(Guid id);
        ItemClassification GetItemClassification(Guid id);
        List<UnitOfMeasure> GetUnitOfMessures(Guid CPId);
        List<ItemClassification> GetItemClassifications(Guid cpId, string search = null);
    }
}
