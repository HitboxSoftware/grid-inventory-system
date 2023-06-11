using Hitbox.Inventory.Categories;
using Hitbox.Utilities.Data;
using UnityEngine;

namespace Hitbox.Inventory
{
    public class Item : ScriptableObject
    {
        #region --- VARIABLES ---
        
        [Header("Item Properties")]
        public Vector2Int size = Vector2Int.one;
        public ushort id;
        public Sprite icon;
        public string description;
        public GameObject worldObject;
        public ItemCategory category;
        [HideInInspector] public ItemDatabase parentDatabase; // This should always be set by the database upon insertion.

        /// <summary>
        /// Invoked whenever an item has been updated.
        /// </summary>
        public event System.Action Updated;

        #endregion

        #region --- METHODS ---
        
        /// <summary>
        /// Creates a new runtime data object, override this when implementing custom runtime data.
        /// </summary>
        public virtual InventoryItem CreateItem => new (this);

        /// <summary>
        /// Attempts to combine two items together allowing for logic such as element stacking.
        /// </summary>
        /// <param name="itemToCombine">The item being combined</param>
        /// <param name="combineRecipient">The target of the combination</param>
        /// <returns>(bool, bool) where true is combined</returns>
        public virtual (bool, bool) TryCombineItems(InventoryItem itemToCombine, InventoryItem combineRecipient) { return (false, false); }

        /// <summary>
        /// This is called whenever an item has been updated so that the frontend can be notified.
        /// </summary>
        public void OnUpdate()
        {
            Updated?.Invoke();
        }

        #endregion
    }
}