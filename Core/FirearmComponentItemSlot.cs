using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.Categories;
using Hitbox.Inventory.Items;
using UnityEngine;

public class FirearmComponentItemSlot : InventoryItemSlot
{
    #region --- VARIABLES ---

    public FirearmComponentTag acceptedTag;

    #endregion
    
    #region --- METHODS ---

    public override bool InsertItem(InventoryItem invItem, bool combineItems = false)
    {
        // Ensures component is not null.
        if (invItem?.item is null) return false;
        
        // Ensures slot is empty.
        if (AttachedItem is not null)
        {
            if(!combineItems) return false;

            (bool combined, _) = AttachedItem.item.TryCombineItems(invItem, AttachedItem);

            return combined;
        }

        // Ensuring item has correct profile.
        if (invItem is not FirearmComponentInventoryItem componentInvItem) return false;
        if (componentInvItem.item is not FirearmComponentItem item) return false;

        // Ensures item's category is accepted.
        if (AcceptedCategory != null && !AcceptedCategory.ContainsCategory(item.category)) return false;
        if (componentInvItem.FirearmComponent.profile.tag != acceptedTag) return false;
        

        AttachedItem = componentInvItem;
        AttachedItem.rotated = false;

        OnUpdate(AttachedItem);

        return true;
    }

    #endregion

    #region --- CONSTRUCTORS ---

    public FirearmComponentItemSlot(FirearmComponentTag acceptedTag, ItemCategory acceptedCategory, FirearmComponentInventoryItem attachedItem) : base(acceptedCategory, attachedItem)
    {
        this.acceptedTag = acceptedTag;
    }

    public FirearmComponentItemSlot(FirearmComponentTag acceptedTag, ItemCategory acceptedCategory) : base(acceptedCategory)
    {
        this.acceptedTag = acceptedTag;
    }

    public FirearmComponentItemSlot(FirearmComponentTag acceptedTag) : base()
    {
        this.acceptedTag = acceptedTag;
    }

    #endregion
}
