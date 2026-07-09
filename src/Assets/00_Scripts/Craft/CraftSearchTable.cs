using System;
using CampWorkbench_Scripts.RecipeData;
using HBDinosaur_ER_Project.ItemData;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    public class CraftSearchTable : MonoBehaviour
    {
        [SerializeField] private Transform contentRoot;
        [SerializeField] private CraftSearchITableView itemViewPrefab;

        private CraftRecipeDatabase recipeDatabase;
        private Action<int> onItemSelected;

        public void Initialize(Action<int> onItemSelected, CraftRecipeDatabase recipeDatabase)
        {
            this.recipeDatabase = recipeDatabase;
            this.onItemSelected = onItemSelected;
            Refresh();
        }
        public void Refresh()
        {
            Clear();

            if (recipeDatabase == null)
                return;

            var recipes = recipeDatabase.GetAllRecipes();

            for (int i = 0; i < recipes.Count; i++)
            {
                CraftRecipe recipe = recipes[i];
                if (recipe == null)
                    continue;

                int itemId = recipe.ResultItemId;

                CraftSearchITableView view = Instantiate(itemViewPrefab, contentRoot);

                ItemDataStruct? itemData = ItemDatabase.Instance.GetItemByID(itemId);
                var palatte = ItemDatabase.Instance.colorPalette;
                view.SetData(itemId, itemData, palatte, OnClickedItem);
            }
        }
        private void OnClickedItem(int itemId)
        {
            onItemSelected?.Invoke(itemId);
        }

        private void Clear()
        {
            if (contentRoot == null)
                return;

            for (int i = contentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(contentRoot.GetChild(i).gameObject);
            }
        }

    }
}

