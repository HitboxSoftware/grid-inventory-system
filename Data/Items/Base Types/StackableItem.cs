namespace Hitbox.Inventory.Items
{
    public abstract class StackableItem : Item
    {
        /*
         * This sub-class makes an item stackable, this is kept separate from Item due to conflicts with Container and
         * Durability. Currently thinking about making a ComplexStackable item that supports Containers and Durability
         * but this is not coming soon. I'm sure you can figure it out, but i'd probably just use a List to represent
         * a stack of items, all of which can be durable or containers without issue.
         */
        #region --- VARIABLES ---

        public int stackCapacity;

        #endregion

        #region --- METHODS ---
        
        public override InventoryItem CreateItem => new InventoryStackableItem(this);

        public override (bool, bool) TryCombineItems(InventoryItem itemToCombine, InventoryItem combineRecipient)
        {
            // Return if either item doesn't exist
            if (itemToCombine == null || combineRecipient == null) return (false, false);
            // Return if items aren't the same
            if (itemToCombine.item != combineRecipient.item) return (false, false);
            // Return if the target is not stackable
            if (itemToCombine is not InventoryStackableItem combineStackable) return (false, false);
            if (combineRecipient is not InventoryStackableItem targetStackable) return (false, false);
            // Return if the profile is not stackable (should always be stackable, but we need the profile anyways)
            if (itemToCombine.item is not StackableItem stackableProfile) return (false, false);
            
            // If no space, return.
            if (combineStackable.currentStackAmount >= stackableProfile.stackCapacity) return (false, false);
            //TODO: This isn't making sure the added amount won't take it above the stack capacity.
            
            // Incrementing Stack Count
            combineStackable.currentStackAmount += targetStackable.currentStackAmount;

            targetStackable.currentStackAmount = 0; // Setting stack count to 0, we've now added it to the other stack.
            
            // Item Stacked Correctly
            return (true, false);
        }

        #endregion
    }
}