using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Stash.UI.Highlight
{
    [RequireComponent(typeof(GridLayoutGroup))]

    public class InventoryUIGridLayoutHighlighter : InventoryUIAbstractHighlighter
    {
        #region Fields

        // Components
        [SerializeField] InventoryUIManager manager;
        private GridLayoutGroup _gridLayout;

        // Runtime
        private Dictionary<Vector2Int, Image> _slots = new Dictionary<Vector2Int, Image>();
        private List<Vector2Int> _currentHighlightedSlots = new List<Vector2Int>();
        private Vector2Int _currentHoveredSlot;

        #endregion

        #region MonoBehaviour

        private IEnumerator Start()
        {
            yield return 0;
            Init();
        }

        private void OnEnable()
        {
            InventoryUIAbstractGrid.GridSlotMouseEnter += OnGridSlotMouseEnter;
            InventoryUIAbstractGrid.GridSlotMouseExit += OnGridSlotMouseExit;
        }

        private void Update()
        {
            ClearHighlights();
            if (_currentHoveredSlot == -Vector2Int.one) return;

            if (manager.CurrentDragData != null)
            {

                Vector2 difference = (Vector2)Input.mousePosition -
                                     uiGrid.GridToCanvasPoint(_currentHoveredSlot, new Vector2(0.5f, 0.5f));

                Vector2Int placementPos = _currentHoveredSlot - manager.CurrentDragData.CalculateGridOffset(difference);

                Color desiredColor = uiGrid.Grid.CanInsertAtPosition(placementPos, manager.CurrentDragData.InvItem)
                    ? uiGrid.Style.slotPositiveHighlightColour
                    : uiGrid.Style.slotNegativeHighlightColour;

                if (uiGrid.Grid.CanCombineAtPosition(_currentHoveredSlot, manager.CurrentDragData.InvItem))
                {
                    desiredColor = uiGrid.Style.slotNeutralHighlightColour;
                }

                HighlightSlots(placementPos, manager.CurrentDragData.InvItem.Size, desiredColor);
            }
            else if (uiGrid.Style.highlightOnHover)
            {
                HighlightSlots(_currentHoveredSlot, Vector2Int.one, uiGrid.Style.slotHoverHighlightColour);
            }
        }

        #endregion

        #region Methods

        private void Init()
        {
            if (uiGrid == null) return;
            
            RectTransform parentTransform = GetComponentInParent<RectTransform>();
            
            Vector2 size = new Vector2(
                x: parentTransform.rect.width / uiGrid.Grid.Size.x,
                y: parentTransform.rect.height / uiGrid.Grid.Size.y);
            
            _gridLayout = GetComponent<GridLayoutGroup>();
            _gridLayout.cellSize = size;
            _gridLayout.spacing = uiGrid.Style.cellSpacing;
            
            for (int y = 0; y < uiGrid.Grid.Size.y; y++)
            {
                for (int x = 0; x < uiGrid.Grid.Size.x; x++)
                { 
                    GameObject slotObj = Instantiate(uiGrid.Style.highlightObject, _gridLayout.transform);
                    slotObj.name = $" )";

                    Vector2Int pos = new Vector2Int(x, y);
                    _slots.Add(pos, slotObj.GetComponent<Image>());
                    _slots[pos].enabled = false;
                }
            }
        }

        public override void SetGrid(InventoryUIAbstractGrid newGrid)
        {
            base.SetGrid(newGrid);
            
            Init();
        }

        public override bool HighlightSlots(Vector2Int pos, Vector2Int size, Color color)
        {
            for (int y = pos.y; y < pos.y + size.y; y++)
            {
                for (int x = pos.x; x < pos.x + size.x; x++)
                {
                    Vector2Int newPos = new Vector2Int(x, y);
                    if (!_slots.ContainsKey(newPos)) continue;

                    _slots[newPos].enabled = true;
                    _slots[newPos].color = color;

                    _currentHighlightedSlots.Add(newPos);
                }
            }

            return true;
        }
        
        public override void RemoveHighlightAtPosition(Vector2Int position)
        {
            if (!_slots.ContainsKey(position)) return;

            _slots[position].enabled = false;

            _currentHighlightedSlots.Remove(position);
        }

        public override void ClearHighlights()
        {
            foreach (Vector2Int slot in _currentHighlightedSlots)
            {
                if (!_slots.ContainsKey(slot)) continue;
                _slots[slot].enabled = false;
            }

            _currentHighlightedSlots.Clear();
        }

        private void OnGridSlotMouseExit(InventoryUIAbstractGrid obj)
        {
            RemoveHighlightAtPositions(_currentHighlightedSlots.ToArray());
            _currentHoveredSlot = -Vector2Int.one;
        }

        private void OnGridSlotMouseEnter(InventoryUIAbstractGrid grid, Vector2Int pos)
        {
            if (grid != uiGrid) return;

            _currentHoveredSlot = pos;
        }

        #endregion
    }

}