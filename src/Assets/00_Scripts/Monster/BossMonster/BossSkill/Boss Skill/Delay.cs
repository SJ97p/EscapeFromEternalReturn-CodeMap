using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/Delay")]
    public class Delay : BossMonsterSkill
    {
        [SerializeField] private float _delayTime = 2f;
        private float _delayTimer = 0f;
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
            if (skillSlot.isRunning == true)
            {
                _delayTimer += Time.deltaTime;

                if (_delayTime <= _delayTimer)
                {
                    _delayTimer = 0f;
                    skillSlot.isRunning = false;
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            
        }
    }
}