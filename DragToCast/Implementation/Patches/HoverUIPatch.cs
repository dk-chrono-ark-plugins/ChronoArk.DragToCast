using DragToCast.Api;
using DragToCast.Implementation.Components;
using HarmonyLib;
using UnityEngine;

namespace DragToCast.Implementation.Patches;

#nullable enable

internal class HoverUIPatch : IPatch
{
    public string Id => "hover-ui";
    public string Name => Id;
    public string Description => Id;
    public bool Mandatory => true;

    public void Commit()
    {
        var harmony = DragToCastMod.Instance!._harmony!;
        harmony.Patch(
            original: AccessTools.PropertySetter(
                typeof(Cursor),
                nameof(Cursor.visible)
            ),
            prefix: new(typeof(HoverUIPatch), nameof(OnCursorVisibleSet))
        );
    }

    private static void OnCursorVisibleSet(ref bool value)
    {
        if (value && (DragBehaviour.CurrentDragging?.IsDragging ?? false)) {
            value = false;
        }
    }
}
