namespace JFUtils;

[PublicAPI]
public static class CharacterExtension
{
    public static string GetTamedName(this Character character) =>
        character.IsTamed(0) ? character.m_nview.GetZDO().GetString(ZDOVars.s_tamedName) : character.m_name;
}