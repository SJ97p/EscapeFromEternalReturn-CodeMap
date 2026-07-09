using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class PlayerCommand : MonoBehaviour
    {
        private PlayerStat stat;
        private void OnEnable()
        {
            StateEventBus.OnZoneTimerZero += DeadCommand;
        }
        private void OnDestroy()
        {
            StateEventBus.OnZoneTimerZero -= DeadCommand;
        }

        private void ChangeState(PlayerFSMState state)
        {
            fsm.StateConverter(state);
        }
        private PlayerFSM fsm;

        public void MoveCommand(Vector3 dest)
        {
            fsm.Move.SetDestination(dest);
            ChangeState(PlayerFSMState.MOVE);
        }
        public void DeadCommand()
        {
            ChangeState(PlayerFSMState.DIE);
        }

        public void KnockbackCommand(Vector3 direction, float knockbackSpeed, float knockbackDistacne)
        {
            fsm.Knockback.SetData(direction, knockbackSpeed, knockbackDistacne);
            ChangeState(PlayerFSMState.KNOCKBACK);
        }

        public void StunCommand(float duration)
        {
            fsm.Stun.SetDuration(duration);
            ChangeState(PlayerFSMState.STUN);
        }

        private float interactableDistacne = 1.8f;

        public void InteractCommand(Transform target)
        {
            float range = interactableDistacne;
            if (target.TryGetComponent(out IDamageable _))
            {
                //VoiceManager.Instance.PlayVoice(Sound.VoiceSituation.TargetOn);
                range = fsm.Stat.AttackRange;
            }

            float distance = Vector3.Distance(target.position, transform.position);
            if (distance > range)
            {
                fsm.Move.SetInteractTarget(target, range);
                ChangeState(PlayerFSMState.MOVE);
            }
            else
            {
                fsm.Interact.SetTarget(target);
                ChangeState(PlayerFSMState.INTERACT);
            }
        }

        public void StopCommand()
        {
            ChangeState(PlayerFSMState.IDLE);
        }

        public void SkillCommand(int skillNum, Vector3 targetPos)
        {
            SkillContext context = stat.BuildContext(skillNum, targetPos);

            if (fsm.SkillManager.IsOccupyingSkill(skillNum))
            {
                fsm.Skill.SetContext(skillNum, context);
                ChangeState(PlayerFSMState.SKILL);
            }
            else
            {
                fsm.SkillManager.UseSkill(skillNum, context);
            }
        }

        private void Awake()
        {
            fsm = GetComponent<PlayerFSM>();
            stat = GetComponent<PlayerStat>();
        }
    }
}