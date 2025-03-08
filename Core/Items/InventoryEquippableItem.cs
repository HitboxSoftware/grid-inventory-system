using Hitbox.Stash;
using UnityEngine;

public class InventoryEquippableItem : InventoryItem
{
    public InventoryEquippableItem(ItemProfile itemProfile, bool rotated = false) : base(itemProfile, rotated)
    {
        
    }

    public InventoryEquippableItem(ItemProfile itemProfile, InventoryGrid parentContainer, bool rotated = false) : base(itemProfile, parentContainer, rotated)
    {
        
    }
}
