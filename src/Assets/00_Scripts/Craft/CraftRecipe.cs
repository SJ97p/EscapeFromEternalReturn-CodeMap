using System.Collections.Generic;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    [CreateAssetMenu(fileName = "CraftRecipe", menuName = "HBDinosaur/Crafting/Recipe")]
    public class CraftRecipe : ScriptableObject
    {
        [Header("Result")]
        [SerializeField] private int resultItemId;
        [SerializeField] private int resultAmount = 1;

        [Header("Ingredients")]
        [SerializeField] private CraftIngredient ingredientA;
        [SerializeField] private CraftIngredient ingredientB;

        public int ResultItemId => resultItemId;
        public int ResultAmount => resultAmount;

        public CraftIngredient IngredientA => ingredientA;
        public CraftIngredient IngredientB => ingredientB;
    }
}