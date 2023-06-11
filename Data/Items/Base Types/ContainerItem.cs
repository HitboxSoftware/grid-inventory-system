using UnityEngine;

namespace Hitbox.Inventory.Items
{
    public abstract class ContainerItem : Item
    {
        // This sub-class of Item allows for items to have their own storage. Useful for clothing, or... Containers!
        
        #region --- VARIABLES ---
     
        [Header("Container Properties")]
        public Vector2Int[] gridSizes;

        public int[] rowCapacities;

        // List of items that aren't permitted inside the container.
        public Item[] itemBlacklist;
        
        // Makes itemBlacklist a whitelist, meaning only items in the list can be added.
        public bool blacklistIsWhitelist; 
        
        public override InventoryItem CreateItem => new InventoryContainerItem(this);

        #endregion

        #region --- METHODS ---

        public override (bool, bool) TryCombineItems(InventoryItem itemToCombine, InventoryItem combineRecipient)
        {
            // Return if either item doesn't exist
            if (itemToCombine == null || combineRecipient == null) return (false, false);
            // Return if the target is not container
            if (combineRecipient is not InventoryContainerItem containerInvItem) return (false, false);
            
            // Return if container grid doesn't exist
            if (containerInvItem.gridGroup == null) return (false, false);
            
            // Try and insert the item into one of the containers
            if (containerInvItem.gridGroup.InsertElement(itemToCombine)) return (true, false);

            return (false, false); 

        }

        #endregion
    }

}