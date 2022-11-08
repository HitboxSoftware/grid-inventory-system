using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KoalaDev.UGIS
{
    public class InventoryGrid : MonoBehaviour
    {
        #region --- VARIABLES ---

        public Vector2Int size = Vector2Int.one;
        private readonly Dictionary<Vector2Int, InventoryItem> items = new ();

        #endregion
        
        #region --- METHODS ---
        
        public InventoryItem[] GetAllItems()
        {
            return new HashSet<InventoryItem>(items.Values).ToArray();
        }

        #region - ADDING ITEMS -

        // Add item to specified position, if not obstructed.
        public bool AddItemAtPosition(InventoryItem invItem, Vector2Int position)
        {
            List<Vector2Int> itemSlots = new ();

            RemoveItem(invItem); // Removing Item from this Grid, if it's inside it.

            for (int y = position.y; y < position.y + invItem.Item.size.y; y++)
            {
                for (int x = position.x; x < position.x + invItem.Item.size.x; x++)
                {
                    Vector2Int newPos = new (x, y);

                    if (items.ContainsKey(newPos) || newPos.x >= size.x || newPos.y >= size.y || newPos.x < 0 || newPos.y < 0) return false;

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
            // Go through every slot, except those where the item couldn't fit anyway.
            for (int y = 0; y < size.y - (invItem.Item.size.y - 1); y++)
            {
                for (int x = 0; x < size.x - (invItem.Item.size.x - 1); x++)
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
    }

}