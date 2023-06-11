using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Inventory
{
    public abstract class InventoryContainer
    {
        #region --- VARIABLES ---

        /// <summary>
        /// Invoked whenever an item is removed, added or changed within the container.
        /// </summary>
        public event System.Action<InventoryItem> Updated;

        #endregion

        #region --- METHODS ---

        /// <summary>
        /// Try and insert the item into the container.
        /// </summary>
        /// <param name="invItem">Item to Insert</param>
        /// <param name="combineItems">should it try and combine with other items during search</param>
        /// <returns>Whether item was added successfully</returns>
        public abstract bool InsertItem(InventoryItem invItem, bool combineItems = false);

        /// <summary>
        /// Insert an item at a given position.
        /// </summary>
        /// <param name="invItem">item to insert</param>
        /// <param name="position">position on the grid to insert the element</param>
        /// <param name="combineItems">whether items should be combined if cell at position is not empty.</param>
        public abstract bool InsertItemAtPosition(InventoryItem invItem, Vector2Int position, bool combineItems = true);
        
        /// <summary>
        /// Removes the given item from the container.
        /// </summary>
        /// <param name="invItem">target item to remove from the container</param>
        /// <param name="clearItem">should item have it's container data cleared, generally leave this.</param>
        /// <returns>true if the item was successfully removed</returns>
        public abstract bool RemoveItem(InventoryItem invItem, bool clearItem = true);
        
        protected virtual void OnUpdate(InventoryItem invItem)
        {
            Updated?.Invoke(invItem);
        }
        
        #endregion
    }

}