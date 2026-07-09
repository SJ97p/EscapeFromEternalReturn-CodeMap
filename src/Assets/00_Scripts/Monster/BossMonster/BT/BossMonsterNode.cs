using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Monster.BossMonster
{
    public class BossMonsterNode
    {
        public enum Status { Success, Failure, Running }

        public readonly string name;
        public readonly int priority;

        public readonly List<BossMonsterNode> children = new();
        protected int currentChild;

        public BossMonsterNode(string name = "Node", int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }

        public void AddChild(BossMonsterNode node) => children.Add(node);

        public virtual Status Process() => children[currentChild].Process();

        public virtual void Reset()
        {
            currentChild = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
}