using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/NomalAttack_2Skill")]
    public class NomalAttack_2 : BossMonsterSkill
    {
        public override bool CanUseSkill(BossContext con)
        {
            if (con == null)
                return false;

            return true;
        }

        public override bool IsOneShot()
        {
            return false;
        }

        public override bool IsFinished(BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == false)
                return true;

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                skillSlot.isRunning = false;
                skillSlot.instantiatedHitbox.SetActive(false);
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            skillSlot.isRunning = true;

            con.animation.NomalAttack2Anim();

            Vector3 direction = con.target.position - con.owner.transform.position;

            direction.y = 0;

            skillSlot.instantiatedHitbox.transform.localScale = new Vector3(1f, 1f, 3f);

            con.owner.transform.rotation = Quaternion.LookRotation(direction);

            skillSlot.instantiatedHitbox.transform.rotation = Quaternion.LookRotation(direction);

            skillSlot.instantiatedHitbox.transform.localPosition = con.owner.transform.position + con.owner.transform.forward * 2f;

            skillSlot.instantiatedHitbox.SetActive(true);
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.hitboxComponent.Target != null && skillSlot.hitboxComponent.Target.TryGetComponent(out IDamageable damageable))
            {
                float damage;
                damage = (int)con.runtimeMonsterData.damage;

                DamageContext Context = new DamageContext
                {
                    Damage = (int)damage,
                    Attacker = con.owner.transform
                };

                damageable.TakeDamage(Context);
            }

            if (con.target.TryGetComponent(out IKnockbackable knockbackable) && con.phase >= 2)
            {
                knockbackable.ApplyKnockback(con.owner.transform.forward, 5f, 0.5f);
            }

            skillSlot.isRunning = false;

            skillSlot.instantiatedHitbox.SetActive(false);
        }
    }
}