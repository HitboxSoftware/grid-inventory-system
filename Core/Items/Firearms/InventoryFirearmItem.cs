using System.Collections.Generic;
using Hitbox.Firearms;
using Hitbox.Inventory.Items;
using UnityEngine;

namespace Hitbox.Inventory.Inventories
{
    public class InventoryFirearmItem : InventoryItem
    {
        #region --- VARIABLES ---

        public Firearm Firearm { get; }
        private FirearmComponentItemSlot[] _firearmNodes;
        
        #endregion

        #region --- METHODS ---
        
        
        
        #endregion

        #region --- CONSTRUCTORS ---

        public InventoryFirearmItem(FirearmItem item, bool rotated = false) : base(item, rotated)
        {
            Firearm = new Firearm(item.firearmProfile);
            
            // Generating Slots for each node on the component
            List<FirearmComponentItemSlot> nodeSlots = new List<FirearmComponentItemSlot>();
            foreach (FirearmComponentNode node in Firearm.nodes)
            {
                nodeSlots.Add(new FirearmComponentItemSlot(node.requiredTag));
            }
            
            _firearmNodes = nodeSlots.ToArray();
            
            //TODO: Get base nodes of weapon and add to _firearmNodes;
        }

        public InventoryFirearmItem(FirearmItem item, InventoryGrid parentGrid, Vector2Int[] takenPositions = null, bool rotated = false) : base(item, parentGrid, takenPositions, rotated)
        {
            Firearm = new Firearm(item.firearmProfile);
            
            // Generating Slots for each node on the component
            List<FirearmComponentItemSlot> nodeSlots = new List<FirearmComponentItemSlot>();
            foreach (FirearmComponentNode node in Firearm.nodes)
            {
                nodeSlots.Add(new FirearmComponentItemSlot(node.requiredTag));
            }
            
            Debug.Log(nodeSlots.Count);

            _firearmNodes = nodeSlots.ToArray();
            
            //TODO: Get base nodes of weapon and add to _firearmNodes;
        }

        #endregion
    }

    public class InventoryFirearmItemData : InventoryItemData
    {
        
    }
}