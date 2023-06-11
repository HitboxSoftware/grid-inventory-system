using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Hitbox.Inventory
{
    public class InventoryGridGroup
    {
        #region --- VARIABLES ---

        /// <summary>
        /// Grids contained within the group.
        /// </summary>
        public List<InventoryGrid> grids;

        #endregion

        #region --- METHODS ---

        public virtual bool InsertElement(InventoryItem item)
        {
            if (grids is { Count: 0 }) return false;

            foreach (InventoryGrid grid in grids)
            {
                if (grid.InsertItem(item)) return true;
            }

            return false;
        }

        public virtual bool ContainsItem(InventoryItem item)
        {
            return grids.Any(grid => grid.ContainsItem(item));
        }

        public virtual bool ContainsGrid(InventoryGrid grid)
        {
            return grids.Contains(grid);
        }

        #endregion

        #region --- CONSTRUCTORS ---

        /// <summary>
        /// Generates an inventory grid for each of the given sizes
        /// </summary>
        /// <param name="sizes">each element index indicates a grid, value is the size of the grid</param>
        /// <param name="blacklistedItems">Any items to add to the new grid blacklists</param>
        public InventoryGridGroup(IEnumerable<Vector2Int> sizes, InventoryItem[] blacklistedItems = default)
        {
            grids = new List<InventoryGrid>();
            foreach (Vector2Int size in sizes)
            {
                // Creating new grid
                InventoryGrid newGrid = new(size);

                // Updating new grid blacklist
                if (blacklistedItems is not null)
                {
                    newGrid.blacklist.AddRange(blacklistedItems);
                }

                // Adding to group
                grids.Add(newGrid);
            }
        }

        public InventoryGridGroup(int gridCount, Vector2Int size)
        {
            grids = new List<InventoryGrid>();
            for (int i = 0; i < gridCount; i++)
            {
                grids.Add(new InventoryGrid(size));
            }
        }

        public InventoryGridGroup(InventoryGridGroupData data)
        {
            grids = new List<InventoryGrid>();
            foreach (InventoryGridData gridData in data.grids)
            {
                grids.Add(gridData);
            }
        }

        #endregion

        #region --- CONVERTERS ---

        public static implicit operator InventoryGridGroup(InventoryGridGroupData data) => new(data);

        #endregion
    }

    public class InventoryGridGroupData
    {
        public InventoryGridData[] grids;

        public InventoryGridGroupData(InventoryGridGroup group)
        {
            InventoryGridData[] data = new InventoryGridData[group.grids.Count];
            for (int index = 0; index < group.grids.Count; index++)
            {
                data[index] = group.grids[index];
            }
            
            grids = data;
        }

        [JsonConstructor]
        public InventoryGridGroupData(InventoryGridData[] grids)
        {
            this.grids = grids;
        }

        public static implicit operator InventoryGridGroupData(InventoryGridGroup group) => new(group);

    }
}