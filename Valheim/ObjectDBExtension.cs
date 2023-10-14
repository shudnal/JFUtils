namespace JFUtils.Valheim;

public static class ObjectDBExtension
{
    public static ItemDrop GetItem(this ObjectDB objectDB, string name) => objectDB.GetItemPrefab(name)?.GetComponent<ItemDrop>();

    public static ItemDrop GetItem(this ObjectDB objectDB, int hash) => objectDB.GetItemPrefab(hash)?.GetComponent<ItemDrop>();
}