using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash;
using Hitbox.Stash.UI;
using UnityEngine;

public class InventoryUIUnexaminedItemDisplay : MonoBehaviour
{
    public InventoryUIGridDrawer GridDrawer;
    public InventoryUIAbstractGrid UIGrid;

    private InventoryExaminableGrid _grid;

    private Dictionary<InventoryItem, GameObject> _panels = new();

    private void Start()
    {
        if (UIGrid.Grid is not InventoryExaminableGrid grid)
        {
            return;
        }

        _grid = grid;

        foreach (var inventoryItem in _grid.AllItems)
        {
            if (_grid.ExaminedItems.Contains(inventoryItem)) continue;

            DrawGridPanel(inventoryItem);
        }
    }

    public void DrawGridPanel(InventoryItem inventoryItem)
    {
        GameObject panel = GridDrawer.DrawRect(UIGrid.Grid.IndexToPosition(inventoryItem.Index), inventoryItem.Size, UIGrid.Style.slotHighlightSprite, new Color(0.05f, 0.05f, 0.05f));
        
        _panels.Add(inventoryItem, panel);
    }

    private void OnGridUpdated(InventoryItem inventoryItem)
    {
        if (!_grid.ExaminedItems.Contains(inventoryItem))
        {
            DrawGridPanel(inventoryItem);
            return;
        }

        GameObject obj = _panels[inventoryItem];
        
        GridDrawer.RemoveRect(obj);
    }

    private void OnEnable()
    {
        UIGrid.Grid.OnUpdated += OnGridUpdated;
    }

    private void OnDisable()
    {
        UIGrid.Grid.OnUpdated -= OnGridUpdated;
    }
}
