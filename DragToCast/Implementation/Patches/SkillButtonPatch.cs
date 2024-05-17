using DragToCast.Api;
using DragToCast.Helper;
using DragToCast.Implementation.Components.Skills;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

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
        harmony.Patch(
            original: AccessTools.Method(
                typeof(SkillButton),
                "Update"
            ),
            transpiler: new(typeof(SkillButtonPatch), nameof(OnIlGenerated))
        );
    }

    private static void OnStart(SkillButton __instance)
    {
        if (BattleSystem.instance == null) {
            return;
        }

        __instance.gameObject.GetOrAddComponent<SkillButtonBehaviour>();
    }

    private static IEnumerable<CodeInstruction> OnIlGenerated(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codes = new List<CodeInstruction>(instructions);
        var preCheck = AccessTools.Method(
            typeof(BasicSkillPatch),
            nameof(BasicSkillPatch.IsPreActivated),
            [
                typeof(Skill),
            ]
        );
        var preCheckArg = AccessTools.Field(
            typeof(SkillButton),
            nameof(SkillButton.Myskill)
        );
        var patched = false;

        for (var i = 0; i < codes.Count; ++i) {
            if (i <= codes.Count - 6 && !patched &&
                codes[i].opcode == OpCodes.Ldarg_0 &&
                codes[i + 1].opcode == OpCodes.Ldfld &&
                codes[i + 2].opcode == OpCodes.Ldstr &&
                codes[i + 2].operand.ToString() == "Selected" &&
                codes[i + 3].opcode == OpCodes.Ldc_I4_0 &&
                codes[i + 4].opcode == OpCodes.Callvirt) {
                codes[i + 5].labels.Add(generator.DefineLabel());
                var disp = codes[i + 5].labels[0];

                yield return new(OpCodes.Ldarg_0);
                yield return new(OpCodes.Ldfld, preCheckArg);
                yield return new(OpCodes.Call, preCheck);
                yield return new(OpCodes.Brtrue_S, disp);
                patched = true;
            }
            yield return codes[i];
        }
    }
}
