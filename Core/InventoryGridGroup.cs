using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hitbox.Stash
{
    public class InventoryGridGroup
    {
        #region Fields

        /// <summary>
        /// Grids contained within the group.
        /// </summary>
        public List<InventoryGrid> Grids;

        public InventoryItem[] AllItems => GetAllItems();

        #endregion

        #region Methods

        #region Insertion

        public virtual bool InsertItem(InventoryItem item, bool combineItems = false)
        {
            if (Grids is { Count: 0 }) return false;

            foreach (InventoryGrid grid in Grids)
            {
                if (grid.InsertItem(item, combineItems)) return true;
            }

            return false;
        }
        
        #endregion

        #region Removal

        public virtual bool RemoveItem(InventoryItem invItem, bool clearItem = true)
        {
            foreach (InventoryGrid grid in Grids)
            {
                if (grid.RemoveItem(invItem, clearItem)) return true;
            }

            return false;
        }

        #endregion

        public virtual bool ContainsItem(InventoryItem item)
        {
            return Grids.Any(grid => grid.ContainsItem(item));
        }

        public virtual bool ContainsGrid(InventoryGrid grid)
        {
            return Grids.Contains(grid);
        }

        public void Load(InventoryGridGroupData data)
        {
            for (int index = 0; index < Grids.Count; index++)
            {
                InventoryGrid grid = Grids[index];
                grid.Load(data.Grids[index]);
            }
        }

        public InventoryItem[] GetAllItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            foreach (InventoryGrid grid in Grids)
            {
                items.AddRange(grid.Items);
            }

            return items.ToArray();
        }

        public bool CanInsertInGrids(InventoryItem item)
        {
            // If any grids have space for the given item, return true.
            return Grids.Any(inventoryGrid => inventoryGrid.CanInsertInGrid(item));
        }
        

        #endregion

        #region Constructors

        /// <summary>
        /// Generates an inventory grid for each of the given sizes
        /// </summary>
        /// <param name="sizes">each element index indicates a grid, value is the size of the grid</param>
        /// <param name="blacklistedItems">Any items to add to the new grid blacklists</param>
        public InventoryGridGroup(IEnumerable<Vector2Int> sizes, InventoryItem[] blacklistedItems = default)
        {
            Grids = new List<InventoryGrid>();

            foreach (Vector2Int size in sizes)
            {
                // Creating new grid
                InventoryGrid newGrid = new(size);

                // Updating new grid blacklist
                if (blacklistedItems is not null)
                {
                    newGrid.Blacklist.AddRange(blacklistedItems);
                }

                // Adding to group
                Grids.Add(newGrid);
            }
        }

        public InventoryGridGroup(int gridCount, Vector2Int size)
        {
            Grids = new List<InventoryGrid>();
            for (int i = 0; i < gridCount; i++)
            {
                Grids.Add(new InventoryGrid(size));
            }
        }

        #endregion
    }

    public class InventoryGridGroupData
    {
        public readonly InventoryGridData[] Grids;

        public InventoryGridGroupData(InventoryGridGroup group)
        {
            InventoryGridData[] data = new InventoryGridData[group.Grids.Count];

            if (group.AllItems.Length > 0)
            {
                for (int index = 0; index < group.Grids.Count; index++)
                {
                    data[index] = group.Grids[index];
                }
            
                Grids = data;
            }
            else
            {
                Grids = null;
            }
        }

        public InventoryGridGroupData(InventoryGridData[] grids)
        {
            this.Grids = grids;
        }

        public static implicit operator InventoryGridGroupData(InventoryGridGroup group) => new(group);

    }
}