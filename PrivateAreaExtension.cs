namespace JFUtils;

public static class PrivateAreaExtension
{
    public static bool InsideActiveFactionArea(this PrivateArea area, Vector3 point, Character.Faction faction) =>
        area.m_ownerFaction == faction && area.IsEnabled() && area.IsInside(point, 0.0f);

    public static bool InsideAnyActiveFactionArea(this Vector3 point, Character.Faction faction)
    {
        foreach (var area in PrivateArea.m_allAreas)
            if (area.InsideActiveFactionArea(point, faction)) return true;

        return false;
    }
}