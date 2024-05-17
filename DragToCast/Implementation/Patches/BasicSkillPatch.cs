﻿using DragToCast.Api;
using DragToCast.Helper;
using DragToCast.Implementation.Components.Skills;
using HarmonyLib;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class BasicSkillPatch : IPatch
{
    public string Id => "basic-skill";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        var harmony = DragToCastMod.Instance!._harmony!;
        harmony.Patch(
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

        __instance.PadTarget.gameObject.GetOrAddComponent<BasicSkillBehaviour>();
    }
}
