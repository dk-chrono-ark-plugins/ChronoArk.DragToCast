using ChronoArkMod.Helper;
using DragToCast.Api;
using GameDataEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components.Skills;

#nullable enable

internal class SkillButtonBehaviour : DraggableSkill
{
    private static GameObject? _discardIndicator;
    private bool _discardIndicatorOwning;

    public override bool Interactable => SkillImpl != null && (SkillImpl.interactable || base.Interactable) && !SkillImpl.IsUseBig && !SkillImpl.IsNowCasting;
    public override ICastable.CastingType CastType => ICastable.CastingType.SkillButton;
    public override Skill SkillData => SkillImpl!.Myskill;
    public override bool IsSelfActive => SkillImpl?.MainAni.GetBool("Selected") ?? false;

    private SkillButton? SkillImpl => GetComponent<SkillButton>();

    public override void OnDestroy()
    {
        DetachDiscardIndicator();
        base.OnDestroy();
    }

    public override void Cast()
    {
        SkillImpl?.Click();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (!_isDragging) {
            return;
        }
        AttachDiscardIndicator();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        DetachDiscardIndicator();
        base.OnEndDrag(eventData);
    }

    public override void Accept(ICastable castable)
    {
        Cast();
    }

    public override bool IsValidTargetOf(ICastable castable)
    {
        // skills can't be casted on itself
        return castable.SkillData.TargetTypeKey == GDEItemKeys.s_targettype_allskill
            || castable.SkillData.TargetTypeKey == GDEItemKeys.s_targettype_skill;
    }

    public override void PreActivateSkill()
    {
        if (SkillData.IsCastOnClick()) {
            SkillImpl?.MainAni?.SetBool("Selected", true);
        } else if (!IsSelfActive) {
            Cast();
        }
    }

    public override void DeactivateSkill()
    {
        if (SkillData.IsCastOnClick()) {
            SkillImpl?.MainAni?.SetBool("Selected", false);
        } else if (IsSelfActive) {
            Cast();
        }
    }

    private void AttachDiscardIndicator()
    {
        if (_discardIndicatorOwning && _discardIndicator != null) {
            return;
        }

        var trashButton = SkillImpl?.WasteButton;
        var discardUI = BattleSystem.instance?.ActWindow.TrashButton;
        var wasteMode = SkillImpl!.Myskill.IsWaste || SkillImpl!.Myskill.isExcept || SkillImpl!.Myskill.Master.GetStat.Stun;
        if (trashButton != null && discardUI != null) {
            if (_discardIndicator != null) {
                // shouldn't happen but in case
                DestroyImmediate(_discardIndicator);
            }
            _discardIndicator = Instantiate(trashButton);
            _discardIndicator.transform.SetParent(discardUI.transform, false);

            _discardIndicator.GetComponent<Animator>().SetBool("Trash", !wasteMode);
            _discardIndicator.GetComponent<Animator>().Play("TrashButton_On");

            _discardIndicatorOwning = true;
        }
    }

    private void DetachDiscardIndicator()
    {
        if (_discardIndicatorOwning) {
            DestroyImmediate(_discardIndicator);
            _discardIndicator = null;
            _discardIndicatorOwning = false;
        }
    }
}
