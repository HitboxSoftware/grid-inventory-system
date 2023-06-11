namespace Hitbox.Inventory
{
    public abstract class Inventory
    {
        #region --- METHODS ---

        /// <summary>
        /// Insert an item into the inventory.
        /// </summary>
        /// <param name="invItem">Item to insert into the inventory</param>
        /// <returns>true if insertion was successful</returns>
        public abstract bool InsertItem(InventoryItem invItem);
        
        /// <summary>
        /// Checks if the given inventory item is contained in the inventory.
        /// </summary>
        /// <param name="invItem">Inventory item to find in the inventory</param>
        /// <returns>true if inventory contains invItem</returns>
        public abstract bool ContainsItem(InventoryItem invItem);
        
        /// <summary>
        /// Checks if the given item is contained in the inventory.
        /// </summary>
        /// <param name="item">Item to find in the inventory</param>
        /// <returns>true if inventory contains item</returns>
        public abstract bool ContainsItem(Item item);

        /// <summary>
        /// Attempts to retrieve an inventory item from the inventory with
        /// the given profile.
        /// </summary>
        /// <param name="item">Item profile to search for</param>
        /// <returns>InventoryItem with the given item profile</returns>
        public abstract InventoryItem GetItem(Item item);

        /// <summary>
        /// Attempt to remove the given inventory item from the inventory.
        /// </summary>
        /// <param name="invItem">The inventory item to remove</param>
        /// <returns>true if successful</returns>
        public abstract bool RemoveItem(InventoryItem invItem);
        
        #endregion
    }

    // NOTE: Used for Serialisation
    public abstract class InventoryData
    {
        
    }

}