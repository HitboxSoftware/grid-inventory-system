using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hitbox.Stash.UI
{
    /// <summary>
    /// Simple grid that requires no layout groups and will dynamically adjust UI Item size based on container size.
    /// <para/>
    /// Slots aren't visualised so it's not easy to setup visually, but it's far more performant than other grid types
    /// and as such better for large grids.
    /// </summary>
    public class InventoryUISimpleGrid : InventoryUIAbstractGrid, IPointerMoveHandler, IPointerExitHandler
    {
        #region Fields

        [SerializeField] bool setLayoutSize;
        private Vector2Int _currentHoveredPos = -Vector2Int.one;

        #endregion

        #region Methods

        public override bool AssignGrid(InventoryGrid newGrid, bool generateUI)
        {
            if (base.AssignGrid(newGrid, false))
            {
                RectTransform.SetPivot(new Vector2(0, 1)); ;

                if (setLayoutSize)
                {
                    if (TryGetComponent(out LayoutElement layout))
                    {
                        layout.preferredWidth = style.cellSize.x * Grid.Size.x;
                        layout.preferredHeight = style.cellSize.y * Grid.Size.y;
                    }
                    
                    RectTransform.sizeDelta = new Vector2(style.cellSize.x * Grid.Size.x, style.cellSize.y * Grid.Size.y);
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
                
                if (generateUI)
                {
                    Generate();
                }

                return true;
            }

            return false;
        }

        #region Utilities

        public override Vector2 GridToCanvasPoint(Vector2Int gridPos, Vector2 offset = default)
        {
            return transform.TransformPoint(GridToLocalPoint(gridPos, offset));
        }
        
        #endregion

        #endregion
        
        #region UI Events

        public void OnPointerMove(PointerEventData eventData)
        {
            if (Grid == null) return;

            Vector2 pointerLocalPos = ScreenToLocalPoint(eventData.position, eventData.pressEventCamera);
            Vector2Int gridPos = CellToGridPoint(pointerLocalPos);
            if (gridPos == _currentHoveredPos) return;
            if (Grid.CanInsertAtPosition(gridPos, Vector2Int.one, false))
            {
               UpdateGridPos(CellToGridPoint(pointerLocalPos));
                _currentHoveredPos = gridPos;
            }
            else
            {
                _currentHoveredPos = -Vector2Int.one;
                ClearGridPos();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _currentHoveredPos = -Vector2Int.one;
            ClearGridPos();
        }

        #endregion
    }

}