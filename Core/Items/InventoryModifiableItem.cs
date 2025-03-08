using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash;
using UnityEngine;

public class InventoryModifiableItem : InventoryItem
{
    
    
    public InventoryModifiableItem(ItemProfile itemProfile, bool rotated = false) : base(itemProfile, rotated)
    {
        
    }

    public InventoryModifiableItem(ItemProfile itemProfile, InventoryGrid parentContainer, bool rotated = false) : base(itemProfile, parentContainer, rotated)
    {
        
    }
}
