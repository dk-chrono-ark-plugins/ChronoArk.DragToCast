using UnityEngine;

namespace DragToCast.Helper;

internal static class ComponentFetch
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    public static bool TryGetComponent<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponent<T>();
        return component != null;
    }

    public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponentInParent<T>();
        return component != null;
    }

    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>();
        return component != null;
    }
}
