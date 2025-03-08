using System;
using System.Linq;
using Hitbox.Stash;
using UnityEngine;

namespace Hitbox.Stash.UI.Actions
{
    [CreateAssetMenu(fileName = "New Item Action", menuName = "Hitbox/Inventory/Actions/Simple")]

    public class InventoryUIItemAction : ScriptableObject
    {
        #region Fields

        public Sprite icon;

        [Tooltip("Items in this list are excluded.")]
        public ItemProfile[] blacklist = Array.Empty<ItemProfile>();

        [Tooltip("Leave empty to accept every item.")]
        public ItemProfile[] whitelist = Array.Empty<ItemProfile>();

        /// <summary>
        /// Called whenever the action is invoked on an item.
        /// </summary>
        public event Action<InventoryItem> Interact;

        #endregion

        #region Methods

        /// <summary>
        /// Check whether the given inventory item is supported by this action.
        /// </summary>
        /// <param name="invItem">Inventory item to check.</param>
        public virtual bool IsCompatible(InventoryItem invItem)
        {
            if (blacklist.Contains(invItem.ItemProfile)) return false;
            if (whitelist.Length > 0 && !whitelist.Contains(invItem.ItemProfile)) return false;

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
            if (!IsCompatible(invItem)) return false;

            Interact?.Invoke(invItem);
            return true;
        }

        #endregion

    }

}