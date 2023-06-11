using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory.Categories;
using UnityEngine;

namespace Hitbox.Inventory.UI.Actions
{
    [CreateAssetMenu(fileName = "New Item Equip Action", menuName = "Hitbox/Inventory/Actions/Equip")]

    public class InventoryUIItemEquipAction : InventoryUIItemAction
    {
        #region --- VARIABLES ---

        [Tooltip("Category to search for to determine if item can be equipped.")]
        public ItemCategory[] searchCategories;

        #endregion

        #region --- METHODS ---

        public override bool SupportsAction(InventoryItem invItem)
        {
            if (!base.SupportsAction(invItem)) return false;

            if (invItem.ParentContainer is not InventoryGrid) return false;

            foreach (ItemCategory searchCategory in searchCategories)
            {
                if (searchCategory.ContainsCategory(invItem.item.category)) return true;
            }

            return false;
        }

        #endregion
    }
}