using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hitbox.Stash;
using UnityEngine;

public class InventoryExaminableGrid : InventoryGrid
{
    public List<InventoryItem> ExaminedItems = new List<InventoryItem>();

    public override InventoryItem[] Items => ExaminedItems.ToArray();

    public InventoryItem[] AllItems => Grid.GetAllData();

    public IEnumerator ExamineAllItems()
    {
        foreach (var inventoryItem in AllItems)
        {
            if(ExaminedItems.Contains(inventoryItem)) continue;

            ExaminedItems.Add(inventoryItem);
            OnUpdate(inventoryItem);
            yield return new WaitForSeconds(1);
        }
    }
    
    public override bool TryGetItemAtPosition(Vector2Int position, out InventoryItem invItem)
    {
        bool itemAtPosition = base.TryGetItemAtPosition(position, out invItem);

        if (!itemAtPosition) return false;
        
        // Making sure the item has been examined.
        // TODO: I don't like this solution
        return ExaminedItems.Contains(invItem);
    }

    public override bool ContainsItem(InventoryItem invItem)
    {
        return ExaminedItems.Contains(invItem);
    }

    public void GetOrderedItems()
    {
        var items = Items;

        var sorted = items.OrderBy(i => i.Index).ToArray();
        Queue<InventoryItem> queue = new Queue<InventoryItem>(sorted);
        
    }
    
    public InventoryExaminableGrid(Vector2Int size) : base(size)
    {
        
    }
}
