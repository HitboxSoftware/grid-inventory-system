using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIGridGroup : MonoBehaviour
{
    #region --- VARIABLES ---
    public Dictionary<InventoryGrid, InventoryUIAbstractGrid> grids = new();
    public InventoryUIStyle style;
    public bool generateOnStart = true;
    
    // Components
    private VerticalLayoutGroup _layoutGroup;

    private List<GameObject> _generatedRows = new List<GameObject>();

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        if (generateOnStart)
        {
            Init();
        }
    }

    #endregion

    #region --- METHODS ---

    public void Init()
    {
        if(!TryGetComponent(out _layoutGroup))
        {
            _layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            _layoutGroup.spacing = 2;
            _layoutGroup.padding = new RectOffset(5, 5, 5, 5);
            _layoutGroup.childForceExpandWidth = false;
            _layoutGroup.childAlignment = TextAnchor.UpperCenter;
        }
    }

    public void SetGrids(InventoryGridGroup gridGroup, IEnumerable<int> rowCapacities, bool generateUI)
    {
        ClearGrids();

        int currentGridIndex = 0;
        foreach (int rowCapacity in rowCapacities)
        {
            // Stop if current index is greater than grid count
            if (currentGridIndex >= gridGroup.grids.Count) break;
            
            Transform rowParent = CreateRow();
            for (int rowIndex = 0; rowIndex < rowCapacity; rowIndex++)
            {
                // Stop if current index is greater than grid count
                if (currentGridIndex >= gridGroup.grids.Count) break;
                
                AddGrid(gridGroup.grids[currentGridIndex], rowParent, generateUI);
                currentGridIndex++;
            }
        }
        
        while (currentGridIndex < gridGroup.grids.Count)
        {
            AddGrid(gridGroup.grids[currentGridIndex], CreateRow(), true);
            currentGridIndex++;
        }
    }

    public bool AddGrid(InventoryGrid invGrid, Transform parent, bool generateUI)
    {
        if (invGrid == null) return false;
        if (grids.ContainsKey(invGrid)) return false;

        GameObject inventoryGridObj = Instantiate(style.gridObj, parent, false);
        inventoryGridObj.name = "InventoryGrid";

        if (!inventoryGridObj.TryGetComponent(out InventoryUISlotGrid uiGrid))
        {
            uiGrid = inventoryGridObj.AddComponent<InventoryUISlotGrid>();
        }

        uiGrid.SetStyle(style);
        uiGrid.Assign(invGrid, generateUI);

        grids.Add(invGrid, uiGrid);
        
        return true;
    }

    public Transform CreateRow()
    {
        _generatedRows ??= new List<GameObject>(); // Ensure Generated Rows List is Created.
        
        // Creating new row object
        GameObject rowObj = new GameObject("Row", typeof(HorizontalLayoutGroup));
        _generatedRows.Add(rowObj);

        rowObj.transform.SetParent(transform, false);
        // Setting Spacing
        HorizontalLayoutGroup horizontalGroup = rowObj.GetComponent<HorizontalLayoutGroup>();
        horizontalGroup.spacing = 2;
        horizontalGroup.childForceExpandWidth = false;
        horizontalGroup.childAlignment = TextAnchor.MiddleCenter;

        return rowObj.transform;
    }

    public void ClearGrids()
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
