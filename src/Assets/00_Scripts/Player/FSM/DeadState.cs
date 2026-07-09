namespace HBDinosaur_ER_Project.Player
{
    public class DeadState : IPlayerState
    {
        private PlayerFSM fsm;
        public bool IsOccupying => true;

        public int Priority => 100;
        public DeadState(PlayerFSM fsm)
        {
            this.fsm = fsm;
        }

        public void Enter()
        {
            AnimationEventBus.RaiseDead();
        }

        public void Exit()
        {

        }

        public void Update()
        {
            InputBlocker.SetBlock(true);
        }
    }
}