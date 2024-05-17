using DragToCast.Api;
using DragToCast.Helper;
using System;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components.Skills;

#nullable enable

internal class DraggableSkill : DragBehaviour, ICastable
{
    public override bool Interactable => BattleSystem.instance.AllyTeam.DiscardCount > 0;
    public virtual ICastable.CastingType CastType { get; }
    public virtual Skill SkillData => throw new NotImplementedException();
    public virtual bool IsSelfActive { get; }

    public virtual void Cast()
    {
        throw new NotImplementedException();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!_setToCancel && _isDragging && SkillData != null) {
            if (CurrentTarget == null) {
                // released on empty
                if (SkillData.IsCastOnClick()) {
                    Cast();
                }
            } else if (CurrentTarget is DraggableSkill skill && skill == this) {
                // released on self, do nothing
                // let game handle its own click
            } else if (CurrentTarget.IsValidTargetOf(this)) {
                // released on valid target
                CurrentTarget.Accept(this);
            }
        }

        DeactivateSkill();
        base.OnEndDrag(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (_isDragging && !IsSelfActive) {
            PreActivateSkill();
        }
    }

    public virtual void PreActivateSkill()
    {
        if (!IsSelfActive) {
            if (SkillData.IsCastOnClick()) {
                BattleSystem.instance.SelectedSkill = SkillData;
            } else {
                Cast();
            }
        }
    }

    public virtual void DeactivateSkill()
    {
        if (IsSelfActive) {
            if (SkillData.IsCastOnClick()) {
                BattleSystem.instance.SelectedSkill = null;
            } else {
                Cast();
            }
        }
    }
}
