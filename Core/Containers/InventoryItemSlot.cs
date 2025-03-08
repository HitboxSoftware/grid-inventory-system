using Hitbox.Stash.Categories;
using UnityEngine;

namespace Hitbox.Stash
{
    public class InventoryItemSlot : InventoryItemSlot<InventoryItem>
    {
        public InventoryItemSlot(ItemCategory acceptedCategory, InventoryItem attachedItem) : base(acceptedCategory, attachedItem)
        {
            
        }

        public InventoryItemSlot(ItemCategory acceptedCategory) : base(acceptedCategory)
        {
            
        }
    }
    
    public class InventoryItemSlot<T> : InventoryContainer where T : InventoryItem
    {
        #region Fields
        
        public T AttachedItem { get; protected set; }
        public ItemCategory AcceptedCategory { get; protected set; }
        
        // todo: determine if this is bad practice
        public new event System.Action<T> OnUpdated;

        public override InventoryItem[] Items =>
            AttachedItem == null ? System.Array.Empty<InventoryItem>() : new InventoryItem[] { AttachedItem };

        #endregion

        #region Methods
        
        public override bool InsertItem(InventoryItem invItem, bool combineItems = false)
        {
            // Ensures item is not null.
            if (invItem?.ItemProfile is null) return false;
            if (invItem is not T typedInvItem) return false;
            
            // Ensures slot is empty.
            if (AttachedItem is not null)
            {
                if(!combineItems) return false;

                bool combined = AttachedItem.TryCombineItem(typedInvItem);

                return combined;
            }

            
            ItemProfile itemProfile = typedInvItem.ItemProfile;
            // Ensures item's category is accepted, ignored if accepted category empty.
            if (AcceptedCategory != null && !AcceptedCategory.ContainsCategory(itemProfile.category)) return false;

            // Removing and clearing parent grid references.
            typedInvItem.ParentContainer?.RemoveItem(typedInvItem);

            // Attaching Item
            AttachedItem = typedInvItem;
            AttachedItem.Rotated = false;
            
            OnUpdate(AttachedItem);
            
            // Updating container of inventory item.
            typedInvItem.SetContainer(this, 0);

            return true;
        }
        
        public override bool InsertItemAtIndex(InventoryItem invItem, int index, bool combineItems = false)
        {
            // Ignore index.
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
        
        public virtual bool IsItemAttached()
        {
            return AttachedItem != null;
        }
        
        private void OnUpdate(T invItem)
        {
            OnUpdated?.Invoke(invItem);
        }
        
        #endregion
        
        #region Constructors
        
        public InventoryItemSlot(ItemCategory acceptedCategory, T attachedItem)
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
        
        public static InventoryItemSlot<T> FromData(InventoryItemSlotData<T> data)
        {
            var slot = new InventoryItemSlot<T>();
    
            if (data?.attachedItem != null)
            {
                InventoryItem item = data.attachedItem.Load();
                if (item is T typedItem)
                {
                    slot.AttachedItem = typedItem;
                    item.SetContainer(slot, 0);
                }
                else
                {
                    Debug.LogWarning($"Loaded item is not of type {typeof(T).Name}");
                }
            }
    
            return slot;
        }

        #endregion

        #region Conversions

        public static implicit operator InventoryItemSlot<T>(InventoryItemSlotData<T> data) => FromData(data);

        #endregion
    }

    public class InventoryItemSlotData<T> where T : InventoryItem
    {
        #region Fields

        public InventoryItemData attachedItem;

        #endregion

        #region Constructors
        
        public InventoryItemSlotData(InventoryItemSlot<T> slot)
        {
            if (slot.AttachedItem != null) attachedItem = slot.AttachedItem;
        }

        public InventoryItemSlotData(InventoryItemData attachedItem)
        {
            this.attachedItem = attachedItem;
        }

        #endregion
        
        #region Conversions

        public static implicit operator InventoryItemSlotData<T>(InventoryItemSlot<T> itemSlot) => new (itemSlot);

        #endregion
    }

}