using UnityEngine;
namespace HBDinosaur_ER_Project.Player
{
    public class MoveState : IPlayerState
    {
        public bool IsOccupying => false;
        public int Priority => 1;
        private PlayerFSM fsm;
        private Vector3 destination;
        private Transform interactTarget;
        private float interactDistance;

        public MoveState(PlayerFSM fsm)
        {
            this.fsm = fsm;
        }

        public void SetDestination(Vector3 dest)
        {
            destination = dest;
            interactTarget = null;
        }

        public void SetInteractTarget(Transform target, float distance)
        {
            interactTarget = target;
            interactDistance = distance;
        }

        public void Enter()
        {
            if (interactTarget != null)
            {
                float targetDist = Mathf.Max(0.1f, interactDistance - 0.5f);
                fsm.Movement.MoveToTargetWithDistance(interactTarget.position, targetDist);
            }
            else
            {
                fsm.Movement.SetMovePath(destination);
            }
        }

        public void Exit()
        {

        }

        public void Update()
        {
            Vector3 targetPos = interactTarget != null ? interactTarget.position : destination;
            float dist = Vector3.Distance(fsm.transform.position, targetPos);

            float stopDist = 0.1f;
            if (interactTarget != null)
            {
                stopDist = interactDistance;
            }

            if (dist <= stopDist)
            {
                if (interactTarget != null && interactTarget.TryGetComponent(out IDamageable _))
                {
                    fsm.Attack.SetTarget(interactTarget);
                    fsm.ChangeState(fsm.Attack);
                }
                else if (interactTarget != null && interactTarget.TryGetComponent(out IClickable _))
                {
                    fsm.Interact.SetTarget(interactTarget);
                    fsm.ChangeState(fsm.Interact);
                }
                else
                {
                    fsm.ChangeState(fsm.Idle);
                }
            }
        }
    }
}