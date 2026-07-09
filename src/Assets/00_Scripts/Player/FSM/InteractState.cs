using HBDinosaur_ER_Project.Common;
using HBDinosaur_ER_Project.Core;
using HBDinosaur_ER_Project.UI;
using UnityEngine;

namespace HBDinosaur_ER_Project.Player
{
    public class InteractState : IPlayerState
    {
        private PlayerFSM fsm;
        private Transform target;

        public InteractState(PlayerFSM fsm)
        {
            this.fsm = fsm;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void Enter()
        {
            if (target == null) return;
            fsm.Movement.StopMove();
            if (target.TryGetComponent(out IDamageable _))
            {
                fsm.Attack.SetTarget(target);
                fsm.ChangeState(fsm.Attack);
                return;
            }
            if (target.TryGetComponent(out IClickable clickable))
            {
                clickable.Clicked(InputType.RIGHT_CLICK);
            }
        }

        public void Exit()
        {
            target = null;

            if (NewUIManager.Instance != null)
            {
                NewUIManager.Instance.Close(UIPanelId.HyperloopUI);
                NewUIManager.Instance.Close(UIPanelId.TargetInventory);
                NewUIManager.Instance.Close(UIPanelId.Storage);
            }
        }

        public void Update()
        {

        }

        public bool IsOccupying => false;
        public int Priority => 1;
    }
}