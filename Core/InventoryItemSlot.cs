using Hitbox.Inventory.Categories;
using Newtonsoft.Json;
using UnityEngine;

namespace Hitbox.Inventory
{
    public class InventoryItemSlot : InventoryContainer
    {
        #region --- VARIABLES ---
        
        public InventoryItem AttachedItem { get; protected set; }
        public ItemCategory AcceptedCategory { get; protected set; }
        
        #endregion

        #region --- METHODS ---
        
        public override bool InsertItem(InventoryItem invItem, bool combineItems = false)
        {
            // Ensures item is not null.
            if (invItem?.item is null) return false;
            
            // Ensures slot is empty.
            if (AttachedItem is not null)
            {
                if(!combineItems) return false;

                (bool combined, _) = AttachedItem.item.TryCombineItems(invItem, AttachedItem);

                return combined;
            }

            
            Item item = invItem.item;
            // Ensures item's category is accepted, ignored if accepted category empty.
            if (AcceptedCategory != null && !AcceptedCategory.ContainsCategory(item.category)) return false;

            // Removing and clearing parent grid references.
            invItem.ParentContainer?.RemoveItem(invItem);

            // Attaching Item
            AttachedItem = invItem;
            AttachedItem.rotated = false;
            
            OnUpdate(AttachedItem);
            
            // Updating container of inventory item.
            invItem.SetContainer(this);

            return true;
        }
        
        public override bool InsertItemAtPosition(InventoryItem invItem, Vector2Int position, bool combineItems = true)
        {
            // Ignore position data.
            return InsertItem(invItem, combineItems);
        }

        public override bool RemoveItem(InventoryItem invItem, bool clearItem = true)
        {
            if (AttachedItem != invItem) return false;

            AttachedItem = null;
            OnUpdate(AttachedItem);

            return true;
        }

        /// <summary>
        /// Detach the current attached item and return it
        /// </summary>
        /// <returns>The attached item or null if no item was attached.</returns>
        public virtual InventoryItem DetachItem()
        {
            // Save the return item and remove it from the slot
            InventoryItem returnItem = AttachedItem;
            RemoveItem(AttachedItem);

            return returnItem;
        }

        public virtual void SetCategory(ItemCategory category)
        {
            AcceptedCategory = category;
        }
        
        public virtual bool HasItem()
        {
            return AttachedItem != null;
        }
        
        #endregion
        
        #region --- CONSTRUCTORS ---
        
        public InventoryItemSlot(ItemCategory acceptedCategory, InventoryItem attachedItem)
        {
            AttachedItem = attachedItem;
            AcceptedCategory = acceptedCategory;
        }
        
        public InventoryItemSlot(ItemCategory acceptedCategory)
        {
            AcceptedCategory = acceptedCategory;
        }

        public InventoryItemSlot()
        {
            
        }
        
        // This will only load the item, not the accepted category.
        private InventoryItemSlot(InventoryItemSlotData data)
        {
            if (data == null) return;
            if (data.attachedItem == null) return;

            InventoryItem newItem = data.attachedItem.Load();
            
            AttachedItem = newItem;
            newItem.SetContainer(this);
        }

        #endregion

        #region --- CONVERSIONS ---

        public static implicit operator InventoryItemSlot(InventoryItemSlotData data) => new (data);

        #endregion
    }

    public class InventoryItemSlotData
    {
        #region --- VARIABLES ---

        public InventoryItemData attachedItem;

        #endregion

        #region --- CONSTRUCTORS ---
        
        public InventoryItemSlotData(InventoryItemSlot slot)
        {
            if (slot.AttachedItem != null) attachedItem = slot.AttachedItem;
        }

        [JsonConstructor]
        public InventoryItemSlotData(InventoryItemData attachedItem)
        {
            this.attachedItem = attachedItem;
        }

        #endregion
        
        #region --- CONVERSIONS ---

        public static implicit operator InventoryItemSlotData(InventoryItemSlot itemSlot) => new (itemSlot);

        #endregion
    }

}