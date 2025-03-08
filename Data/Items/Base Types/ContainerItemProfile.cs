using UnityEngine;

namespace Hitbox.Stash.Items
{
    [CreateAssetMenu(fileName = "New Container Item", menuName = "Hitbox/Inventory/Items/Container")]
    public class ContainerItemProfile : ItemProfile
    {
        // This sub-class of Item allows for items to have their own storage. Useful for clothing, or... Containers!
        
        #region Fields
     
        [Header("Container Properties")]
        public Vector2Int[] gridSizes;

        public int[] rowCapacities;

        // List of items that aren't permitted inside the container.
        public ItemProfile[] itemBlacklist;
        
        // Makes itemBlacklist a whitelist, meaning only items in the list can be added.
        public bool blacklistIsWhitelist; 
        
        #endregion

        #region Methods

        public override InventoryItem CreateItem() => new InventoryContainerItem(this);

        #endregion
    }

}