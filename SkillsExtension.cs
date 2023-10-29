namespace JFUtils;

public static class SkillsExtension
{
    public static Skill GetCustomSkill(this Skills skills, string skillName) =>
        skills.GetSkill(skills.GetCustomSkillType(skillName));

    public static SkillDef GetCustomSkillDef(this Skills skills, string skillName) =>
        skills.GetSkillDef(skills.GetCustomSkillType(skillName));

    public static SkillType GetCustomSkillType(this Skills skills, string skillName)
    {
        if (!Enum.TryParse(skillName, out SkillType skill))
        {
            var skillDef = skills.GetSkillDef((SkillType)Mathf.Abs(skillName.GetStableHashCode()));
            if (skillDef == null) return SkillType.None;
            skill = skillDef.m_skill;
        }

        return skill;
    }

    public static string LocalizeSkill(this Skill skill)
    {
        if (skill == null || skill.m_info == null) return "NULL";
        var name = skill.m_info.m_skill.ToString();
        return Enum.TryParse(name, out SkillType _)
            ? Localization.instance.Localize("$skill_" + name.ToLower())
            : Localization.instance.Localize($"$skill_" + Mathf.Abs(name.GetStableHashCode()));
    }
}