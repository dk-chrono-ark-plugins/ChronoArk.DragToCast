using ChronoArkMod.Helper;
using DragToCast.Api;
using DragToCast.Implementation.Components;
using DragToCast.Implementation.Components.Skills;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

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
        harmony.Patch(
            original: AccessTools.Method(
                typeof(BasicSkill),
                "Update"
            ),
            transpiler: new(typeof(BasicSkillPatch), nameof(OnIlGenerated))
        );
    }

    private static void OnStart(BasicSkill __instance)
    {
        if (BattleSystem.instance == null) {
            return;
        }

        __instance.PadTarget.gameObject.GetOrAddComponent<BasicSkillBehaviour>();
    }

    private static IEnumerable<CodeInstruction> OnIlGenerated(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var codes = new List<CodeInstruction>(instructions);
        var preCheck = AccessTools.Method(
            typeof(BasicSkillPatch),
            nameof(IsPreActivated),
            [
                typeof(Skill),
            ]
        );
        var preCheckArg = AccessTools.Field(
            typeof(BasicSkill),
            nameof(BasicSkill.buttonData)
        );
        var patched = false;

        for (var i = 0; i < codes.Count; ++i) {
            if (i <= codes.Count - 6 && !patched &&
                codes[i].opcode == OpCodes.Ldarg_0 &&
                codes[i + 1].opcode == OpCodes.Ldfld &&
                codes[i + 2].opcode == OpCodes.Ldstr &&
                codes[i + 2].operand.ToString() == "Using" &&
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

    internal static bool IsPreActivated(Skill skill)
    {
        var dragging = DragBehaviour.CurrentDragging as DraggableSkill;
        return dragging != null && dragging.IsDragging && dragging.SkillData == skill;
    }
}
