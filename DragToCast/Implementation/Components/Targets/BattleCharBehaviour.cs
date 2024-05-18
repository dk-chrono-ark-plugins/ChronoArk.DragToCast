using ChronoArkMod.Helper;
using DragToCast.Api;

namespace DragToCast.Implementation.Components.Targets;

#nullable enable

internal class BattleCharBehaviour : HoverBehaviour
{
    public required BattleChar Attached { get; set; }

    public override void Accept(ICastable castable)
    {
        if (castable.SkillData.IsCastOnClick()) {
            castable.Cast();
        } else {
            Attached.Click();
        }
    }

    public override bool IsValidTargetOf(ICastable castable)
    {
        return castable.SkillData.IsCastOnClick()
            || BattleSystem.IsSelect(Attached, castable.SkillData);
    }
}
