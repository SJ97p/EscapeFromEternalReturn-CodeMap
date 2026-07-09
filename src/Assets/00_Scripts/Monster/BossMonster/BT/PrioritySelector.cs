using System.Collections.Generic;
using System.Linq;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class PrioritySelector : BossMonsterNode
    {
        private List<BossMonsterNode> sortedChildren;
        private List<BossMonsterNode> SortedChildren => sortedChildren ??= SortChildren();

        private BossMonsterNode runningNode;

        protected virtual List<BossMonsterNode> SortChildren() => children.OrderByDescending(Child => Child.priority).ToList();

        public PrioritySelector(string name) : base(name) { }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }

        public override Status Process()
        {
            if (runningNode != null)
            {
                var status = runningNode.Process();

                if (status == Status.Running)
                    return Status.Running;

                runningNode.Reset();
                runningNode = null;

                return status;
            }

            foreach (var child in SortedChildren)
            {
                var status = child.Process();

                if (status == Status.Running)
                {
                    runningNode = child;
                    return Status.Running;
                }

                if (status == Status.Success)
                    return Status.Success;
            }

            return Status.Failure;
        }
    }
}