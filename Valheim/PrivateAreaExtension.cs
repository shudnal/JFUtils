namespace JFUtils.Valheim;

public static class PrivateAreaExtension
{
    public static bool InsideActiveFactionArea(this PrivateArea area, Vector3 point, Character.Faction faction)
    {
        if (area.m_ownerFaction == faction && area.IsEnabled() && area.IsInside(point, 0.0f))
            return true;

        return false;
    }

    public static bool InsideAnyActiveFactionArea(this Vector3 point, Character.Faction faction)
    {
        foreach (var area in PrivateArea.m_allAreas)
            if (area.InsideActiveFactionArea(point, faction))
                return true;

        return false;
    }
}