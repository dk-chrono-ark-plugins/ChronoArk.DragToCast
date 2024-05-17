using DragToCast.Api;

namespace DragToCast.Implementation.Components.Skills;

#nullable enable

internal class SkillButtonBehaviour : DraggableSkill
{
    public override bool Interactable => ((SkillImpl?.interactable ?? false) || base.Interactable) && !SkillImpl.IsUseBig && !SkillImpl.IsNowCasting;
    public override bool IsDelayed => Interactable && (!SkillImpl?.Ani.GetBool("On") ?? false);
    public override ICastable.CastingType CastType => ICastable.CastingType.SkillButton;
    public override Skill SkillData => SkillImpl!.Myskill;
    public override bool IsSelfActive => SkillImpl?.MainAni.GetBool("Selected") ?? false;

    private SkillButton? SkillImpl => GetComponent<SkillButton>();

    public override void Cast()
    {
        SkillImpl?.Click();
    }

    public override void Accept(ICastable castable)
    {
        Cast();
    }

    public override bool IsValidTargetOf(ICastable castable)
    {
        // skills can't be casted on itself
        return !castable.SkillData.IsTargetSkill;
    }
}
