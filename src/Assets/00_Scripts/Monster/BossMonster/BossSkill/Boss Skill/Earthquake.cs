using HBDinosaur_ER_Project.Player;
using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/EarthquakeSkill")]
    public class Earthquake : BossMonsterSkill
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

            con.animation.SkillAttack1Anim();

            Vector3 projectileCount = GetProjectileCount(con.phase);

            skillSlot.instantiatedHitbox.transform.localScale = projectileCount;

            skillSlot.instantiatedHitbox.transform.position = con.owner.transform.position;

            skillSlot.instantiatedHitbox.SetActive(true);

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.hitboxComponent.Target != null)
            {
                DamageContext context = new DamageContext
                {
                    Damage = (int)con.runtimeMonsterData.damage,
                    Attacker = con.owner.transform
                };

                if (skillSlot.hitboxComponent.Target.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(context);
                }

                Debug.Log("플레이어가 공격에 맞았습니다!");
            }
            skillSlot.instantiatedHitbox.SetActive(false);

            skillSlot.isRunning = false;

            skillSlot.Reset();
        }

        private Vector3 GetProjectileCount(int phase)
        {
            switch (phase)
            {
                case 1:
                    return new Vector3(5, 5, 5);

                case 2:
                    return new Vector3(7, 7, 7);

                case 3:
                    return new Vector3(10, 10, 10);
            }

            return new Vector3(5, 5, 5);
        }
    }
}