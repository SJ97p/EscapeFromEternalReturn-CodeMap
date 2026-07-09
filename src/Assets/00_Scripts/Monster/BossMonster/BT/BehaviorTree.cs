namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BehaviorTree : BossMonsterNode
    {
        public BehaviorTree(string name) : base(name) { }

        public override Status Process()
        {
            currentChild = 0;

            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();

                if (status != Status.Success)
                    return status;

                currentChild++;
            }

            return Status.Success;
        }
    }
}