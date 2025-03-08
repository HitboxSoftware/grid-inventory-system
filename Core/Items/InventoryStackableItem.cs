using UnityEngine;

namespace Hitbox.Stash.Items
{
    public class InventoryStackableItem : InventoryItem
    {
        #region Fields

        public int CurrentStackAmount = 1;

        #endregion

        #region Methods
        
        //TODO: Add and Remove from stack methods.
        
        public override bool TryCombineItem(InventoryItem combinableItem)
        {
            // Ensure items can be combined.
            if (!CanCombineItem(combinableItem)) return false;

            // Getting item references.
            InventoryStackableItem stackableCombinableItem = combinableItem as InventoryStackableItem;
            StackableItemProfile stackableProfile = ItemProfile as StackableItemProfile;
            
            // Incrementing Stack Count
            stackableCombinableItem.CurrentStackAmount -= Mathf.Clamp(stackableProfile.stackCapacity - CurrentStackAmount, 0, stackableCombinableItem.CurrentStackAmount);
            
            OnUpdate();
            
            // Item Stacked Correctly
            return stackableCombinableItem.CurrentStackAmount == 0;
        }

        public override bool CanCombineItem(InventoryItem combinableItem)
        {
            if(!base.CanCombineItem(combinableItem)) return false;

            if (CurrentStackAmount == ((StackableItemProfile)ItemProfile).stackCapacity) return false;
            
            // Return if either item doesn't exist
            if (combinableItem == null) return false;
            
            // Return if items aren't the same
            if (combinableItem.ItemProfile != ItemProfile) return false;
            
            // Return if either item is not stackable
            if (combinableItem is not InventoryStackableItem) return false;

            return true;
        }

        #endregion
        
        #region Constructors

        public InventoryStackableItem(StackableItemProfile itemProfile, bool rotated = false) : base(itemProfile, rotated)
        {
            
        }

        public InventoryStackableItem(StackableItemProfile itemProfile, InventoryGrid parentGrid, bool rotated = false) : base(itemProfile, parentGrid, rotated)
        {
            
        }
        
        #endregion

    }

    public class InventoryStackableItemData : InventoryItemData
    {
        public int CurrentStackAmount;
        
        protected InventoryStackableItemData(InventoryStackableItem invItem) : base(invItem)
        {
            CurrentStackAmount = invItem.CurrentStackAmount;
        }
        
        public InventoryStackableItemData()
        {
            
        }

        public static implicit operator InventoryStackableItemData(InventoryStackableItem item) => new(item);
    }
}