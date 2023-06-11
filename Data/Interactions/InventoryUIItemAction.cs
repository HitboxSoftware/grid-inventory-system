using System;
using System.Linq;
using Hitbox.Inventory;
using UnityEngine;

namespace Hitbox.Inventory.UI.Actions
{
    [CreateAssetMenu(fileName = "New Item Action", menuName = "Hitbox/Inventory/Actions/Simple")]

    public class InventoryUIItemAction : ScriptableObject
    {
        #region --- VARIABLES ---

        public Sprite icon;

        [Tooltip("Items in this list are excluded.")]
        public Item[] blacklist = Array.Empty<Item>();

        [Tooltip("Leave empty to accept every item.")]
        public Item[] whitelist = Array.Empty<Item>();

        /// <summary>
        /// Called whenever the action is invoked on an item.
        /// </summary>
        public event Action<InventoryItem> Interact;

        #endregion

        #region --- METHODS ---

        /// <summary>
        /// Check whether the given inventory item is supported by this action.
        /// </summary>
        /// <param name="invItem">Inventory item to check.</param>
        public virtual bool SupportsAction(InventoryItem invItem)
        {
            if (blacklist.Contains(invItem.item)) return false;
            if (whitelist.Length > 0 && !whitelist.Contains(invItem.item)) return false;

            // By default, always return true.
            return true;
        }

        /// <summary>
        /// Invoke the action on the given inventory item, listen to "Interact" event to use this.
        /// </summary>
        /// <param name="invItem">Inventory item to take action on</param>
        /// <returns>true if action was successful.</returns>
        public virtual bool Invoke(InventoryItem invItem)
        {
            if (!SupportsAction(invItem)) return false;

            Interact?.Invoke(invItem);
            return true;
        }

        #endregion

    }

}