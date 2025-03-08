using System.Collections;
using System.Collections.Generic;
using Hitbox.Collections;
using Hitbox.Stash;
using Hitbox.Stash.Items;
using Hitbox.Stash.UI;
using Hitbox.Stash.UI.Highlight;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIPanelHighlighter : InventoryUIAbstractHighlighter
{
    private InventoryUIManager _manager;
    
    // Runtime
    private SizedQuadTree<GameObject> _currentHighlights;
    private Queue<GameObject> _highlightPool = new Queue<GameObject>();
    private Vector2Int _currentHoveredSlot;
    
    #region MonoBehaviour

    private void Start()
    {
        Init();
    }
    
    private void Update()
    {
        if (_currentHighlights == null) return;

        

        ClearHighlights();
        if (_currentHoveredSlot == -Vector2Int.one) return;

        if (_manager.CurrentDragData != null)
        {
            if (_manager.HoveredSlot.uiGrid == uiGrid)
            {
                HighlightHoveredSlots();
            }
        }
        else if (uiGrid.Style.highlightOnHover)
        {
            HighlightSlots(_currentHoveredSlot, Vector2Int.one, uiGrid.Style.slotHoverHighlightColour);
        }
    }
    
    private void OnEnable()
    {
        InventoryUIAbstractGrid.GridSlotMouseEnter += OnGridSlotMouseEnter;
        InventoryUIAbstractGrid.GridSlotMouseExit += OnGridSlotMouseExit;
    }

    private void OnDisable()
    {
        InventoryUIAbstractGrid.GridSlotMouseEnter -= OnGridSlotMouseEnter;
        InventoryUIAbstractGrid.GridSlotMouseExit -= OnGridSlotMouseExit;
    }

    #endregion
    
    public void HighlightHoveredSlots()
    {
        Vector2 difference = (Vector2)Input.mousePosition -
                             uiGrid.GridToCanvasPoint(_currentHoveredSlot, new Vector2(0.5f, 0.5f));

        Vector2Int placementPos = _currentHoveredSlot - _manager.CurrentDragData.CalculateGridOffset(difference);

        Color desiredColor = uiGrid.Grid.CanInsertAtPosition(placementPos, _manager.CurrentDragData.InvItem)
            ? uiGrid.Style.slotPositiveHighlightColour
            : uiGrid.Style.slotNegativeHighlightColour;

        if (uiGrid.Grid.CanCombineAtPosition(_currentHoveredSlot, _manager.CurrentDragData.InvItem))
        {
            desiredColor = uiGrid.Style.slotNeutralHighlightColour;
        }
        
        HighlightSlots(placementPos, _manager.CurrentDragData.InvItem.Size, desiredColor);
    }

    public bool HighlightSlots(Vector2Int pos, Vector2Int size, Color color, out GameObject highlight)
    {
        Rect rect = GetClampedRect(pos, size);
        if (!_currentHighlights.Bounds.ContainsRect(rect))
        {
            highlight = null;
            return false;
        }
        
        // Getting panel from pool or creating new one.
        GameObject panel;
        Image panelImg;
        if (_highlightPool.Count == 0)
        {
            panel = new GameObject();
            
            panel.transform.SetParent(transform, false);
            
            panelImg = panel.AddComponent<Image>();
            panelImg.sprite = uiGrid.Style.slotHighlightSprite;
            panelImg.type = Image.Type.Tiled;
        }
        else
        {
            panel = _highlightPool.Dequeue();
            panel.SetActive(true);
            panelImg = panel.GetComponent<Image>();
        }

        panelImg.color = color;
        
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        
        rectTransform.sizeDelta = rect.size * uiGrid.Style.cellSize + new Vector2(0.5f, 0);
        
        rectTransform.anchoredPosition = uiGrid.GridToLocalPoint(rect.position);


        if (!_currentHighlights.Insert(rect, panel, false))
        {
            Destroy(panel);
        }

        highlight = panel;
        return true;
    }
    
    public override bool HighlightSlots(Vector2Int pos, Vector2Int size, Color color)
    {
        return HighlightSlots(pos, size, color, out _);
    }
    
    public override void RemoveHighlightAtPosition(Vector2Int position)
    {
        if (_currentHighlights == null) return;
        if (!_currentHighlights.TryGetElementAtPosition(position, out var element)) return;

        // Removing highlight from current list.
        _currentHighlights.RemoveElement(position);
        
        // Disabling highlight
        element.data.SetActive(false);

        // Adding existing highlight back to pool
        _highlightPool.Enqueue(element.data);
    }

    public override void ClearHighlights()
    {
        if (_currentHighlights == null) return;
        
        var elements = new List<(GameObject data, Rect rect)>();
        _currentHighlights.RetrieveAllElements(ref elements);
        foreach ((GameObject currentHighlight, _) in elements)
        {
            currentHighlight.SetActive(false);
            _highlightPool.Enqueue(currentHighlight);
        }
        
        _currentHighlights.Clear();
    }

    public override void SetGrid(InventoryUIAbstractGrid newGrid)
    {
        base.SetGrid(newGrid);

        Init();
    }

    private Rect GetClampedRect(Vector2Int pos, Vector2Int size)
    {
        // Clamping x to zero and reducing size of rect using overflow.
        if (pos.x < 0)
        {
            size.x += pos.x;
            pos.x = 0;
        }
        
        // Clamping y to zero and reducing size of rect using overflow.
        if (pos.y < 0)
        {
            size.y += pos.y;
            pos.y = 0;
        }

        // Clamping x to max grid x and reducing size of rect using overfow.
        if (pos.x + size.x > uiGrid.Grid.Size.x)
        {
            int difference = (pos.x + size.x) - uiGrid.Grid.Size.x;

            size.x -= difference;
        }
        
        // Clamping y to max grid y and reducing size of rect using overflow.
        if (pos.y + size.y > uiGrid.Grid.Size.y)
        {
            int difference = (pos.y + size.y) - uiGrid.Grid.Size.y;

            size.y -= difference;
        }

        return new Rect(pos, size);
    }

    private void Init()
    {
        if (uiGrid == null) return;

        uiGrid.GridAssigned += _ => UpdateBounds();

        _manager = InventoryUIManager.Instance;
        
        UpdateBounds();

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
    }

    public void UpdateBounds()
    {
        if (uiGrid == null || uiGrid.Grid == null) return;
        
        _currentHighlights = new SizedQuadTree<GameObject>(new Rect(Vector2.zero, uiGrid.Grid.Size +  Vector2.one), 20, 2);
    }
    
    private void OnGridSlotMouseExit(InventoryUIAbstractGrid obj)
    {
        RemoveHighlightAtPosition(_currentHoveredSlot);
        _currentHoveredSlot = -Vector2Int.one;
    }

    private void OnGridSlotMouseEnter(InventoryUIAbstractGrid grid, Vector2Int pos)
    {
        if (grid != uiGrid) return;

        _currentHoveredSlot = pos;
    }

}
