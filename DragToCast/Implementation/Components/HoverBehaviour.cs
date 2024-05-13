using DragToCast.Helper;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation;

#nullable enable

internal class HoverBehaviour : MonoBehaviour
{
    internal static GameObject? CurrentHovering { get; private set; }

    private void Start()
    {
        var attachObject = gameObject;
        var enemy = attachObject.GetComponent<BattleEnemy>();
        if (enemy != null) {
            // attach to spritecollider for enemies
            attachObject = enemy.SpriteCollider.gameObject;
        }

        var eventTrigger = attachObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        eventTrigger.AddOrMergeTrigger(EventTriggerType.PointerEnter, (_) => OnPointerEnter(gameObject));
        eventTrigger.AddOrMergeTrigger(EventTriggerType.PointerExit, (_) => OnPointerExit(gameObject));
    }

    private void OnPointerEnter(GameObject fixedObject)
    {
        CurrentHovering = fixedObject;

        if (fixedObject.GetComponent<TrashButton>() != null && BattleSystem.instance != null && BattleSystem.instance.AllyTeam.DiscardCount > 0) {
            BattleSystem.instance.WasteMode = true;
        }
    }

    private void OnPointerExit(GameObject fixedObject)
    {
        CurrentHovering = null;

        fixedObject.GetComponent<TrashButton>()?.Quit();
    }
}
