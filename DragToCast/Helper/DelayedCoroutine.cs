using System;
using System.Collections;
using UnityEngine;

namespace DragToCast.Helper;

internal static class DelayedCoroutine
{
    internal static Coroutine StartDelayedCoroutine(this MonoBehaviour instance, Action action, int frames = 1)
    {
        return instance.StartCoroutine(DelayedFunctor(action, frames));
    }

    private static IEnumerator DelayedFunctor(Action action, int frames = 1)
    {
        for (int i = 0; i < frames; ++i) {
            yield return null;
        }
        action();
    }
}
