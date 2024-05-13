using DragToCast.Helper;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components;

#nullable enable

internal class HoverBehaviour : MonoBehaviour
{
    /// <summary>
    /// This will be the stack to the actual <see cref="GameObject"/> holder <br/>
    /// e.g. <see cref="BattleChar"/>, <see cref="SkillButton"/>, 
    /// <see cref="BasicSkill"/>, <see cref="TrashButton"/>
    /// </summary>
    internal static GameObject? CurrentHovering { get; private set; }
    private static GameObject? LastHovering { get; set; }

    private void Start()
    {
        var attachObject = gameObject;
        if (gameObject.TryGetComponent<BattleEnemy>(out var enemy)) {
            // attach to spritecollider for BattleEnemy
            attachObject = enemy.SpriteCollider.gameObject;
            // some UI elements block a small region
            // of BattleEnemy sprite, patch that too
            AttachTrigger(enemy.MyUIObject.tooltip.gameObject);
            var buffList = enemy.MyUIObject.transform.GetFirstChildWithName("AlignBuff");
            if (buffList != null) {
                AttachTrigger(buffList.gameObject);
            }
        } else if (gameObject.TryGetComponentInParent<BasicSkill>(out var basic)) {
            // attach to PadTarget but fix on actual skill
            attachObject = basic.PadTarget.gameObject;
        }

        AttachTrigger(attachObject);
    }

    private void AttachTrigger(GameObject attachObject)
    {
        var eventTrigger = attachObject.GetComponent<EventTrigger>() ?? attachObject.AddComponent<EventTrigger>();
        eventTrigger.AddOrMergeTrigger(EventTriggerType.PointerEnter, (_) => OnPointerEnter(gameObject));
        eventTrigger.AddOrMergeTrigger(EventTriggerType.PointerExit, (_) => OnPointerExit(gameObject));
    }

    private void OnPointerEnter(GameObject fixedObject)
    {
        LastHovering = CurrentHovering;
        CurrentHovering = fixedObject;

        if (DragBehaviour.CurrentDragging?.BasicSkill == null &&
            fixedObject.GetComponent<TrashButton>() != null &&
            BattleSystem.instance != null &&
            BattleSystem.instance.AllyTeam.DiscardCount > 0) {
            BattleSystem.instance.WasteMode = true;
        }
    }

    private void OnPointerExit(GameObject fixedObject)
    {
        // we need to re-assign the topology to the underlying BattleAlly
        if (LastHovering != null &&
            LastHovering.TryGetComponent<BattleAlly>(out var ally) &&
            CurrentHovering != null &&
            CurrentHovering.TryGetComponent<BasicSkill>(out var basic) &&
            basic.buttonData.Master.gameObject == ally.gameObject) {
            CurrentHovering = LastHovering;
            LastHovering = null;
        } else {
            CurrentHovering = null;
        }

        basic = DragBehaviour.CurrentDragging?.BasicSkill;
        if (basic == null) {
            fixedObject.GetComponent<TrashButton>()?.Quit();
        }
    }
}
