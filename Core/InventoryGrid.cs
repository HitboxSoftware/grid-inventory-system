using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Hitbox.Inventory
{
    public class InventoryGrid : InventoryContainer
    {
        #region --- VARIABLES ---
        
        /// <summary>
        /// All of the items contained within the grid.
        /// </summary>
        private readonly Dictionary<Vector2Int, InventoryItem> _items = new ();
        
        /// <summary>
        /// Returns an array of all the items contained within the grid avoiding duplicates.
        /// </summary>
        public InventoryItem[] AllItems => new HashSet<InventoryItem>(_items.Values).ToArray(); // Hashset will remove duplicate items.

        public readonly List<InventoryItem> blacklist = new ();
        public bool blacklistIsWhitelist; // Treat the blacklist as a whitelist
        
        public Vector2Int size;

        #endregion

        #region --- GRID METHODS ---

        #region Manipulation

        #region Insertion
        
        // Search through entire grid to find possible place, if none return false.
        public override bool InsertItem(InventoryItem item, bool combineItems = false)
        {
            if (item == null) return false;

            // Blacklist / Whitelist Check
            if ((!blacklistIsWhitelist && blacklist.Contains(item)) ||
                (blacklistIsWhitelist && !blacklist.Contains(item)))
            { return false; }

            // Go through every slot, except those where the item couldn't fit anyway.
            for (int y = 0; y < size.y - (item.Size.y - 1); y++)
            {
                for (int x = 0; x < size.x - (item.Size.x - 1); x++)
                {
                    // Attempting to add item at new position, returns true if item added successfully.
                    if (InsertItemAtPosition(item, new Vector2Int(x, y), combineItems)) return true;
                }
            }
            
            // Rotate Item and try again!
            item.rotated = !item.rotated;
            
            // Go through every slot, except those where the item couldn't fit anyway.
            for (int y = 0; y < size.y - (item.Size.y - 1); y++)
            {
                for (int x = 0; x < size.x - (item.Size.x - 1); x++)
                {
                    // Attempting to add item at new position, returns true if item added successfully.
                    if (InsertItemAtPosition(item, new Vector2Int(x, y), combineItems)) return true;
                }
            }
            
            // Flip rotation back to original
            item.rotated = !item.rotated;
            
            // No place for item was found
            return false;
        }
        
        public override bool InsertItemAtPosition(InventoryItem invItem, Vector2Int position, bool combineItems = true)
        {
            RemoveItem(invItem, false); // Removing Element from this Grid, if it's inside it.
            invItem.RemoveFromGrid();
            
            // Checking if the item is within bounds.
            if (!CanInsertAtPosition(position, invItem, false)) // Checking the element 
            {
                return false;
            }

            List<Vector2Int> itemPositions = new ();
            for (int y = position.y; y < position.y + invItem.Size.y; y++)
            {
                for (int x = position.x; x < position.x + invItem.Size.x; x++)
                {
                    Vector2Int offsetPos = new (x, y);
                    
                    // Try to combine elements if possible.
                    if (combineItems && ItemAtPosition(offsetPos))
                    {
                        (bool combined, _) = _items[offsetPos].item.TryCombineItems(invItem, _items[offsetPos]);

                        if (!combined) return false;
                        
                        _items[offsetPos].item.OnUpdate();
                        return true;
                    }

                    if (ItemAtPosition(offsetPos)) return false;
                    
                    itemPositions.Add(offsetPos);
                }
            }

            if (itemPositions.Count == 0) return false; // Element was not inserted.

            foreach (Vector2Int slot in itemPositions)
            {
                _items.Add(slot, invItem);
            }
            
            invItem.SetContainer(this); // Parenting element to this grid.
            invItem.takenPositions = itemPositions.ToArray();

            OnUpdate(invItem);
            
            return true;
        }
        
        #endregion

        #region Interaction

        public bool TryRotateItem(InventoryItem invItem)
        {
            // Flip Rotation
            invItem.rotated = !invItem.rotated;
            
            // Check if rotation possible
            if (!CanAddToPosition(invItem.takenPositions[0], invItem))
            {
                // Un-flip Rotation, Rotation not possible.
                invItem.rotated = !invItem.rotated;
                return false;
            }

            Vector2Int origin = invItem.takenPositions[0];

            // Updating Item Position.
            RemoveItem(invItem);
            InsertItemAtPosition(invItem, origin);
            
            OnUpdate(invItem);
            
            return true;
        }

        #endregion

        #region Removal
        
        /// <summary>
        /// Removes and returns the item at the given position.
        /// </summary>
        /// <returns>InventoryItem removed or null if nothing found.</returns>
        public InventoryItem RemoveItemAtPosition(Vector2Int position)
        {
            InventoryItem foundItem = _items[position];

            if (foundItem == null) return null;
            
            RemoveItem(foundItem);

            return foundItem;
        }
        
        public override bool RemoveItem(InventoryItem invItem, bool clearItem = true)
        {
            if (!invItem.InGrid(this)) return false; // Stop if element not in this grid.
            if (invItem.takenPositions == null) return false; // Stop if element has no taken positions.
            
            // Removing all references to the item in this grid, but not the element itself.
            foreach (Vector2Int slot in invItem.takenPositions)
            {
                if (_items.ContainsKey(slot) && _items[slot] != invItem) continue;
                _items.Remove(slot);
            }
            
            OnUpdate(invItem);
            
            if(clearItem)
                invItem.RemoveFromGrid();
            
            return true;
        }

        /// <summary>
        /// Checks if the given item is located within the grid.<para/>This is search is of O(N) complexity.
        /// </summary>
        /// <param name="invItem">Search target</param>
        public virtual bool ContainsItem(InventoryItem invItem) => _items.ContainsValue(invItem);

        /// <summary>
        /// Checks if the given cell is taken by another element.
        /// </summary>
        /// <param name="position">The position in the grid to check.</param>
        /// <returns>true if element has been found at position.</returns>
        public virtual bool ItemAtPosition(Vector2Int position) => _items.ContainsKey(position);

        /// <summary>
        /// Retrieves the item located at the given position.
        /// </summary>
        /// <param name="position">position to get item from.</param>
        /// <returns>item at the given position, or null if not found.</returns>
        public virtual InventoryItem GetItemAtPosition(Vector2Int position) => _items.ContainsKey(position) ? _items[position] : null;

        #endregion

        #endregion

        #region Utilities

        /// <summary>
        /// Checks every cell within the given item's dimensions to find if it's possible to add to the given position.
        /// </summary>
        /// <param name="position">start position of the search.</param>
        /// <param name="invItem">used to get search dimensions.</param>
        /// <param name="itemCheck">Should an item taking the position return false</param>
        public virtual bool CanInsertAtPosition(Vector2Int position, InventoryItem invItem, bool itemCheck = true)
        {
            if (blacklistIsWhitelist)
            {
                if (!blacklist.Contains(invItem)) return false;
            } else if (blacklist.Contains(invItem)) 
                return false;
            
            return CanInsertAtPosition(position, invItem.Size, itemCheck);
        }

        /// <summary>
        /// Checks every cell within the given dimensions to find if it's possible to add to the given position.
        /// </summary>
        /// <param name="position">start position of the search.</param>
        /// <param name="dimensions">width and height of the area to check.</param>
        /// <param name="itemCheck">Should an element taking the position return false</param>
        public virtual bool CanInsertAtPosition(Vector2Int position, Vector2Int dimensions, bool itemCheck = true)
        {
            // Ensure Area is within world size.
            if (position.x + dimensions.x - 1 >= size.x || // Check X Boundary
                position.y + dimensions.y - 1 >= size.y || // Check Y Boundary
                position.x < 0 || position.y < 0) return false; // Check [< 0] XY Boundaries.
            
            for (int y = position.y; y < position.y + dimensions.y; y++)
            {
                for (int x = position.x; x < position.x + dimensions.x; x++)
                {
                    Vector2Int offsetPos = new (x, y);

                    if (!itemCheck) continue;
                    
                    if (ItemAtPosition(offsetPos)) return false;
                }
            }
            return true;
        }
        
        public bool CanAddToPosition(Vector2Int position, InventoryItem invItem)
        {
            for (int y = position.y; y < position.y + invItem.Size.y; y++)
            {
                for (int x = position.x; x < position.x + invItem.Size.x; x++)
                {
                    Vector2Int newPos = new (x, y);

                    if (_items.ContainsKey(newPos) && _items[newPos] != invItem) return false;
                    
                    if (newPos.x >= size.x || newPos.y >= size.y || newPos.x < 0 || newPos.y < 0) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Given the position and size, return an array of all the positions in that area.
        /// </summary>
        /// <param name="startPos">start position of the selection</param>
        /// <param name="dimensions">Height and Width of the area to select.</param>
        /// <returns></returns>
        public static Vector2Int[] SelectPositions(Vector2Int startPos, Vector2Int dimensions)
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int y = startPos.y; y < startPos.y + dimensions.y; y++)
            {
                for (int x = startPos.x; x < startPos.x + dimensions.x; x++)
                {
                    Vector2Int newPos = new (x, y);
                    positions.Add(newPos);
                }
            }

            return positions.ToArray();
        }

        #endregion

        #endregion

        #region --- CONSTRUCTOR ---

        public InventoryGrid(Vector2Int size)
        {
            this.size = size;
        }

        public InventoryGrid(InventoryGridData data)
        {
            size = new Vector2Int(data.sizeX, data.sizeY);
            
            for (int index = 0; index < data.items.Length; index++)
            {
                InventoryItemData elementData = data.items[index];

                InventoryItem newItem = elementData.Load();

                InsertItemAtPosition(newItem, new Vector2Int(elementData.x, elementData.y), false);
            }
        }

        #endregion

        #region --- CONVERTERS ---

        public static implicit operator InventoryGrid(InventoryGridData data) => new(data);

        #endregion
    }

    public class InventoryGridData
    {
        public InventoryItemData[] items;

        public int sizeX;
        public int sizeY;
        
        public InventoryItemData[] blacklist;
        public bool blacklistIsWhitelist;

        public InventoryGridData(InventoryGrid grid)
        {
            InventoryItem[] gridItems = grid.AllItems;
            InventoryItemData[] gridItemData = new InventoryItemData[gridItems.Length];
            for (int index = 0; index < gridItems.Length; index++)
            {
                gridItemData[index] = gridItems[index].GetItemData();
            }

            items = gridItemData;
            
            sizeX = grid.size.x;
            sizeY = grid.size.y;
            
            if (grid.blacklist != null)
            {
                InventoryItemData[] gridBlacklistData = new InventoryItemData[grid.blacklist.Count];
                for (int index = 0; index < grid.blacklist.Count; index++)
                {
                    gridBlacklistData[index] = grid.blacklist[index];
                }
                
                blacklist = gridBlacklistData;
            }

            blacklistIsWhitelist = grid.blacklistIsWhitelist;
        }

        [JsonConstructor]
        public InventoryGridData(InventoryItemData[] items, int sizeX, int sizeY, InventoryItemData[] blacklist, bool blacklistIsWhitelist)
        {
            this.items = items;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.blacklist = blacklist;
            this.blacklistIsWhitelist = blacklistIsWhitelist;
        }

        public static implicit operator InventoryGridData(InventoryGrid grid) => new(grid);
    }

}