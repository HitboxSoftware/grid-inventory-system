using UnityEngine;
using UnityEngine.EventSystems;

namespace Hitbox.Inventory.UI
{
    /// <summary>
    /// Simple grid that requires no layout groups and will dynamically adjust UI Item size based on container size.
    /// <para/>
    /// Slots aren't visualised so it's not easy to setup visually, but it's more performant friendly than other grid types
    /// and so is better for very large grids.
    /// </summary>
    public class InventoryUISimpleGrid : InventoryUIAbstractGrid, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region --- METHODS ---
        
        #region Utilities

        public override Vector2 GridToCellPoint(Vector2 gridPoint)
        {
            if (rectTransform == null) return Vector2.zero;

            Vector2 cellSize = CalculateCellSize();
            Vector2 pos = new Vector2(
                x: cellSize.x * (gridPoint.x + 0.5f),
                y: cellSize.y * -(gridPoint.y + 0.5f)
            );

            return pos;
        }

        #endregion

        #endregion
        
        #region --- UI EVENTS ---

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 localPoint = ScreenToLocalPoint(eventData.position, eventData.pressEventCamera);
            Debug.Log(CellToGridPoint(localPoint));
        }

        #endregion
    }

}