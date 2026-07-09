using UnityEngine;
using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Player.Skill
{
    public class SkillContext
    {
        public Transform Caster;
        public Transform Target;
        public Vector3 Direction;
        public Vector3 LookPosition;

        public BuffManager BuffManager;
        public float Power;
        public float Range;
        public float AttackMultiplier;
        public float SkillAmplification;
        public LayerMask TargetLayer;
        public int SkillNum;
        public List<IDamageable> HitTargets = new();
        public float ChargeRatio;

        public Vector3 GetNormalizedDirection()
        {
            return Direction.sqrMagnitude > 0.001f ? Direction.normalized : Vector3.forward;
        }
        // public bool Is
    }
}