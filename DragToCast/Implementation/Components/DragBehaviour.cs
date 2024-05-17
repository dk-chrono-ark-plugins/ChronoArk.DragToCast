using DragToCast.Api;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components;

#nullable enable

internal class DragBehaviour : HoverBehaviour, IDraggable
{
    protected bool _isDragging;
    protected bool _setToCancel;

    public virtual bool Interactable { get; }
    public virtual bool IsDragging => _isDragging;
    public virtual bool IsDelayed { get; }
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

        CastingLineRenderer.Instance?.Clear();
        BattleSystem.instance?.ActWindow.TargetSelectText.SetActive(value: false);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (_setToCancel || !Interactable) {
            OnDestroy();
            return;
        }

        if (BattleSystem.instance != null) {
            var clr = CastingLineRenderer.Instance;
            if (clr != null) {
                clr.DrawToPointer(
                    GetComponent<RectTransform>().position,
                    CastingLineRenderer.Curvature.BezierQuadratic
                );
                _isDragging = true;
                CurrentDragging = this;
                BattleSystem.instance?.ActWindow.TargetSelectText.SetActive(value: true);
            }
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        OnDestroy();
        _setToCancel = false;
    }
}
