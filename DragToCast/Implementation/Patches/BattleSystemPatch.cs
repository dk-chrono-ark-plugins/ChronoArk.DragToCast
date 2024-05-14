using DragToCast.Api;
using DragToCast.Helper;
using DragToCast.Implementation.Components;
using HarmonyLib;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class BattleSystemPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "battle-system";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(BattleSystem),
                "Start"
            ),
            postfix: new(typeof(BattleSystemPatch), nameof(OnStart))
        );
    }

    private static void OnStart(BattleSystem __instance)
    {
        __instance.gameObject.GetOrAddComponent<CastingLineRenderer>();
        __instance.ActWindow.TrashButton.GetOrAddComponent<HoverBehaviour>();
    }
}
