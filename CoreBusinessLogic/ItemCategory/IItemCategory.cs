using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._ItemCategory
{
    public interface IItemCategoryService
    {
        bool AddItemCategory(ItemCategory itemCategory);
        bool EditItemCategory(ItemCategory itemCategory);
        bool DeleteItemCategory(Guid id);
        ItemCategory GetItemCategory(Guid id);
        List<ItemCategory> GetItemCategories(string search = null);
    }
}
