using DragToCast.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components;

#nullable enable

internal class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITarget
{
    private static readonly List<HoverBehaviour> _hoverHierarchy = [];
    internal static ITarget? CurrentTarget
    {
        get
        {
            _hoverHierarchy.RemoveAll(go => go == null);
            return _hoverHierarchy.LastOrDefault();
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Enter(this);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Exit(this);
    }

    public virtual void Accept(ICastable castable)
    {
        throw new NotImplementedException();
    }

    public virtual bool IsValidTargetOf(ICastable castable)
    {
        throw new NotImplementedException();
    }

    protected static void Enter(HoverBehaviour ho)
    {
        _hoverHierarchy.Add(ho);
    }

    protected static void Exit(HoverBehaviour ho)
    {
        _hoverHierarchy.Remove(ho);
    }
}
