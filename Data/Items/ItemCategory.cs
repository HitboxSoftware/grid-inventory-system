using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Inventory.Categories
{
    [CreateAssetMenu(fileName = "New Item Category", menuName = "Hitbox/Inventory/Item Category")]
    public class ItemCategory : ScriptableObject
    {
        #region --- VARIABLES ---

        /// <summary>
        /// Description of the item category
        /// </summary>
        public string description;

        /// <summary>
        /// Icon for the category
        /// </summary>
        public Sprite icon;

        /// <summary>
        /// Any categories that are subordinate to the parent category.
        /// </summary>
        public ItemCategory[] subCategories;

        #endregion

        #region --- METHODS ---

        /// <summary>
        /// Checks if the sub-categories contains the given target category.
        /// </summary>
        /// <param name="target">Target category to find</param>
        /// <param name="searchedCategories">Categories already searched, prevents looping</param>
        /// <returns>true if category is found</returns>
        public bool ContainsCategory(ItemCategory target, List<ItemCategory> searchedCategories = default)
        {
            if (target == null) return false;
            if (target == this) return true;

            // Creating new list if searched categories is null.
            searchedCategories ??= new List<ItemCategory>();

            // Return false if this category has already been searched
            if (searchedCategories.Contains(this))
            {
                return false;
            }
            
            // Adding this category to searched categories.
            searchedCategories.Add(this);

            foreach (ItemCategory subCategory in subCategories)
            {
                if (subCategory.ContainsCategory(target, searchedCategories)) return true;
            }

            return false;
        }

        #endregion
    }

}