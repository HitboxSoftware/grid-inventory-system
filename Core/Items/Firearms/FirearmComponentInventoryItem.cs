using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.Items;
using UnityEngine;

public class FirearmComponentInventoryItem : InventoryItem
{
    #region --- VARIABLES ---
    
    private FirearmComponentItemSlot[] _childSlots;
    public FirearmComponent FirearmComponent { get; }

    #endregion

    #region --- METHODS ---
    
    

    #endregion

    #region --- CONSTRUCTORS ---

    public FirearmComponentInventoryItem(FirearmComponentItem item, bool rotated = false) : base(item, rotated)
    {
        FirearmComponent = item.componentProfile.CreateComponent();

        // Generating Slots for each node on the component
        List<FirearmComponentItemSlot> nodeSlots = new List<FirearmComponentItemSlot>();
        foreach (FirearmComponentNode node in FirearmComponent.nodes)
        {
            nodeSlots.Add(new FirearmComponentItemSlot(node.requiredTag));
        }
        
        _childSlots = nodeSlots.ToArray();
    }

    public FirearmComponentInventoryItem(FirearmComponentItem item, InventoryGrid parentContainer, Vector2Int[] takenPositions = null, bool rotated = false) : base(item, parentContainer, takenPositions, rotated)
    {
        FirearmComponent = item.componentProfile.CreateComponent();
        
        // Generating Slots for each node on the component
        List<FirearmComponentItemSlot> nodeSlots = new List<FirearmComponentItemSlot>();
        foreach (FirearmComponentNode node in FirearmComponent.nodes)
        {
            nodeSlots.Add(new FirearmComponentItemSlot(node.requiredTag));
        }

        _childSlots = nodeSlots.ToArray();
    }

    #endregion
}
