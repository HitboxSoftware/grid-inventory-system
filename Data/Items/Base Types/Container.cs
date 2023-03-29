using System;
using UnityEngine;

namespace Hitbox.UGIS.Items
{
    public abstract class Container : Item
    {
        // This sub-class of Item allows for items to have their own storage. Useful for clothing, or... Containers!
        
        #region --- VARIABLES ---
     
        [Header("Container Properties")]
        public Vector2Int itemStorageSize;

        // List of items that aren't permitted inside the container.
        public Item[] itemBlacklist;
        
        // Makes itemBlacklist a whitelist, meaning only items in the list can be added.
        public bool blacklistIsWhitelist; 
        
        public override ItemRuntimeData GetRuntimeData => new ContainerItemRuntimeData();

        #endregion

        #region --- METHODS ---

        public override (InventoryItem, InventoryItem) ResolveItemCombine(InventoryItem target, InventoryItem placedItem)
        {
            // Return if either item doesn't exist
            if (target == null || placedItem == null) return (target, placedItem);
            // Return if the target is not container
            if (target.Item is not Container containerItem) return (target, placedItem);
            
            // Getting Runtime Data
            ContainerItemRuntimeData containerData = (ContainerItemRuntimeData)containerItem.GetRuntimeData;
            
            // Return if runtime data doesn't exist
            if (containerData == null) return (target, placedItem);
            // Return if container grid doesn't exist
            if (containerData.containerGrid == null) return (target, placedItem);

            // Attempting to Add to Container
            if (containerData.containerGrid.AutoAddItem(placedItem))
            {
                
            }

            return (target, placedItem); 

        }

        #endregion
    }

    [Serializable]
    public class ContainerItemRuntimeData : ItemRuntimeData
    {
        public InventoryGrid containerGrid;
    }

}