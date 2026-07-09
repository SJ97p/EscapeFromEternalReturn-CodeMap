using UnityEngine;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    [CreateAssetMenu(menuName = "Monster/BossMonsterSkill/SpeedMovementSkill")]
    public class SpeedMovement : BossMonsterSkill
    {
        private float _originalMoveSpeed;
        private bool _isMoveSkillActive = false;

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
            if (_isMoveSkillActive == false && skillSlot.isRunning == false)
                return true;

            return false;
        }

        public override void SkillUpdate(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (con.runtimeMonsterData.currentHp <= 0)
            {
                _isMoveSkillActive = false;
                skillSlot.isRunning = false;
            }

            if (con.target != null && _isMoveSkillActive == true)
            {
                con.movement.Moveto(con.target.position);

                if (con.movement.IsArrived(con.target.position) == true)
                {
                    con.runtimeMonsterData.moveSpeed = _originalMoveSpeed;

                    con.animation.EndMoveSkillAnim();

                    skillSlot.Reset();

                    _isMoveSkillActive = false;

                    skillSlot.isRunning = false;
                }
            }
        }

        public override void UseSkill(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            if (skillSlot.isRunning == true)
                return;

            skillSlot.Init(con);

            if (con.target == null)
                return;

            _originalMoveSpeed = con.runtimeMonsterData.moveSpeed;

            con.runtimeMonsterData.moveSpeed *= 1.5f;

            con.animation.StartMoveSkillAnim();

            _isMoveSkillActive = true;

            skillSlot.isRunning = true;
        }

        public override void OnDamage(BossContext con, BossMonsterSkillSlot skillSlot)
        {
            
        }
    }
}