namespace HBDinosaur_ER_Project.Monster
{
    public class BossMonsterStateMachine
    {
        private IBossMonsterFSM fsm;
        public IBossMonsterFSM FSM { get { return fsm; } }

        public void ChangeBossStateFSM(IBossMonsterFSM newfsm)
        {
            if (newfsm == null || fsm == newfsm) return;

            fsm?.Exit();
            fsm = newfsm;
            fsm.Enter();
        }

        public void Update()
        {
            fsm?.Update();
        }
    }
}