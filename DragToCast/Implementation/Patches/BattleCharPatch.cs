using DragToCast.Api;
using DragToCast.Helper;
using DragToCast.Implementation.Components;
using HarmonyLib;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class BattleCharPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "battle-char";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(BattleChar),
                nameof(BattleChar.Update)
            ),
            postfix: new(typeof(BattleCharPatch), nameof(OnUpdate))
        );
    }

    private static void OnUpdate(BattleChar __instance)
    {
        if (BattleSystem.instance == null) {
            return;
        }

        __instance.gameObject.GetOrAddComponent<HoverBehaviour>();
    }
}
