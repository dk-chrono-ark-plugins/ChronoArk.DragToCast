using ChronoArkMod.Plugin;
using DragToCast.Api;
using DragToCast.Implementation.Patches;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace DragToCast;

#nullable enable

public class DragToCastMod : ChronoArkPlugin
{
    private static DragToCastMod? _instance;
    private readonly List<IPatch> _patches = [];

    public static DragToCastMod? Instance => _instance;
    internal Harmony? _harmony;

    public override void Dispose()
    {
        _instance = null;
    }

    public override void Initialize()
    {
        _instance = this;
        _harmony = new(GetGuid());

        _patches.Add(new BattleSystemPatch());

        _patches.Add(new BattleCharPatch());
        _patches.Add(new BasicSkillPatch());
        _patches.Add(new SkillButtonPatch());

        _patches.Add(new HoverUIPatch());

        foreach (var patch in _patches) {
            if (patch.Mandatory) {
                Debug.Log($"patching {patch.Id}");
                patch.Commit();
                Debug.Log("success!");
            }
        }
    }
}
