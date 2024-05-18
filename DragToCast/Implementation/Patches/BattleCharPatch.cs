using ChronoArkMod.Helper;
using DragToCast.Api;
using DragToCast.Implementation.Components.Targets;
using HarmonyLib;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class BattleCharPatch : IPatch
{
    public string Id => "battle-char";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        var harmony = DragToCastMod.Instance!._harmony!;
        harmony.Patch(
            original: AccessTools.Method(
                typeof(BattleChar),
                nameof(BattleChar.Update)
            ),
            postfix: new(typeof(BattleCharPatch), nameof(OnInstantiate))
        );
    }

    private static void OnInstantiate(BattleChar __instance)
    {
        if (BattleSystem.instance == null) {
            return;
        }

        if (BattleSystemPatch.PatchedHoverables.Contains(__instance.GetInstanceID())) {
            return;
        }

        if (__instance is BattleEnemy enemy) {
            var @base = enemy.SpriteCollider.gameObject.GetOrAddComponent<BattleCharBehaviour>();
            @base.Attached = enemy;
            var ui = enemy.MyUIObject.tooltip.gameObject.GetOrAddComponent<BattleCharBehaviour>();
            ui.Attached = enemy;
            var buffList = enemy.MyUIObject.transform.GetFirstChildWithName("AlignBuff");
            if (buffList != null) {
                var buff = buffList.gameObject.GetOrAddComponent<BattleCharBehaviour>();
                buff.Attached = enemy;
            }
        } else if (__instance is BattleAlly ally) {
            var @base = __instance.gameObject.GetOrAddComponent<BattleCharBehaviour>();
            @base.Attached = ally;
        } else {
            return;
        }

        BattleSystemPatch.PatchedHoverables.Add(__instance.GetInstanceID());
    }
}
