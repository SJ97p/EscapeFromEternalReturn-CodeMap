namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class Selector : BossMonsterNode
    {
        public Selector(string name, int priority) : base(name, priority) { }

        public override Status Process()
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();

                if (status == Status.Running)
                    return Status.Running;

                if (status == Status.Success)
                {
                    Reset();
                    return Status.Success;
                }

                currentChild++;
            }

            Reset();
            return Status.Failure;
        }
    }
}