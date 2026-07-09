using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/TekkaiSkill")]
    public class Tekkai : BossMonsterSkill
    {
        private float _skillDuration = 3f;
        private float _timer = 0f;

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

            if (skillSlot.isRunning == true)
            {
                _timer += Time.deltaTime;
                if (_timer > _skillDuration)
                {
                    skillSlot.isRunning = false;
                    skillSlot.Reset();
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            skillSlot.isRunning = true;

            con.animation.SkillAttack3Anim();
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            con.combat.Tekkai(_skillDuration);
            _timer = 0f;
        }
    }
}