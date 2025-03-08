using UnityEngine;

namespace Hitbox.Stash.Items
{
    [CreateAssetMenu(fileName = "New Stackable Item", menuName = "Hitbox/Inventory/Items/Stackable")]
    public class StackableItemProfile : ItemProfile
    {
        /*
         * This sub-class makes an item stackable, this is kept separate from Item due to conflicts with Container and
         * Durability. Currently thinking about making a ComplexStackable item that supports Containers and Durability
         * but this is not coming soon. I'm sure you can figure it out, but I'd probably just use a List to represent
         * a stack of items, all of which can be durable or containers without issue.
         */
        
        #region Fields

        public int stackCapacity;

        #endregion

        #region Methods
        
        public override InventoryItem CreateItem() => new InventoryStackableItem(this);

        #endregion
    }
}