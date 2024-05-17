using DragToCast.Api;

namespace DragToCast.Implementation.Components.Skills;

#nullable enable

internal class BasicSkillBehaviour : DraggableSkill
{
    public override bool Interactable => SkillImpl?.interactable ?? false;
    public override bool IsDelayed => Interactable && (!SkillImpl?.MainAni.GetBool("Enable") ?? false);
    public override ICastable.CastingType CastType => ICastable.CastingType.BasicSkill;
    public override Skill SkillData => SkillImpl!.buttonData;
    public override bool IsSelfActive => SkillImpl?.MainAni.GetBool("Using") ?? false;

    private BasicSkill? SkillImpl => GetComponentInParent<BasicSkill>();

    public override void Cast()
    {
        SkillImpl?.Click();
    }

    public override void Accept(ICastable castable)
    {
        // methinks we cannot set BasicSkill as a target, or can we verily?
        // the sole circumstance is that we invoke upon the inherent BattleAlly?
        SkillData.Master.Click();
    }

    public override bool IsValidTargetOf(ICastable castable)
    {
        return BattleSystem.IsSelect(SkillData.Master, castable.SkillData);
    }
}
