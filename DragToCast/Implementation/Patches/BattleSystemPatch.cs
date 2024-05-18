using ChronoArkMod.Helper;
using DragToCast.Api;
using DragToCast.Implementation.Components;
using DragToCast.Implementation.Components.Targets;
using HarmonyLib;
using System.Collections.Generic;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class BattleSystemPatch : IPatch
{
    public static readonly List<int> PatchedHoverables = [];

    public string Id => "battle-system";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        var harmony = DragToCastMod.Instance!._harmony!;
        harmony.Patch(
            original: AccessTools.Method(
                typeof(BattleSystem),
                "Start"
            ),
            postfix: new(typeof(BattleSystemPatch), nameof(OnStart))
        );
    }

    private static void OnStart(BattleSystem __instance)
    {
        PatchedHoverables.Clear();
        __instance.gameObject.GetOrAddComponent<CastingLineRenderer>();
        __instance.ActWindow.TrashButton.GetOrAddComponent<TrashButtonBehaviour>();
    }
}
