namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftTreeNode
    {
        public int ItemId { get; private set; }
        public int NeedAmount { get; private set; }

        public CraftTreeNode Left { get; private set; }
        public CraftTreeNode Right { get; private set; }

        public bool IsCraftable => Left != null || Right != null;

        public CraftTreeNode(int itemId, int needAmount)
        {
            ItemId = itemId;
            NeedAmount = needAmount;
        }

        public void SetChildren(CraftTreeNode left, CraftTreeNode right)
        {
            Left = left;
            Right = right;
        }
    }
}