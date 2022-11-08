using System;
using UnityEngine;

namespace KoalaDev.UGIS.Items
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

        #endregion
    }

    [Serializable]
    public abstract class ContainerItemData : AdditionalItemData
    {
        private InventoryGrid containerGrid;
    }

}