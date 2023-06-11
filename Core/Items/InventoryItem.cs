using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Hitbox.Inventory
{
    public class InventoryItem
    {
        #region --- VARIABLES ---

        /// <summary>Information about the grid element such as name and size.</summary>
        public readonly Item item;

        /// <summary>
        /// Returns the size of this element, can be overriden to add additional behaviour (i.e. rotation).
        /// </summary>
        public virtual Vector2Int Size => rotated
            ? new Vector2Int(item.size.y, item.size.x)
            : item.size;

        /// <summary>
        /// The positions in the grid that this element takes up.
        /// First index is used as the origin position for the element in the grid.
        /// </summary>
        public Vector2Int[] takenPositions = { Vector2Int.zero };

        public Vector2Int Position => takenPositions is { Length: > 0 } ? takenPositions[0] : -Vector2Int.one;

        public InventoryContainer ParentContainer { get; protected set; }

        public bool rotated;

        #endregion

        #region --- METHODS ---
        
        #region Properties

        public virtual List<ItemProperty> GetProperties()
        {
            ItemProperty sizeProp = new ItemProperty(item.icon, "Size of the Item", $"{item.size.x}x{item.size.y}");
            return new List<ItemProperty> { sizeProp };
        }

        #endregion
        
        #region Grid

        /// <summary>
        /// Checks if the element is contained in the given grid.
        /// </summary>
        public virtual bool InGrid(InventoryGrid grid) => ParentContainer == grid;

        /// <summary>
        /// Set the element's parent to the given grid.
        /// </summary>
        public virtual void SetContainer(InventoryContainer container) => ParentContainer = container;

        /// <summary>
        /// Remove the item from the grid and clear all position data.
        /// </summary>
        public virtual void RemoveFromGrid()
        {
            if (ParentContainer != null)
            {
                ParentContainer.RemoveItem(this, false);
                ParentContainer = null;
            }

            takenPositions = System.Array.Empty<Vector2Int>();
        }


        /// <summary>
        /// Checks if the element is contained in the given position.
        /// </summary>
        public virtual bool InPosition(Vector2Int position) => takenPositions.Contains(position);

        #endregion
        
        #region Data

        public virtual InventoryItemData GetItemData()
        {
            return new InventoryItemData(this);
        }

        #endregion

        #endregion

        #region --- CONSTRUCTORS ---

        public InventoryItem(Item item, bool rotated = false)
        {
            this.item = item;
            this.rotated = rotated;
        }

        public InventoryItem(Item item, InventoryGrid parentContainer, Vector2Int[] takenPositions = null,
            bool rotated = false)
        {
            this.item = item;
            this.takenPositions = takenPositions;
            this.ParentContainer = parentContainer;
            this.rotated = false;
        }

        #endregion

        #region --- CONVERSIONS ---

        //public static implicit operator InventoryItem(InventoryItemData data) => new(data);

        #endregion
    }

    public class InventoryItemData
    {
        #region --- VARIABLES ---

        public int x;
        public int y;
        public ushort id;
        public string dbName;
        public bool rotated;

        #endregion

        #region --- METHODS ---

        public virtual InventoryItem Load()
        {
            Item profile = DatabaseManager.Instance.GetItemDatabase(dbName).items[id];

            // Creating new item from profile.
            InventoryItem item = new InventoryItem(profile, rotated);

            // Setting position to saved position.
            item.takenPositions[0] = new Vector2Int(x, y);

            return item;
        }

        #endregion

        #region --- CONSTRUCTORS ---

        public InventoryItemData(InventoryItem invItem)
        {
            x = invItem.Position.x;
            y = invItem.Position.y;
            id = invItem.item.id;
            dbName = invItem.item.parentDatabase.name;
            rotated = invItem.rotated;
        }

        [JsonConstructor]
        public InventoryItemData(int x, int y, ushort id, string dbName, bool rotated)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            this.dbName = dbName;
            this.rotated = rotated;
        }

        public InventoryItemData()
        {
        }

        #endregion

        #region --- CONVERSIONS ---

        public static implicit operator InventoryItemData(InventoryItem invItem) => invItem.GetItemData();

        #endregion
    }

    public class InventoryItemStat
    {
        public Sprite icon;
        public string value;
        public string name;

        public InventoryItemStat(Sprite icon, string value, string name)
        {
            this.icon = icon;
            this.value = value;
            this.name = name;
        }
    }
}