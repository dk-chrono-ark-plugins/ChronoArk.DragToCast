using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DragToCast.Helper;

internal static class EventTriggerMerge
{
    internal static void AddOrMergeTrigger(this EventTrigger eventTrigger, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        foreach (var trigger in eventTrigger.triggers) {
            if (type == trigger.eventID) {
                trigger.callback.AddListener(action);
                return;
            }
        }

        var entry = new EventTrigger.Entry() { eventID = type };
        entry.callback.AddListener(action);
        eventTrigger.triggers.Add(entry);
    }
}
