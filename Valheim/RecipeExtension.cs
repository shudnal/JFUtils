namespace JFUtils.Valheim;

public static class RecipeExtension
{
    public static List<(ItemDrop.ItemData.SharedData, int)> ToList(this Recipe recipe)
    {
        if (!recipe || recipe.m_resources == null) return null;
        var result = new (ItemDrop.ItemData.SharedData, int)[recipe.m_resources.Length].ToList();
        for (var i = 0; i < recipe.m_resources.Length; i++)
        {
            var resource = recipe.m_resources[i];
            result[i] = (resource.m_resItem?.m_itemData.m_shared, resource.m_amount);
        }

        return result;
    }
}