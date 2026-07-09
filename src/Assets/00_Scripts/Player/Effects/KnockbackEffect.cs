using HBDinosaur_ER_Project.Player;
using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Effects
{
    public class KnockbackEffect : IStateEffect
    {
        private float knockbackSpeed;
        private float knockbackDistance;

        public KnockbackEffect(float speed, float distance)
        {
            knockbackSpeed = speed;
            knockbackDistance = distance;
        }

        public void ApplyStatus(IDamageable target, SkillContext context)
        {
            if (target is MonoBehaviour mb && mb.TryGetComponent(out IKnockbackable knockbackable))
            {
                Vector3 direction = context.GetNormalizedDirection();
                
                if (direction.sqrMagnitude < 0.001f && context.Caster != null)
                {
                    direction = (mb.transform.position - context.Caster.position).normalized;
                }
                
                direction.y = 0;

                knockbackable.ApplyKnockback(direction, knockbackSpeed, knockbackDistance);
            }
        }
    }
}
