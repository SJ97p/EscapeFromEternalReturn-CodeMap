using HBDinosaur_ER_Project.Crafting.UI;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftingDebugRunner : MonoBehaviour
    {
        [SerializeField] private CraftRecipeDatabase recipeDatabase;
        [SerializeField] private CraftTreeRenderer treeRenderer;
        [SerializeField] private int testItemId;

        private void Start()
        {
            if (recipeDatabase == null)
            {
                Debug.LogError("[CraftingDebugRunner] RecipeDatabase is null.", this);
                return;
            }

            if (treeRenderer == null)
            {
                Debug.LogError("[CraftingDebugRunner] TreeRenderer is null.", this);
                return;
            }

            CraftTreeBuilder treeBuilder = new CraftTreeBuilder(recipeDatabase);
            CraftTreeNode rootNode = treeBuilder.BuildTree(testItemId);

            //treeRenderer.Render(rootNode);
        }
    }
}