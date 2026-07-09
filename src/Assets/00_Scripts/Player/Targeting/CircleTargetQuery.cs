using HBDinosaur_ER_Project.Player.Skill;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class CircleTargetQuery : ISkillTargetQuery
    {
        private readonly float radius;
        private Collider[] hits = new Collider[10];

        public CircleTargetQuery(float radius)
        {
            this.radius = radius;
        }
        public List<IDamageable> Query(SkillContext context)
        {
            var results = new List<IDamageable>();

            int cnt = Physics.OverlapSphereNonAlloc(context.Caster.position, radius, hits, context.TargetLayer);

            for (int i = 0; i < cnt; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable dmg))
                {
                    results.Add(dmg);
                }
            }
            return results;
        }
    }
}