using DragToCast.Api;
using DragToCast.Implementation.Components;
using HarmonyLib;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class BasicSkillPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "basic-skill";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.Method(
                typeof(BasicSkill),
                "Start"
            ),
            postfix: new(typeof(BasicSkillPatch), nameof(OnStart))
        );
    }

    private static void OnStart(BasicSkill __instance)
    {
        if (BattleSystem.instance == null) {
            return;
        }

        __instance.PadTarget.gameObject.AddComponent<DragBehaviour>();
        __instance.gameObject.AddComponent<HoverBehaviour>();
    }
}
