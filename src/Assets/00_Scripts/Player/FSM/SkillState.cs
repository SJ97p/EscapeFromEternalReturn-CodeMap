using HBDinosaur_ER_Project.Player.Skill;
namespace HBDinosaur_ER_Project.Player
{
    public class SkillState : IPlayerState
    {
        private PlayerFSM fsm;
        private int skillNum;
        private SkillContext context;
        public bool IsOccupying => fsm.SkillCaster.IsCasting;
        public int Priority => 3;

        public SkillState(PlayerFSM fsm)
        {
            this.fsm = fsm;
        }

        public void SetContext(int num, SkillContext context)
        {
            skillNum = num;
            this.context = context;
        }

        public void Enter()
        {
            if (fsm.SkillManager.UseSkill(skillNum, context))
            {
                context.Caster.LookAt(context.LookPosition);
            }
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (!fsm.SkillCaster.IsCasting)
            {
                fsm.ChangeState(fsm.Idle);
            }
        }
    }
}