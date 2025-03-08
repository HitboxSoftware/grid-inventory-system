using System.Linq;
using Hitbox.Stash;
using Hitbox.Stash.Categories;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Hitbox/Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    #region Fields

    public ItemProfile[] items = new ItemProfile[ushort.MaxValue];
    public ItemCategory[] categories = new ItemCategory[ushort.MaxValue];

    #endregion
    
    #region Methods
    
    /// <summary>
    /// Find the first available place for an item and insert.
    /// </summary>
    /// <param name="newItemProfile">Item to add</param>
    /// <returns>index of item or -1 if no place found.</returns>
    public int AddToItems(ItemProfile newItemProfile)
    {
        // Generate Items array if not already generated.
        items ??= new ItemProfile[ushort.MaxValue];
        
        // Removing Item from existing records
        RemoveFromItems(newItemProfile);

        // Find first available index.
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] != null) continue;

            items[i] = newItemProfile;

            if (!categories.Contains(newItemProfile.category))
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
    /// <param name="targetItemProfile">Item to remove</param>
    public void RemoveFromItems(ItemProfile targetItemProfile)
    {
        for (int i = items.Length - 1; i > 0; i--)
        {
            ItemProfile itemProfile = items[i];
            if (itemProfile == null) continue;

            if (itemProfile == targetItemProfile)
            {
                Debug.LogWarning($"Warning: {targetItemProfile.name} (Item) removed from {name} (Database) at index {i}!");
                items[i] = null;
            }
        }
    }

    #endregion
}
