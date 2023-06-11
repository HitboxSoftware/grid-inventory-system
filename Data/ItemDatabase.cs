using System.Linq;
using Hitbox.Inventory;
using Hitbox.Inventory.Categories;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Hitbox/Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    #region --- VARIABLES ---

    public Item[] items = new Item[ushort.MaxValue];
    public ItemCategory[] categories = new ItemCategory[ushort.MaxValue];

    #endregion
    
    #region --- METHODS ---
    
    /// <summary>
    /// Find the first available place for an item and insert.
    /// </summary>
    /// <param name="newItem">Item to add</param>
    /// <returns>index of item or -1 if no place found.</returns>
    public int AddToItems(Item newItem)
    {
        // Generate Items array if not already generated.
        items ??= new Item[ushort.MaxValue];
        
        // Removing Item from existing records
        RemoveFromItems(newItem);

        // Find first available index.
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] != null) continue;

            items[i] = newItem;

            if (!categories.Contains(newItem.category))
            {
                
            }
            
            return i;
        }

        // No available index for item
        return -1; 
    }

    /// <summary>
    /// Removes all occurrences of target item from database.
    /// </summary>
    /// <param name="targetItem">Item to remove</param>
    public void RemoveFromItems(Item targetItem)
    {
        for (int i = items.Length - 1; i > 0; i--)
        {
            Item item = items[i];
            if (item == null) continue;

            if (item == targetItem)
            {
                Debug.LogWarning($"Warning: {targetItem.name} (Item) removed from {name} (Database) at index {i}!");
                items[i] = null;
            }
        }
    }

    #endregion
}
