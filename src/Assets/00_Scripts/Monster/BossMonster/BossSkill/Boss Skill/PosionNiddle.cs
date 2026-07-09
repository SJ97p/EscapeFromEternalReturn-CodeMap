using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/PosionNiddleSkill")]
    public class PosionNiddle : BossMonsterSkill
    {
        public float spreadAngle = 60f;
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
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            con.animation.SkillAttack2Anim();
            con.owner.transform.LookAt(con.target);
            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            int projectileCount = GetProjectileCount(con.phase);

            float startAngle = -spreadAngle * 0.5f;

            for (int i = 0; i < projectileCount; i++)
            {
                float t = projectileCount == 1 ? 0.5f : (float)i / (projectileCount - 1);

                float angle = Mathf.Lerp(startAngle, spreadAngle * 0.5f, t);

                Quaternion rotation = Quaternion.Euler(0f, angle, 0f) * con.owner.transform.rotation;

                GameObject projectile = con.projectilePool.GetProjectile(BossProjectileType.PosionNiddle);

                projectile.transform.SetParent(null);

                projectile.GetComponent<BossProjectileAttack>().Fire(con, con.firePosition.position, rotation);

                skillSlot.isRunning = false;

                skillSlot.Reset();
            }
        }

        private int GetProjectileCount(int phase)
        {
            switch (phase)
            {
                case 1:
                    return 3;

                case 2:
                    return 5;

                case 3:
                    return 8;
            }

            return 3;
        }
    }
}