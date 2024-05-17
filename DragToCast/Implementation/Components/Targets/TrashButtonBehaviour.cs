using DragToCast.Api;
using DragToCast.Implementation.Components.Skills;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components.Targets;

#nullable enable

internal class TrashButtonBehaviour : HoverBehaviour
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        var draggable = DragBehaviour.CurrentDragging as DraggableSkill;
        if (draggable != null && draggable.IsDragging && IsValidTargetOf(draggable)) {
            BattleSystem.instance.WasteMode = true;
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        var draggable = DragBehaviour.CurrentDragging as DraggableSkill;
        if (draggable != null && draggable.IsDragging) {
            GetComponent<TrashButton>()?.Quit();
        }
    }

    public override void Accept(ICastable castable)
    {
        castable.SkillData.MyButton.ClickWaste();
        GetComponent<TrashButton>()?.Quit();
    }

    public override bool IsValidTargetOf(ICastable castable)
    {
        return castable.CastType != ICastable.CastingType.BasicSkill
            && BattleSystem.instance.AllyTeam.DiscardCount > 0;
    }
}
