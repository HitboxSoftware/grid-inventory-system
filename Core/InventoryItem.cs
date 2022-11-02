using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KoalaDev.UGIS
{
    public class InventoryItem
    {
        #region --- VARIABLES ---

        public readonly GenericItem Item;
        public Vector2Int[] TakenSlots; // All of the slots taken by this item. Index 0 is top left.

        private InventoryGrid parentGrid;

        #endregion

        #region --- METHODS ---

        public bool InSlot(Vector2Int slot) => TakenSlots.Contains(slot);

        public bool InGrid(InventoryGrid grid) => grid == parentGrid;

        public void SetGrid(InventoryGrid grid) => parentGrid = grid;

        #region --- CONSTRUCTOR ---
        
        public InventoryItem(GenericItem item, Vector2Int[] takenSlots, InventoryGrid parentGrid)
        {
            Item = item;
            TakenSlots = takenSlots;
            this.parentGrid = parentGrid;
        }

        #endregion

        #endregion
    }

}