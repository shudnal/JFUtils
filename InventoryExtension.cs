namespace JFUtils;

public static class InventoryExtension
{
    [Obsolete]
    private static bool RemoveOneItemByLocKey(this Inventory inventory, string itemLocKey)
    {
        var findItem = inventory.m_inventory.Find(x => x.m_shared.m_name == itemLocKey);
        if (findItem == null) return false;
        if (findItem.m_stack > 1)
        {
            --findItem.m_stack;
            inventory.Changed();
        } else
        {
            inventory.m_inventory.Remove(findItem);
            inventory.Changed();
        }

        return true;
    }

    [Obsolete]
    private static bool GiveItemIfNotHave(this Inventory inventory, string prefabName, int count = 1)
    {
        var item = ObjectDB.instance.GetItem(prefabName);
        if (!item) return false;
        if (inventory.ContainsItemByName(item.m_itemData.m_shared.m_name)) return false;
        inventory.AddItem(item.gameObject, count);

        return true;
    }
}