using DragToCast.Helper;
using GameDataEditor;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DragToCast.Implementation;

#nullable enable

internal class DragBehaviour : MonoBehaviour
{
    internal bool IsDragging { get; private set; }
    internal bool SelfActive { get; private set; }
    private SkillButton? SkillButton => gameObject.GetComponent<SkillButton>();
    private BasicSkill? BasicSkill => gameObject.GetComponentInParent<BasicSkill>();
    private string AnimationActivateValue => BasicSkill != null ? "Using" : "Selected";
    private Skill? Myskill => SkillButton?.Myskill ?? BasicSkill?.buttonData;

    private void Start()
    {
        var eventTrigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();
        eventTrigger.AddOrMergeTrigger(EventTriggerType.Drag, OnDrag);
        eventTrigger.AddOrMergeTrigger(EventTriggerType.EndDrag, OnEndDrag);
        eventTrigger.AddOrMergeTrigger(EventTriggerType.PointerExit, OnPointerExit);
    }

    private void OnDestroy()
    {
        IsDragging = false;
        SelfActive = false;
        CastingLineRenderer.Instance?.Clear();
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
        IsDragging = true;
        var rect = GetComponent<RectTransform>() ?? throw new MissingComponentException();
        CastingLineRenderer.Instance?.DrawToPointer(rect.position, CastingLineRenderer.Curvature.BezierQuadratic);
    }

    private void OnEndDrag(BaseEventData data)
    {
        var hovering = HoverBehaviour.CurrentHovering;

        if (IsCastOnClick(Myskill!) && hovering != gameObject) {
            ActivateSkill();
        } else {
            if (hovering == null) {
                // released on empty 
                PreActivateSkill(activate: false);
            } else if (hovering == gameObject) {
                // released on self, do nothing
                // let game handle its own click
            } else if (hovering.GetComponent<BattleChar>() != null) {
                // released on a target
                var target = hovering.GetComponent<BattleChar>();
                if (BattleSystem.IsSelect(target, Myskill)) {
                    target.Click();
                } else {
                    PreActivateSkill(activate: false);
                }
            } else if (hovering.GetComponent<TrashButton>() != null) {
                // released on trashcan
                SkillButton?.WasteButton.transform
                    .GetFirstNestedChildWithName("Align/Collider")?
                    .GetComponent<Button>()
                    .onClick.Invoke();
            } else {
                hovering.GetComponent<BasicSkill>()?.Click();
                hovering.GetComponent<SkillButton>()?.Click();
            }
        }
        OnDestroy();
    }

    private void OnPointerExit(BaseEventData data)
    {
        if (IsDragging && !SelfActive) {
            // we don't pre activate Misc skills cuz they consume on click
            if (!IsCastOnClick(Myskill!)) {
                PreActivateSkill(activate: true);
            }
        }
    }
}
