using ChronoArkMod.Plugin;
using DragToCast.Api;
using DragToCast.Implementation.Patches;
using System.Collections.Generic;
using UnityEngine;

namespace DragToCast;

#nullable enable

public class DragToCastMod : ChronoArkPlugin
{
    public static DragToCastMod? Instance;
    private readonly List<IPatch> _patches = [];

    public override void Dispose()
    {
        Instance = null;
    }

    public override void Initialize()
    {
        Instance ??= this;

        var guid = GetGuid();
        _patches.Add(new BattleSystemPatch(guid));

        _patches.Add(new BattleCharPatch(guid));
        _patches.Add(new BasicSkillPatch(guid));
        _patches.Add(new SkillButtonPatch(guid));

        _patches.Add(new CursorPatch(guid));

        foreach (var patch in _patches) {
            if (patch.Mandatory) {
                Debug.Log($"patching {patch.Id}");
                patch.Commit();
                Debug.Log("success!");
            }
        }
    }
}
