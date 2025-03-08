using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Hitbox.Collections;
using Hitbox.Collections.Data;
using UnityEngine;

namespace Hitbox.Stash
{
    public class InventoryGrid : InventoryContainer
    {
        #region Fields

        /// <summary>
        /// Grid containing inventory items.
        /// </summary>
        protected readonly SizedSparseGrid<InventoryItem> Grid;

        public readonly List<object> Blacklist = new ();
        
        public Vector2Int Size;

        public bool Locked = false;

        /// <summary>
        /// All of the items contained within the grid.
        /// </summary>
        public override InventoryItem[] Items => Grid.GetAllData();

        #endregion

        #region Methods

        #region Manipulation

        #region Insertion
        
        // Search through entire grid to find possible place, if none return false.
        public override bool InsertItem(InventoryItem invItem, bool combineItems = false)
        {
            if (invItem == null || Locked) return false;

            // Blacklist / Whitelist Check
            if (Blacklist.Contains(invItem)) { return false; }

            // Go through every slot, except those where the item couldn't fit anyway.
            for (int y = 0; y < Size.y - (invItem.Size.y - 1); y++)
            {
                for (int x = 0; x < Size.x - (invItem.Size.x - 1); x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (InsertItemAtPosition(invItem, pos, combineItems)) return true;
                }
            }
            
            // Rotate Item and try again!
            invItem.Rotated = !invItem.Rotated;
            
            // Go through every slot, except those where the item couldn't fit anyway.
            for (int y = 0; y < Size.y - (invItem.Size.y - 1); y++)
            {
                for (int x = 0; x < Size.x - (invItem.Size.x - 1); x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (InsertItemAtPosition(invItem, pos, combineItems)) return true;
                }
            }
            
            // Flip rotation back to original
            invItem.Rotated = !invItem.Rotated;

            // No place for item was found
            return false;
        }
        
        public virtual bool InsertItemAtPosition(InventoryItem invItem, Vector2Int position, bool combineItems = true)
        {
            if (Locked) return false;
            
            RemoveItem(invItem, false); // Removing Element from this Grid, if it's inside it.
            invItem.RemoveFromContainer();
            
            // Checking if the item is within bounds.
            if (!CanInsertAtPosition(position, invItem, false)) // Checking the element 
            {
                return false;
            }

            if (Grid.InsertElement(invItem, position, invItem.Size))
            {
                invItem.SetContainer(this, Grid.PositionToIndex(position)); // Parenting element to this grid.
                OnUpdate(invItem);
                return true;
            }

            for (int y = position.y; y < position.y + invItem.Size.y; y++)
            {
                for (int x = position.x; x < position.x + invItem.Size.x; x++)
                {
                    Vector2Int offsetPos = new (x, y);
                    
                    // Try to combine elements if possible.
                    if (combineItems && ItemAtPosition(offsetPos))
                    {
                        bool combined = Grid[offsetPos].TryCombineItem(invItem);

                        if (!combined) return false;
                        
                        return true;
                    }

                    if (ItemAtPosition(offsetPos)) return false;
                }
            }

            return false;
        }
        
        public override bool InsertItemAtIndex(InventoryItem invItem, int index, bool combineItems = false)
        {
            if (Locked) return false;
            
            Vector2Int position = Grid.IndexToPosition(index);

            return InsertItemAtPosition(invItem, position, combineItems);
        }

        #endregion

        #region Removal
        
        /// <summary>
        /// Removes and returns the item at the given position.
        /// </summary>
        /// <returns>InventoryItem removed or null if nothing found.</returns>
        public InventoryItem RemoveItemAtPosition(Vector2Int position)
        {
            if (Locked) return null;
            
            InventoryItem foundItem = Grid[position];

            if (foundItem == null) return null;
            
            RemoveItem(foundItem);

            return foundItem;
        }
        
        public override bool RemoveItem(InventoryItem invItem, bool clearItem = true)
        {
            if (Locked || !invItem.InContainer(this)) return false; // Stop if element not in this grid.

            Grid.Remove(invItem.Index);
            
            OnUpdate(invItem);
            
            if(clearItem)
                invItem.RemoveFromContainer();
            
            return true;
        }

        /// <summary>
        /// Checks if the given item is located within the grid.<para/>This is search is of O(N) complexity.
        /// </summary>
        /// <param name="invItem">Search target</param>
        public virtual bool ContainsItem(InventoryItem invItem) => Grid.ContainsData(invItem);

        /// <summary>
        /// Checks if the given cell is taken by another element.
        /// </summary>
        /// <param name="position">The position in the grid to check.</param>
        /// <returns>true if element has been found at position.</returns>
        public virtual bool ItemAtPosition(Vector2Int position) => Grid.ElementAt(position);

        /// <summary>
        /// Retrieves the item located at the given position.
        /// </summary>
        /// <param name="position">position to get item from.</param>
        /// <param name="invItem">Inventory Item to return</param>
        /// <returns>True if item was found successfully</returns>
        public virtual bool TryGetItemAtPosition(Vector2Int position, out InventoryItem invItem)
        {
            return Grid.TryGetDataAtPosition(position, out invItem);
        }

        #endregion

        #endregion

        public void Load(InventoryGridData data)
        {
            var index = 0;
            for (; index < data.Items.Length; index++)
            {
                var elementData = data.Items[index];
                InventoryItem newItem = elementData.Load();

                Grid.InsertElement(newItem, elementData.index, newItem.Size);
            }
        }

        #region Utilities

        /// <summary>
        /// Checks every cell within the given item's dimensions to find if it's possible to add to the given position.
        /// </summary>
        /// <param name="position">start position of the search.</param>
        /// <param name="invItem">used to get search dimensions.</param>
        /// <param name="itemCheck">Should an item taking the position return false</param>
        public virtual bool CanInsertAtPosition(Vector2Int position, InventoryItem invItem, bool itemCheck = true)
        {
            if (Blacklist.Contains(invItem)) return false;
            
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
            // Ensure Area is within grid size.
            if (position.x + dimensions.x - 1 >= Size.x || // Check X Boundary
                position.y + dimensions.y - 1 >= Size.y || // Check Y Boundary
                position.x < 0 || position.y < 0) return false; // Check [< 0] XY Boundaries.

            if (!itemCheck) return true;

            if (Grid.ElementIn(new GridRect(position, dimensions))) return false;
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invItem"></param>
        /// <returns></returns>
        public virtual bool CanInsertInGrid(InventoryItem invItem)
        {
            for (int y = 0; y < Size.y; y++)
            {
                for (int x = 0; x < Size.x; x++)
                {
                    if (CanInsertAtPosition(new Vector2Int(x, y), invItem.Size)) return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Test if an item can be combined with an item at the given position.
        /// </summary>
        /// <param name="position">Position to retrieve item from</param>
        /// <param name="invItem">Item to be tested</param>
        /// <returns>True if the item can be combined at the given position</returns>
        public virtual bool CanCombineAtPosition(Vector2Int position, InventoryItem invItem)
        {
            return ItemAtPosition(position) && Grid[position].CanCombineItem(invItem);
        }

        /// <summary>
        /// Retrieves every InventoryItem in grid matching the target item.
        /// </summary>
        /// <param name="targetItemProfile">Target item to compare against.</param>
        /// <returns>List of InventoryItems matching target item.</returns>
        public List<InventoryItem> GetInventoryItemsFromItem(ItemProfile targetItemProfile)
        {
            return Grid.GetAllData().Where(inventoryItem => inventoryItem.ItemProfile == targetItemProfile).ToList();
        }

        /// <summary>
        /// Creates a dictionary containing every InventoryItem in the grid with the key being their
        /// linked item.
        /// </summary>
        /// <returns>Dictionary of every InventoryItem keyed to their "Item"</returns>
        public Dictionary<ItemProfile, List<InventoryItem>> GetItemDictionary()
        {
            Dictionary<ItemProfile, List<InventoryItem>> itemDict = new();

            foreach (InventoryItem invItem in Grid.GetAllData())
            {
                if (itemDict.TryGetValue(invItem.ItemProfile, out List<InventoryItem> value))
                {
                    value.Add(invItem);
                }
                else
                {
                    itemDict.Add(invItem.ItemProfile, new List<InventoryItem>{invItem});
                }
            }

            return itemDict;
        }

        public Vector2 GetItemCentre(InventoryItem item)
        {
            return Grid.GetElementAtIndex(item.Index).rect.centre;
        }

        public Vector2Int[] GetItemPositions(InventoryItem item)
        {
            (InventoryItem item, GridRect rect) element = Grid.GetElementAtIndex(item.Index);

            List<Vector2Int> positions = new List<Vector2Int>();
            for (int y = element.rect.yMin; y < element.rect.yMax; y++)
            {
                for (int x = element.rect.xMin; x < element.rect.xMax; x++)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }

            return positions.ToArray();
        }

        public Vector2Int IndexToPosition(int index)
        {
            return Grid.IndexToPosition(index);
        }
        
        public int PositionToIndex(Vector2Int pos)
        {
            return Grid.PositionToIndex(pos);
        }

        #endregion

        #endregion

        #region Constructors

        public InventoryGrid(Vector2Int size)
        {
            Size = size;
            Grid = new SizedSparseGrid<InventoryItem>(size);
        }

        #endregion
    }

    public class InventoryGridData
    {
        public InventoryItemData[] Items;
        
        public InventoryGridData(InventoryGrid grid)
        {
            InventoryItem[] gridItems = grid.Items;
            InventoryItemData[] gridItemData = new InventoryItemData[gridItems.Length];
            for (int index = 0; index < gridItems.Length; index++)
            {
                gridItemData[index] = gridItems[index].GetItemData();
            }

            if(gridItemData.Length > 0)
                Items = gridItemData;
        }
        
        public InventoryGridData(InventoryItemData[] items)
        {
            Items = items;
        }

        public static implicit operator InventoryGridData(InventoryGrid grid) => new(grid);
    }

}