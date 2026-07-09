using HBDinosaur_ER_Project.Player;
using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Effects
{
    public class StunEffect : IStateEffect
    {
        private float duration;

        public StunEffect(float duration)
        {
            this.duration = duration;
        }

        public void ApplyStatus(IDamageable target, SkillContext context)
        {
            if (target is MonoBehaviour mb && mb.TryGetComponent(out IStunnable stunnable))
            {
                stunnable.ApplyStun(duration);
            }
        }
    }
}
