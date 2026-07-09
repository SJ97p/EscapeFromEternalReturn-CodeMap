using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player.Skill
{
    [System.Serializable]
    public class SkillSlot
    {
        public SkillId SkillId { get; private set; }

        private float cooldown;
        private float lastUseTime = -999f;

        public SkillSlot(SkillId id, float cooldown)
        {
            SkillId = id;
            this.cooldown = cooldown;
        }

        public bool CanUse()
        {
            return Time.time >= lastUseTime + cooldown;
        }

        public float GetRemainingCooldown()
        {
            return Mathf.Max(0f, (lastUseTime + cooldown) - Time.time);
        }

        public void MarkUsed()
        {
            lastUseTime = Time.time;
        }

        public void SetCooldown(float value)
        {
            cooldown = value;
        }
    }
}