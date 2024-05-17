using DragToCast.Api;
using DragToCast.Helper;
using DragToCast.Implementation.Components.Skills;
using HarmonyLib;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class SkillButtonPatch : IPatch
{
    public string Id => "skill-button";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        var harmony = DragToCastMod.Instance!._harmony!;
        harmony.Patch(
            original: AccessTools.Method(
                typeof(SkillButton),
                "Start"
            ),
            postfix: new(typeof(SkillButtonPatch), nameof(OnStart))
        );
    }

    private static void OnStart(SkillButton __instance)
    {
        if (BattleSystem.instance == null) {
            return;
        }

        __instance.gameObject.GetOrAddComponent<SkillButtonBehaviour>();
    }
}
