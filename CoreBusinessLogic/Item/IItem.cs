using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.Model;

namespace SCMS.CoreBusinessLogic._Item
{
    public interface IItemService
    {
        bool AddItem(Item item);
        bool EditItem(Item item);
        bool DeleteItem(Guid id);
        bool AddPackage(ItemPackage pack);
        bool EditPackage(ItemPackage pack);
        bool DeletePack(Guid pkgId);
        Item GetItem(Guid id);
        Item GetItemByName(string name, Guid countryProgId);
        ItemView GetItemView(Guid id);
        List<Item> GetItems(Guid Cpid, string search = null);
        List<ItemView> GetItems1(Guid Cpid, string search = null);
        List<ItemView> SearchBySerial(Guid Cpid, string search = null);
        _ItemCategory.ItemCategoryService ItemCatObj { get; }
        _ItemClassification.ItemClassificationService ItemClassObj { get; }
        List<Model.ItemPackage> GetItemPackages(Guid itemId);
    }
}
