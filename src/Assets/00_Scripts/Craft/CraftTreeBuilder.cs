using System.Collections.Generic;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftTreeBuilder
    {
        private readonly CraftRecipeDatabase recipeDatabase;

        public CraftTreeBuilder(CraftRecipeDatabase recipeDatabase)
        {
            this.recipeDatabase = recipeDatabase;
        }

        public CraftTreeNode BuildTree(int rootItemId, int needAmount = 1)
        {
            return BuildNode(rootItemId, needAmount, new HashSet<int>());
        }

        private CraftTreeNode BuildNode(int itemId, int needAmount, HashSet<int> visited)
        {
            CraftTreeNode node = new CraftTreeNode(itemId, needAmount);

            if (visited.Contains(itemId))
                return node;

            if (!recipeDatabase.TryGetRecipeByResultItemId(itemId, out CraftRecipe recipe))
                return node;

            visited.Add(itemId);

            CraftTreeNode leftNode = BuildNode(
                recipe.IngredientA.ItemId,
                recipe.IngredientA.Amount * needAmount,
                new HashSet<int>(visited));

            CraftTreeNode rightNode = BuildNode(
                recipe.IngredientB.ItemId,
                recipe.IngredientB.Amount * needAmount,
                new HashSet<int>(visited));

            node.SetChildren(leftNode, rightNode);
            return node;
        }
    }
}