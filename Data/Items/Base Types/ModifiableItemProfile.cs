using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash.Categories;
using UnityEngine;

namespace Hitbox.Stash.Items
{
    [CreateAssetMenu(fileName = "New Modifiable Item", menuName = "Hitbox/Inventory/Items/Modifiable")]
    public class ModifiableItemProfile : ItemProfile
    {
        [Header("Modification Properties")] 
        public List<ItemModificationNode> nodes = new();
    }

    [System.Serializable]
    public struct ItemModificationNode
    {
        public Vector3 position; // Position of the node in local space
        
        // Item Filters (Should be prioritized over category filters!)
        public List<ItemProfile> itemBlacklist;
        public List<ItemProfile> itemWhitelist;
        
        // Category Filters
        public List<ItemCategory> categoryWhitelist;
        public List<ItemCategory> categoryBlacklist;
        
        // WHITELISTING A CATEGORY ALSO WHITELISTS ALL RELATED SUBCATEGORIES, UNLESS THAT SUBCATEGORY IS BLACKLISTED!!!
        // ITEM FILTERS TAKE PRIORITY, EVEN IF THE ITEM'S CATEGORY ISN'T PERMITTED
    }
}