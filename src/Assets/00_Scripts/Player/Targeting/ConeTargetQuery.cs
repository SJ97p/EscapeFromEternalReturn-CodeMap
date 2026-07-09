using HBDinosaur_ER_Project.Player.Skill;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class ConeTargetQuery : ISkillTargetQuery
    {
        private readonly float angle;
        private readonly float range;
        private Collider[] hits = new Collider[10];

        public ConeTargetQuery(float angle, float range)
        {
            this.angle = angle;
            this.range = range;
        }
        public List<IDamageable> Query(SkillContext context)
        {
            var results = new List<IDamageable>();

            int cnt = Physics.OverlapSphereNonAlloc(context.Caster.position, range, hits, context.TargetLayer);

            for (int i = 0; i < cnt; i++)
            {
                Vector3 dir = (hits[i].transform.position - context.Caster.position).normalized;
                if (Vector3.Angle(context.GetNormalizedDirection(), dir) <= angle * 0.5f)
                {
                    if (hits[i].TryGetComponent(out IDamageable dmg))
                    {
                        results.Add(dmg);
                    }
                }

            }
            return results;
        }
    }
}