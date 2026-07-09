using HBDinosaur_ER_Project.Player.Skill;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class AttackState : IPlayerState
    {
        private PlayerFSM fsm;
        private Transform target;
        private NormalAttackSkill normalAttack;
        private SkillContext context;
        public bool IsOccupying => false;
        public int Priority => 1;

        public AttackState(PlayerFSM fsm)
        {
            this.fsm = fsm;

            normalAttack = new NormalAttackSkill();
            context = new SkillContext { Caster = fsm.transform };
            normalAttack.Initialize(context, null);
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void Enter()
        {
            fsm.Movement.StopMove();

            AnimationEventBus.OnAttack += HandleAttackStarted;
            AnimationEventBus.OnAttackHit += HandleAttackHit;

        }

        public void Exit()
        {
            AnimationEventBus.OnAttack -= HandleAttackStarted;
            AnimationEventBus.OnAttackHit -= HandleAttackHit;

            if (normalAttack.IsRunning)
                normalAttack.Cancel();

            target = null;

        }

        private void HandleAttackStarted(float speed)
        {

        }

        private void HandleAttackHit()
        {

        }

        public void Update()
        {
            if (target == null)
            {
                fsm.ChangeState(fsm.Idle);
                return;
            }

            float dist = Vector3.Distance(fsm.transform.position, target.position);
            float range = fsm.Stat.AttackRange;

            if (dist > range + 0.1f)
            {
                fsm.Move.SetInteractTarget(target, range);
                fsm.ChangeState(fsm.Move);
                return;
            }

            context.Target = target;
            normalAttack.Cast();
        }

    }
}