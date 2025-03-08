using Hitbox.Stash.Categories;
using UnityEngine;

namespace Hitbox.Stash
{
    [CreateAssetMenu(fileName = "New Basic Item", menuName = "Hitbox/Inventory/Items/Basic")]
    public class ItemProfile : ScriptableObject
    {
        #region Fields
        
        [Header("Item Properties")]
        public Vector2Int size = Vector2Int.one;
        public ushort id;
        public Sprite icon;
        public string description;
        public GameObject worldObject;
        public ItemCategory category;
        [HideInInspector] public ItemDatabase parentDatabase; // This should always be set by the database upon insertion.
        
        [Header("Icon Generation")]
        public Vector3 modelIconAngle;

        #endregion

        #region Methods
        
        /// <summary>
        /// Creates a new runtime inventory item, override this when implementing custom item logic.
        /// </summary>
        public virtual InventoryItem CreateItem() => new (this);
        
        /// <summary>
        /// Tries to create a new game object from the given InventoryItem.
        /// </summary>
        /// <returns></returns>
        public virtual bool TryCreateItemObject(out GameObject itemObj, InventoryItem invItem, Transform parent = null)
        {
            itemObj = Instantiate(worldObject, parent);
            return true;
        }

        #endregion
    }
}