using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash.Categories;
using UnityEngine;

namespace Hitbox.Stash.UI.Actions
{
    [CreateAssetMenu(fileName = "New Item Equip Action", menuName = "Hitbox/Inventory/Actions/Equip")]

    public class InventoryUIItemEquipAction : InventoryUIItemAction
    {
        #region Fields

        [Tooltip("Category to search for to determine if item can be equipped.")]
        public ItemCategory[] searchCategories;

        #endregion

        #region Methods

        public override bool IsCompatible(InventoryItem invItem)
        {
            if (!base.IsCompatible(invItem)) return false;

            if (invItem.ParentContainer is not InventoryGrid) return false;

            foreach (ItemCategory searchCategory in searchCategories)
            {
                if (searchCategory.ContainsCategory(invItem.ItemProfile.category)) return true;
            }

            return false;
        }

        #endregion
    }
}