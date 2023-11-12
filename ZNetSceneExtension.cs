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

    public static List<GameObject> GetPrefabs(this ZNetScene zNetScene, params string[] names) =>
        names.Select(zNetScene.GetPrefab).ToList();

    public static List<GameObject> GetPrefabs(this ZNetScene zNetScene, params int[] names) =>
        names.Select(zNetScene.GetPrefab).ToList();
}