using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Stash
{
    public class InventoryItem
    {
        #region Fields

        /// <summary>Information about the grid element such as name and size.</summary>
        public readonly ItemProfile ItemProfile;

        /// <summary>
        /// Returns the size of this element, can be overriden to add additional behaviour (i.e. rotation).
        /// </summary>
        public virtual Vector2Int Size => Rotated
            ? new Vector2Int(ItemProfile.size.y, ItemProfile.size.x)
            : ItemProfile.size;

        /// <summary>
        /// Parent container of the inventory item, can be null.
        /// </summary>
        public InventoryContainer ParentContainer { get; protected set; }

        public int Index { get; protected set; }

        /// <summary>
        /// Is the item rotated? (90 degrees)
        /// </summary>
        public bool Rotated;

        //TODO: When an item is locked, it should be undroppable and immovable!
        public bool Locked;

        /// <summary>
        /// Called whenever the item is updated, like 
        /// </summary>
        public event System.Action Updated;

        #endregion

        #region Methods
        
        #region Properties

        public virtual List<ItemProperty> GetProperties()
        {
            ItemProperty sizeProp = new ItemProperty(ItemProfile.icon, "Size of the Item", $"{ItemProfile.size.x}x{ItemProfile.size.y}");
            return new List<ItemProperty> { sizeProp };
        }

        #endregion
        
        #region Grid

        /// <summary>
        /// Checks if the element is contained in the given grid.
        /// </summary>
        public virtual bool InContainer(InventoryContainer container) => ParentContainer == container;

        /// <summary>
        /// Set the element's parent to the given grid.
        /// </summary>
        public virtual void SetContainer(InventoryContainer container, int index)
        {
            ParentContainer = container;
            Index = index;
            OnUpdate();
        }

        /// <summary>
        /// Remove the item from the grid and clear all position data.
        /// </summary>
        public virtual void RemoveFromContainer()
        {
            if (ParentContainer == null) return;
            ParentContainer.RemoveItem(this, false);
            ParentContainer = null;
            Index = -1;
            OnUpdate();
        }
        

        #endregion
        
        #region Data

        public virtual InventoryItemData GetItemData()
        {
            return new InventoryItemData(this);
        }

        #endregion

        public void OnUpdate()
        {
            Updated?.Invoke();
        }
        
        /// <summary>
        /// Attempts to combine combinableItem, allowing for logic such as element stacking.
        /// </summary>
        /// <param name="combinableItem">The item being combined.</param>
        /// <returns>true when combinableItem has been dealt with, false if it still needs to be handled (i.e. returned to original position).</returns>
        public virtual bool TryCombineItem(InventoryItem combinableItem) { return false; }
        
        /// <summary>
        /// Checks if combinableItem can be combined.
        /// </summary>
        /// <param name="combinableItem">The item being combined.</param>
        /// <returns>true if combinable, false otherwise.</returns>
        public virtual bool CanCombineItem(InventoryItem combinableItem) { return false; }

        #endregion

        #region Constructors

        public InventoryItem(ItemProfile itemProfile, bool rotated = false)
        {
            this.ItemProfile = itemProfile;
            this.Rotated = rotated;
        }

        public InventoryItem(ItemProfile itemProfile, InventoryGrid parentContainer, bool rotated = false)
        {
            this.ItemProfile = itemProfile;
            this.ParentContainer = parentContainer;
            this.Rotated = false;
        }

        #endregion
        
    }

    public class InventoryItemData
    {
        #region Fields

        public int index;
        public ushort id;
        public string dbName;
        public bool rotated;

        #endregion

        #region Methods

        public virtual InventoryItem Load()
        {
            ItemProfile profile = DatabaseManager.Instance.GetItemDatabase(dbName).items[id];

            // Creating new item from profile.
            InventoryItem item = new InventoryItem(profile, rotated);

            return item;
        }

        #endregion

        #region Constructors

        public InventoryItemData(InventoryItem invItem)
        {
            index = invItem.Index;
            id = invItem.ItemProfile.id;
            rotated = invItem.Rotated;
            
            if (invItem.ItemProfile.parentDatabase != null)
            {
                dbName = invItem.ItemProfile.parentDatabase.name;
            }
        }

        public InventoryItemData(int index, ushort id, string dbName, bool rotated)
        {
            this.index = index;
            this.id = id;
            this.dbName = dbName;
            this.rotated = rotated;
        }

        public InventoryItemData()
        {
        }

        #endregion

        #region Conversions

        public static implicit operator InventoryItemData(InventoryItem invItem) => invItem.GetItemData();

        #endregion
    }
}