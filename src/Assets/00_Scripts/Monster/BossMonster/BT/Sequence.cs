namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class Sequence : BossMonsterNode
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();

                if (status == Status.Running)
                    return Status.Running;

                if (status == Status.Failure)
                {
                    Reset();
                    return Status.Failure;
                }

                currentChild++;
            }

            Reset();
            return Status.Success;
        }
    }
}