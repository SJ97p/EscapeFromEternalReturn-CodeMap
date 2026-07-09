using HBDinosaur_ER_Project.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player.Skill
{
    public class SkillManager : MonoBehaviour
    {
        private Dictionary<int, SkillSlot> slots = new();
        private SkillCaster caster;
        public Action<int, float> OnCooldownStart;

        [SerializeField] private List<SkillRawData> data;

        private void Awake()
        {
            caster = GetComponent<SkillCaster>();
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            for (int i = 0; i < data.Count; i++)
            {
                SkillRegistry.RegisterData(data[i]);
                slots.Add(i, new SkillSlot(data[i].skillId, data[i].Cooldown));
            }
        }

        public bool UseSkill(int slotIndex, SkillContext context)
        {
            if (!slots.TryGetValue(slotIndex, out SkillSlot slot)) return false;

            if (!slot.CanUse())
            {
                SFXManager.Instance.PlaySFX3D(Sound.SFXType.Cannot_Do_This, transform.position);
                Debug.Log($"쿨다운 중입니다. 남은 시간 {slot.GetRemainingCooldown()}");
                return false;
            }

            var skill = SkillRegistry.CreateSkill(slot.SkillId);
            if (skill == null)
            {
                Debug.LogError("[SkillMnager]: 스킬 생성 실패");
                return false;
            }

            if (skill.IsOccupying && caster.IsCasting)
            {
                if (caster.CurrentSkill?.SkillId == slot.SkillId)
                {
                    caster.HandleReCast();
                    return true;
                }

                Debug.Log("다른 스킬을 사용중입니다.");
                return false;
            }

            skill.OnFinished += () =>
            {
                slot.MarkUsed();
                OnCooldownStart?.Invoke(slotIndex, slot.GetRemainingCooldown());
            };

            caster.Cast(skill, context);

            return true;
        }

        public bool IsOccupyingSkill(int slotIndex)
        {
            if (!slots.TryGetValue(slotIndex, out SkillSlot slot)) return false;

            var skill = SkillRegistry.CreateSkill(slot.SkillId);
            return skill != null && skill.IsOccupying;
        }

        public float GetCooldown(int slotIndex)
        {
            if (!slots.TryGetValue(slotIndex, out SkillSlot slot))
            {
                return 0f;
            }
            return slot.GetRemainingCooldown();
        }
    }
}