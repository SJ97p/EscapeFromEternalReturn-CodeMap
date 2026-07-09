using HBDinosaur_ER_Project.Player.Skill;
using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class LineTargetQuery : ISkillTargetQuery
    {
        private readonly float width;
        private readonly float length;
        private Collider[] hits = new Collider[10];

        public LineTargetQuery(float width, float length)
        {
            this.width = width;
            this.length = length;
        }
        public List<IDamageable> Query(SkillContext context)
        {
            var results = new List<IDamageable>();

            Vector3 center = context.Caster.localPosition + context.GetNormalizedDirection() * (length * 0.5f);

            int cnt = Physics.OverlapBoxNonAlloc(
                center,
                new Vector3(width, 1f, length * 0.5f),
                hits,
                Quaternion.LookRotation(context.GetNormalizedDirection()),
                context.TargetLayer);

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