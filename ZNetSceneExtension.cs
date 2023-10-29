namespace JFUtils;

public static class ZNetSceneExtension
{
    public static ItemDrop GetItem(this ZNetScene zNetScene, string name) =>
        zNetScene.GetPrefab(name)?.GetComponent<ItemDrop>();

    public static ItemDrop GetItem(this ZNetScene zNetScene, int hash) =>
        zNetScene.GetPrefab(hash)?.GetComponent<ItemDrop>();

    public static Character GetCharacter(this ZNetScene zNetScene, string name) =>
        zNetScene.GetPrefab(name)?.GetComponent<Character>();

    public static Character GetCharacter(this ZNetScene zNetScene, int hash) =>
        zNetScene.GetPrefab(hash)?.GetComponent<Character>();
}