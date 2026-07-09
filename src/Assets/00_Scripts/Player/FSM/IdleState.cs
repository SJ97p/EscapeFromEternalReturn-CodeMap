namespace HBDinosaur_ER_Project.Player
{
    public class IdleState : IPlayerState
    {
        private PlayerFSM fsm;
        public bool IsOccupying => false;
        public int Priority => 2;
        public IdleState(PlayerFSM fsm)
        {
            this.fsm = fsm;
        }

        public void Enter()
        {
            fsm.Movement.StopMove();
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}