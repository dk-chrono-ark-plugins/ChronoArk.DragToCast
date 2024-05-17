using GameDataEditor;

namespace DragToCast.Helper;

internal static class SkillTargetTypeCheck
{
    internal static bool IsCastOnClick(this Skill skill)
    {
        var targetType = skill.TargetTypeKey;
        return targetType == GDEItemKeys.s_targettype_Misc
            || targetType == GDEItemKeys.s_targettype_choiceskill;
    }
}
