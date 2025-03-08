using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hitbox.Stash.UI.Highlight
{
    public abstract class InventoryUIAbstractHighlighter : MonoBehaviour
    {
        #region Fields

        [SerializeField] protected InventoryUIAbstractGrid uiGrid;
        
        #endregion

        #region Methods
        
        /// <summary>
        /// Highlight a Rectangle based on given position and size.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public abstract bool HighlightSlots(Vector2Int pos, Vector2Int size, Color color);

        /// <summary>
        /// Remove any highlights at the given positions
        /// </summary>
        /// <param name="positions">list of positions to remove</param>
        public virtual void RemoveHighlightAtPositions(Vector2Int[] positions)
        {
            foreach (Vector2Int pos in positions)
            {
                RemoveHighlightAtPosition(pos);
            }
        }
        
        /// <summary>
        /// Remove any highlights at the given position
        /// </summary>
        /// <param name="position">position to remove</param>
        public abstract void RemoveHighlightAtPosition(Vector2Int position);

        /// <summary>
        /// Clear all active highlights
        /// </summary>
        public abstract void ClearHighlights();

        public virtual void SetGrid(InventoryUIAbstractGrid newGrid)
        {
            uiGrid = newGrid;
        }
        #endregion
    }

}