using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hitbox.Stash.UI.Debugging
{
    public class InventoryUIGridInitializer : MonoBehaviour
    {
        #region Fields

        public InventoryUIAbstractGrid uiGrid;
        [SerializeField] protected Vector2Int gridSize;

        public List<ItemProfile> initialItems = new List<ItemProfile>();

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Init();
        }

        #endregion

        #region Methods

        public virtual InventoryGrid BuildGrid()
        {
            return new InventoryGrid(gridSize);
        }

        public virtual void Init()
        {
            var grid = BuildGrid();

            foreach (var initialItem in initialItems)
            {
                grid.InsertItem(initialItem.CreateItem());
            }
            
            uiGrid.AssignGrid(grid, true);
        }

        #endregion
    }

}