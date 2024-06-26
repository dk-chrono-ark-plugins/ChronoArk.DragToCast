﻿using DragToCast.Api;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components;

#nullable enable

internal class DragBehaviour : HoverBehaviour, IDraggable
{
    protected bool _isDragging;
    protected bool _setToCancel;

    public virtual bool Interactable { get; }
    public virtual bool IsDeferred { get; }
    public virtual bool IsDragging => _isDragging;
    public static DragBehaviour? CurrentDragging { get; protected set; }

    private void Update()
    {
        if (_isDragging && Input.GetMouseButton(1)) {
            OnDestroy();
            _setToCancel = true;
        }
    }

    public virtual void OnDestroy()
    {
        _isDragging = false;
        CurrentDragging = null;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (_setToCancel || !Interactable) {
            OnDestroy();
        } else {
            _isDragging = true;
            CurrentDragging = this;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        OnDestroy();
        _setToCancel = false;
    }
}
