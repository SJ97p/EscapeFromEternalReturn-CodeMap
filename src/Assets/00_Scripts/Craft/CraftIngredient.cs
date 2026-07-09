using System;
using UnityEngine;

namespace HBDinosaur_ER_Project.Crafting
{
    [Serializable]
    public class CraftIngredient
    {
        [SerializeField] private int itemId;
        [SerializeField] private int amount = 1;

        public int ItemId => itemId;
        public int Amount => amount;
    }
}