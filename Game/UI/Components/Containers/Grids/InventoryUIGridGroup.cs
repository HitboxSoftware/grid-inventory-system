using System.Collections.Generic;
using Hitbox.Stash;
using Hitbox.Stash.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIGridGroup : MonoBehaviour
{
    #region Fields
    public Dictionary<InventoryGrid, InventoryUIAbstractGrid> grids = new();
    public InventoryUIStyle style;
    public bool generateOnStart;
    public bool setLayoutSize = false;
    public Vector2Int[] gridSizes = System.Array.Empty<Vector2Int>();
    public int[] rowCapacities = System.Array.Empty<int>();

    private RectTransform _rectTransform;
    
    
    // Related components to the grid group.
    private HorizontalOrVerticalLayoutGroup _layoutGroup;
    private List<GameObject> _generatedRows = new List<GameObject>();

    #endregion

    #region MonoBehaviour

    private void Start()
    {
        Init();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialising the grid group, and generating UI objects if "generateOnStart"
    /// </summary>
    public void Init()
    {
        // TODO: This stuff should probably be paramaterised? (Is that even a word?)
        if(!TryGetComponent(out _layoutGroup))
        {
            _layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            _layoutGroup.spacing = 2;
            _layoutGroup.padding = new RectOffset(5, 5, 5, 5);
            _layoutGroup.childForceExpandWidth = false;
            _layoutGroup.childAlignment = TextAnchor.UpperCenter;
        }

        _rectTransform = GetComponent<RectTransform>();
        
        if(generateOnStart)
            SetGrids(new InventoryGridGroup(gridSizes), rowCapacities, true);
    }

    public void SetGrids(InventoryGridGroup gridGroup, IEnumerable<int> rowCapacities, bool generateUI)
    {
        ClearGridUI();

        int currentGridIndex = 0;
        Vector2Int size = new Vector2Int();
        foreach (int rowCapacity in rowCapacities)
        {
            // Stop if current index is greater than grid count
            if (currentGridIndex >= gridGroup.Grids.Count) break;
            
            Transform rowParent = CreateRow();
            for (int rowIndex = 0; rowIndex < rowCapacity; rowIndex++)
            {
                // Stop if current index is greater than grid count
                if (currentGridIndex >= gridGroup.Grids.Count) break;

                if (gridGroup.Grids[currentGridIndex].Size.x > size.x)
                    size.x = gridGroup.Grids[currentGridIndex].Size.x;
                
                if (gridGroup.Grids[currentGridIndex].Size.y > size.y)
                    size.y = gridGroup.Grids[currentGridIndex].Size.y;
                
                AddGrid(gridGroup.Grids[currentGridIndex], rowParent, generateUI);
                currentGridIndex++;
            }
        }
        
        while (currentGridIndex < gridGroup.Grids.Count)
        {
            AddGrid(gridGroup.Grids[currentGridIndex], CreateRow(), true);
            currentGridIndex++;
        }
        
        if (setLayoutSize)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
            
            _rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)0, LayoutUtility.GetPreferredSize(_rectTransform, 0));
            _rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)1, LayoutUtility.GetPreferredSize(_rectTransform, 1));
        }
    }

    /// <summary>
    /// Add a grid to the group, set's up all the UI and components required.
    /// </summary>
    /// <param name="invGrid">Backend inventory grid to link to</param>
    /// <param name="parent">Parent transform of the new grid</param>
    /// <param name="generateUI">Whether UI should be generated for the grid.</param>
    /// <returns>Whether the grid was successfully created.</returns>
    public bool AddGrid(InventoryGrid invGrid, Transform parent, bool generateUI)
    {
        if (invGrid == null) return false;
        if (grids.ContainsKey(invGrid)) return false;

        GameObject inventoryGridObj = Instantiate(style.gridObj, parent, false);
        inventoryGridObj.name = "InventoryGrid";
        
        if (!inventoryGridObj.TryGetComponent(out InventoryUISimpleGrid uiGrid))
        {
            uiGrid = inventoryGridObj.AddComponent<InventoryUISimpleGrid>();
        }
        
        if (!inventoryGridObj.TryGetComponent(out LayoutElement element))
        {
            element = inventoryGridObj.AddComponent<LayoutElement>();
        }

        GameObject highlighterObj = new GameObject("Highlights", typeof(RectTransform));
        highlighterObj.transform.SetParent(inventoryGridObj.transform, false);
        if (!highlighterObj.TryGetComponent(out InventoryUIPanelHighlighter highlighter))
        {
            highlighter = highlighterObj.AddComponent<InventoryUIPanelHighlighter>();
        }
        
        
        RectTransform highlightTransform = highlighterObj.transform as RectTransform; // I don't see a reason why this would ever be on a non-rect transform.
        highlightTransform.sizeDelta = Vector2.zero; 
        
        Vector2 windowSize = invGrid.Size * style.cellSize;
        
        element.minWidth = windowSize.x;
        element.minHeight = windowSize.y;
        
        uiGrid.SetStyle(style);
        uiGrid.AssignGrid(invGrid, generateUI);
        
        highlighter.SetGrid(uiGrid);
        
        grids.Add(invGrid, uiGrid);
        
        return true;
    }

    /// <summary>
    /// Create a row layout for grids to be added to
    /// </summary>
    /// <returns>Reference to the newly created UGUI row</returns>
    public Transform CreateRow()
    {
        _generatedRows ??= new List<GameObject>(); // Ensure Generated Rows List is Created.
        
        // Creating new row object
        GameObject rowObj = new GameObject("Row", typeof(HorizontalLayoutGroup));
        _generatedRows.Add(rowObj);

        // Parenting to grid group.
        rowObj.transform.SetParent(transform, false);
        
        // Building the layout group for the row.
        HorizontalLayoutGroup horizontalGroup = rowObj.GetComponent<HorizontalLayoutGroup>();
        horizontalGroup.spacing = 2;
        horizontalGroup.childForceExpandWidth = false;
        horizontalGroup.childForceExpandHeight = false;
        horizontalGroup.childAlignment = TextAnchor.UpperCenter;

        return rowObj.transform;
    }

    /// <summary>
    /// Destroys all UI related to this grid group.
    /// </summary>
    public void ClearGridUI()
    {
        foreach (InventoryGrid grid in grids.Keys)
        {
            grids[grid].ClearUI();
            Destroy(grids[grid].gameObject);
        }
        
        grids.Clear();

        foreach (GameObject row in _generatedRows)
        {
            Destroy(row);
        }
        
        _generatedRows.Clear();
    }

    #endregion
    
}
