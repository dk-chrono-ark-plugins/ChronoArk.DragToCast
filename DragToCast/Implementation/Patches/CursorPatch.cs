using DragToCast.Api;
using DragToCast.Implementation.Components;
using HarmonyLib;
using UnityEngine;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class CursorPatch(string guid) : IPatch
{
    private Harmony? _harmony;

    public string Id => "cursor-hide";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        _harmony ??= new(guid);
        _harmony.Patch(
            original: AccessTools.PropertySetter(
                typeof(Cursor),
                nameof(Cursor.visible)
            ),
            prefix: new(typeof(CursorPatch), nameof(OnSettingChanged))
        );
    }

    private static void OnSettingChanged(ref bool value)
    {
        if (value && (DragBehaviour.CurrentDragging?.IsDragging ?? false)) {
            value = false;
        }
    }
}
