using DragToCast.Api;
using DragToCast.Helper;

namespace DragToCast.Implementation.Components.Skills;

#nullable enable

internal class BasicSkillBehaviour : DraggableSkill
{
    public override bool Interactable => SkillImpl?.interactable ?? false;
    public override bool IsDelayed => !SkillImpl!.MainAni.GetBool("Enable");
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
        return !castable.SkillData.IsCastOnClick()
            && BattleSystem.IsSelect(SkillData.Master, castable.SkillData);
    }

    public override void PreActivateSkill()
    {
        if (SkillData.IsCastOnClick()) {
            SkillImpl?.MainAni?.SetBool("Using", true);
        } else if (!IsSelfActive) {
            Cast();
        }
    }

    public override void DeactivateSkill()
    {
        if (SkillData.IsCastOnClick()) {
            SkillImpl?.MainAni?.SetBool("Using", false);
        } else if (IsSelfActive) {
            Cast();
        }
    }
}
