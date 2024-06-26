﻿using UnityEngine.EventSystems;

namespace DragToCast.Api;

#nullable enable

internal interface IDraggable : IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Determines whether this draggable can be dragged or not
    /// </summary>
    bool Interactable { get; }

    /// <summary>
    /// Able to drag, with a deferred start
    /// </summary>
    bool IsDeferred { get; }

    /// <summary>
    /// Indicates if this draggable is being dragged
    /// </summary>
    bool IsDragging { get; }
}
