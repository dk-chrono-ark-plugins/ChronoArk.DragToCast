using DragToCast.Helper;
using GameDataEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DragToCast.Implementation.Components;

#nullable enable

internal class DragBehaviour : MonoBehaviour
{
    internal CastingLineRenderer.Curvature _curvature = CastingLineRenderer.Curvature.BezierQuadratic;

    private bool _setToCancel;

    internal static DragBehaviour? CurrentDragging { get; private set; }
    internal bool IsDragging { get; private set; }
    internal bool SelfActive { get; private set; }
    internal SkillButton? SkillButton => gameObject.GetComponent<SkillButton>();
    internal BasicSkill? BasicSkill => gameObject.GetComponentInParent<BasicSkill>();
    internal Skill? Myskill => SkillButton?.Myskill ?? BasicSkill?.buttonData;
    internal bool Interactable
    {
        get
        {
            if (BasicSkill != null) {
                return BasicSkill.interactable;
            }
            if (SkillButton != null) {
                return SkillButton.interactable || (BattleSystem.instance != null && BattleSystem.instance.AllyTeam.DiscardCount > 0);
            }
            return true;
        }
    }
    private string AnimationActivateValue => BasicSkill != null ? "Using" : "Selected";

    private void Start()
    {
        var eventTrigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        eventTrigger.AddOrMergeTrigger(EventTriggerType.Drag, OnDrag);
        eventTrigger.AddOrMergeTrigger(EventTriggerType.EndDrag, OnEndDrag);
        eventTrigger.AddOrMergeTrigger(EventTriggerType.PointerExit, OnPointerExit);
    }

    private void Update()
    {
        if (IsDragging && CurrentDragging == this) {
            if (Input.GetMouseButton(1)) {
                SetToCancel();
            }
        }
    }

    private void OnDestroy()
    {
        IsDragging = false;
        SelfActive = false;
        CurrentDragging = null;
        CastingLineRenderer.Instance?.Clear();
        BattleSystem.instance?.ActWindow.TargetSelectText.SetActive(value: false);
    }

    private void SetToCancel()
    {
        _setToCancel = true;
        if (SelfActive) {
            PreActivateSkill(activate: false);
        }
        OnDestroy();
    }

    private void ActivateSkill()
    {
        SkillButton?.Click();
        BasicSkill?.Click();
    }

    private void PreActivateSkill(bool activate = true)
    {
        var ani = BasicSkill?.MainAni
            ?? SkillButton?.MainAni
            ?? throw new MissingComponentException();
        if (activate != ani.GetBool(AnimationActivateValue)) {
            ActivateSkill();
            SelfActive = activate;
        }
    }

    private bool IsCastOnClick(Skill skill)
    {
        string[] consumeOnClickTypes = [
            GDEItemKeys.s_targettype_Misc,
            GDEItemKeys.s_targettype_choiceskill,
        ];
        return consumeOnClickTypes.Contains(skill.TargetTypeKey);
    }

    private void OnDrag(BaseEventData data)
    {
        if (!Interactable || _setToCancel) {
            ((PointerEventData)data).pointerDrag = null;
            _setToCancel = false;
            return;
        }

        IsDragging = true;
        CurrentDragging = this;
        BattleSystem.instance?.ActWindow.TargetSelectText.SetActive(value: true);
        CastingLineRenderer.Instance?.DrawToPointer(GetComponent<RectTransform>().position, _curvature);
    }

    private void OnEndDrag(BaseEventData data)
    {
        if (!Interactable || _setToCancel || ((PointerEventData)data).pointerDrag == null) {
            _setToCancel = false;
            return;
        }

        var hovering = HoverBehaviour.CurrentHovering;
        if (IsCastOnClick(Myskill!) && hovering != gameObject && (BasicSkill == null || BasicSkill.buttonData.Master.gameObject != hovering?.gameObject)) {
            // consume on click and not self cast
            ActivateSkill();
        } else if (hovering == null) {
            // released on empty
            if (SelfActive) {
                PreActivateSkill(activate: false);
            }
        } else if (hovering == gameObject) {
            // released on self, do nothing
            // let game handle its own click
        } else if (hovering.TryGetComponent<BattleChar>(out var target)) {
            // released on a target
            // we don't cast consume on click skills on self
            // must drag it out
            if (BattleSystem.IsSelect(target, Myskill!) && (!IsCastOnClick(Myskill!) || target != Myskill!.Master)) {
                target.Click();
            } else {
                PreActivateSkill(activate: false);
            }
        } else if (hovering.TryGetComponent<BasicSkill>(out var basic)) {
            // this happens rarely but does exit
            // so we are actually aiming the ally char
            var ally = basic.buttonData.Master;
            if (BasicSkill != null && BasicSkill.buttonData.Master == ally) {
                // if we are dragging a basic skill on itself
                // this won't fall into first comparison because
                // BasicSkill are attached to its parent object
                if (SelfActive) {
                    PreActivateSkill(activate: false);
                }
            } else if (BattleSystem.IsSelect(ally, Myskill!)) {
                ally.Click();
            } else {
                PreActivateSkill(activate: false);
            }
        } else if (hovering.TryGetComponent<TrashButton>(out var _)) {
            // released skill cards on trashcan
            SkillButton?.ClickWaste();
        } else if (hovering.TryGetComponent<SkillButton>(out var skill)) {
            // try release on skill button
            if (Myskill!.TargetTypeKey == GDEItemKeys.s_targettype_skill ||
                Myskill!.TargetTypeKey == GDEItemKeys.s_targettype_allskill) {
                skill.Click();
            }
            if (SelfActive) {
                PreActivateSkill(activate: false);
            }
        }
        OnDestroy();
    }

    private void OnPointerExit(BaseEventData data)
    {
        if (IsDragging && !SelfActive && CurrentDragging == this && !IsCastOnClick(Myskill!)) {
            PreActivateSkill(activate: true);
        }
    }
}
