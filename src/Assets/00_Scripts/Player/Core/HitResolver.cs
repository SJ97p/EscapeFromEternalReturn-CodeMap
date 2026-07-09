using HBDinosaur_ER_Project.Common;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class HitResolver : MonoBehaviour, IDamageable, IStunnable, ISlowable, IKnockbackable
    {
        private PlayerCommand command;
        private PlayerStat stat;

        private void Awake()
        {
            command = GetComponent<PlayerCommand>();
            stat = GetComponent<PlayerStat>();
        }
        public void TakeDamage(DamageContext damageContext)
        {
            Quaternion rot = transform.rotation * Quaternion.Euler(0, 90, 0);
            VFXManager.Instance.HitVFX(transform.position, rot, transform);
            if (damageContext.Attacker == gameObject.transform) return;
            stat.HP -= damageContext.Damage;
            if (stat.HP <= 0)
            {
                command.DeadCommand();
            }
        }

        public void ApplyStun(float duration)
        {
            command.StunCommand(duration);
            Debug.Log("스턴 발생");
        }

        public void ApplySlow(float slowPercentage, float duration)
        {
            BuffData data = new BuffData
            {
                Type = BuffType.Slow,
                Duration = duration,
                SlowPercentage = slowPercentage
            };
            GetComponent<BuffManager>().ApplyBuff(data);
            Debug.Log("슬로우 발생");
        }

        public void ApplyKnockback(Vector3 direction, float knockbackSpeed, float knockbackDistacne)
        {
            command.KnockbackCommand(direction, knockbackSpeed, knockbackDistacne);
            Debug.Log("넉백 발생");
        }

        public float GetDefense()
        {
            return stat.Defense;
        }
    }
}