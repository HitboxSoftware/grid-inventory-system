using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Stash
{
    public abstract class InventoryContainer
    {
        #region Fields
        
        /// <summary>
        /// All items within the container
        /// </summary>
        public abstract InventoryItem[] Items { get; }
        
        /// <summary>
        /// Invoked whenever an item is removed, added or changed within the container.
        /// </summary>
        public event System.Action<InventoryItem> OnUpdated;

        #endregion

        #region Methods

        /// <summary>
        /// Try and insert the item into the container.
        /// </summary>
        /// <param name="invItem">Item to Insert</param>
        /// <param name="combineItems">should it try and combine with other items during search</param>
        /// <returns>Whether item was added successfully</returns>
        public abstract bool InsertItem(InventoryItem invItem, bool combineItems = false);

        public abstract bool InsertItemAtIndex(InventoryItem invItem, int index, bool combineItems = false);

        /// <summary>
        /// Removes the given item from the container.
        /// </summary>
        /// <param name="invItem">target item to remove from the container</param>
        /// <param name="clearItem">should item have its container data cleared, generally leave this.</param>
        /// <returns>true if the item was successfully removed</returns>
        public abstract bool RemoveItem(InventoryItem invItem, bool clearItem = true);
        
        //TODO: ContainsItem, TryGetItemAtIndex
        
        protected void OnUpdate(InventoryItem invItem)
        {
            OnUpdated?.Invoke(invItem);
        }
        
        #endregion
    }

}