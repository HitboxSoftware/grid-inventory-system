using UnityEngine;

namespace Hitbox.Inventory.Items
{
    public class InventoryStackableItem : InventoryItem
    {
        #region --- VARIABLES ---

        public int currentStackAmount = 1;

        #endregion

        #region --- METHODS ---
        
        //TODO: Add and Remove from stack methods.

        #endregion
        
        #region --- CONSTRUCTORS ---

        public InventoryStackableItem(StackableItem item, bool rotated = false) : base(item, rotated)
        {
            
        }

        public InventoryStackableItem(StackableItem item, InventoryGrid parentGrid, Vector2Int[] takenPositions = null, bool rotated = false) : base(item, parentGrid, takenPositions, rotated)
        {
            
        }
        
        #endregion

    }

    public class InventoryStackableItemData : InventoryItemData
    {
        public int currentStackAmount;
        
        protected InventoryStackableItemData(InventoryStackableItem invItem) : base(invItem)
        {
            currentStackAmount = invItem.currentStackAmount;
        }
        public InventoryStackableItemData()
        {
            
        }

        public static implicit operator InventoryStackableItemData(InventoryStackableItem item) => new(item);
    }
}