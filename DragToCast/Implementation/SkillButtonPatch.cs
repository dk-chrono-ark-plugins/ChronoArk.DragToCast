using DragToCast.Api;
using HarmonyLib;

namespace DragToCast.Implementation;

#nullable enable

internal class SkillButtonPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "skill-button";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
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

        __instance.gameObject.AddComponent<DragBehaviour>();
        __instance.gameObject.AddComponent<HoverBehaviour>();
    }
}
