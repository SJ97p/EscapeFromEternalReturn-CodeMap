using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Player;
using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Effects
{
    public class SlowEffect : IStateEffect
    {
        private float slowPercentage;
        private float duration;

        public SlowEffect(float slowPercentage, float duration)
        {
            this.slowPercentage = slowPercentage;
            this.duration = duration;
        }

        public void ApplyStatus(IDamageable target, SkillContext context)
        {
            if (target is MonoBehaviour mb && mb.TryGetComponent(out ISlowable slowable))
            {
                slowable.ApplySlow(slowPercentage, duration);
            }
        }
    }
}
