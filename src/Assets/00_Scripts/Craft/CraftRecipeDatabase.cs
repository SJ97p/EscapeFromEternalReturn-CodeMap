using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    [CreateAssetMenu(fileName = "CraftRecipeDatabase", menuName = "HBDinosaur/Crafting/Recipe Database")]
    public class CraftRecipeDatabase : ScriptableObject
    {
        [SerializeField] private List<CraftRecipe> recipes = new();

        private Dictionary<int, CraftRecipe> recipeMap;
        private bool isInitialized;

        private void OnEnable()
        {
            recipeMap = null;
            isInitialized = false;
        }

        private void EnsureInitialized()
        {
            if (isInitialized && recipeMap != null)
                return;

            recipeMap = new Dictionary<int, CraftRecipe>(recipes.Count);

            for (int i = 0; i < recipes.Count; i++)
            {
                CraftRecipe recipe = recipes[i];
                if (recipe == null)
                    continue;

                int resultItemId = recipe.ResultItemId;

                if (!recipeMap.TryAdd(resultItemId, recipe))
                {
                    Debug.LogWarning(
                        $"[CraftRecipeDatabase] Duplicate recipe resultItemId: {resultItemId}, Recipe: {recipe.name}",
                        this);
                }
            }

            isInitialized = true;
        }

        public bool TryGetRecipeByResultItemId(int itemId, out CraftRecipe recipe)
        {
            EnsureInitialized();
            return recipeMap.TryGetValue(itemId, out recipe);
        }
        public IReadOnlyList<CraftRecipe> GetAllRecipes()
        {
            return recipes;
        }
    }
}