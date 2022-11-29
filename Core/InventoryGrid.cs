using System.Collections.Generic;
using System.Linq;
using KoalaDev.UGIS.Items;
using UnityEngine;

namespace KoalaDev.UGIS
{
    public class InventoryGrid
    {
        #region --- VARIABLES ---

        public Vector2Int Size;
        private readonly Dictionary<Vector2Int, InventoryItem> items = new ();
        public readonly List<InventoryItem> ItemBlacklist = new ();
        
        public IEnumerable<InventoryItem> AllItems => new HashSet<InventoryItem>(items.Values).ToArray();

        #endregion
        
        #region --- METHODS ---

        #region - ADDING ITEMS -

        // Add item to specified position, if not obstructed.
        public bool AddItemAtPosition(InventoryItem invItem, Vector2Int position, bool stackItems = true)
        {
            List<Vector2Int> itemSlots = new ();

            if (ItemBlacklist.Contains(invItem)) return false;

            RemoveItem(invItem); // Removing Item from this Grid, if it's inside it.

            for (int y = position.y; y < position.y + invItem.Item.size.y; y++)
            {
                for (int x = position.x; x < position.x + invItem.Item.size.x; x++)
                {
                    Vector2Int newPos = new (x, y);

                    if (newPos.x >= Size.x || newPos.y >= Size.y || newPos.x < 0 || newPos.y < 0) return false;

                    // Try to stack item if possible.
                    if (items.ContainsKey(newPos))
                    {
                        if(stackItems && items[newPos].Item == invItem.Item)
                            (items[newPos], invItem) = items[newPos].Item.ItemToItem(items[newPos], invItem);

                        if (invItem != null) return false;
                        
                        items[newPos].Item.Updated();
                        return true;
                    }
                    
                    itemSlots.Add(newPos);
                }
            }

            if (itemSlots.Count == 0) return false; 

            foreach (Vector2Int slot in itemSlots)
            {
                items.Add(slot, invItem);
            }
            
            invItem.SetGrid(this); // Parenting item to this grid.
            invItem.TakenSlots = itemSlots.ToArray();

            return true;
        }

        // Search through entire grid to find possible place, if none return false.
        public bool AutoAddItem(InventoryItem invItem)
        {
            if (ItemBlacklist.Contains(invItem)) return false;
            
            // Go through every slot, except those where the item couldn't fit anyway.
            for (int y = 0; y < Size.y - (invItem.Item.size.y - 1); y++)
            {
                for (int x = 0; x < Size.x - (invItem.Item.size.x - 1); x++)
                {
                    // Attempting to add item at new position, returns true if item added successfully.
                    if (AddItemAtPosition(invItem, new Vector2Int(x, y))) return true;
                }
            }
            
            // No place for item was found
            return false;
        }

        #endregion

        #region - REMOVING ITEMS -

        // Removes item at given position from grid if located, returns the item removed if found
        public InventoryItem RemoveItemAtPosition(Vector2Int position)
        {
            InventoryItem foundItem = items[position];

            if (foundItem == null) return null;
            
            // Removing all references to the item in this grid, but not from the item itself.
            foreach (Vector2Int slot in foundItem.TakenSlots)
            {
                items.Remove(slot);
            }

            return foundItem;
        }
        
        // Removes given item from grid if found, false if item not in grid.
        public bool RemoveItem(InventoryItem item)
        {
            if (!item.InGrid(this)) return false;

            if (item.TakenSlots == null) return false;
            
            // Removing all references to the item in this grid, but not from the item itself.
            foreach (Vector2Int slot in item.TakenSlots)
            {
                items.Remove(slot);
            }

            return true;
        }

        public bool ContainsItem(InventoryItem item) => items.ContainsValue(item);

        public bool ItemAtSlot(Vector2Int slot) => items.ContainsKey(slot);

        public InventoryItem GetItemAtSlot(Vector2Int slot) => items[slot];

        #endregion

        #endregion

        #region --- CONSTRUCTOR ---

        public InventoryGrid(Vector2Int size)
        {
            this.Size = size;
        }

        #endregion
    }

}