using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class DamageContext
    {
        public int Damage;
        public Transform Attacker;

        public Vector3 HitPoint;
        public Vector3 HitDirection;

        public bool IsCritical;
    }
}