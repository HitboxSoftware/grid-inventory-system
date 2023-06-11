using Newtonsoft.Json;
using UnityEngine;

namespace Hitbox.Inventory.Items
{
    public class InventoryContainerItem : InventoryItem
    {
        #region --- VARIABLES ---

        public InventoryGridGroup gridGroup;

        #endregion
        
        #region --- METHODS ---

        #region Properties

        

        #endregion

        #region Data

        public override InventoryItemData GetItemData()
        {
            return new InventoryContainerItemData(this);
        }

        #endregion

        #endregion

        #region --- CONSTRUCTORS ---
        
        public InventoryContainerItem(ContainerItem profile, InventoryGridGroup gridGroup = null, bool rotated = false) : base(profile, rotated)
        {
            gridGroup ??= new InventoryGridGroup(profile.gridSizes);
            
            this.gridGroup = gridGroup;
        }

        public InventoryContainerItem(Item profile, InventoryGrid parentGrid, InventoryGridGroup gridGroup = null, Vector2Int[] takenPositions = null, bool rotated = false) : base(profile, parentGrid, takenPositions, rotated)
        {
            this.gridGroup = gridGroup;
        }

        #endregion
    }
    
    public class InventoryContainerItemData : InventoryItemData
    {
        public InventoryGridGroupData containerGrids;

        #region --- METHODS ---

        public override InventoryItem Load()
        {
            Item profile = DatabaseManager.Instance.GetItemDatabase(dbName).items[id];

            if (profile is not ContainerItem container) return base.Load();
            
            Debug.Log("Loading Container!");
            
            // Creating new item from profile.
            InventoryContainerItem item = new InventoryContainerItem(container, containerGrids, rotated);

            // Setting position to saved position.
            item.takenPositions[0] = new Vector2Int(x, y);
            
            return item;
        }

        #endregion

        #region --- CONSTRUCTORS ---

        public InventoryContainerItemData(InventoryContainerItem item) : base(item)
        {
            containerGrids = item.gridGroup;
        }
        
        [JsonConstructor]
        public InventoryContainerItemData(int x, int y, ushort id, string dbName, bool rotated, InventoryGridGroupData containerGrids) : base(x, y, id, dbName, rotated)
        {
            this.containerGrids = containerGrids;
        }

        public InventoryContainerItemData()
        {
            
        }

        #endregion

        #region --- CONVERSIONS ---

        public static implicit operator InventoryContainerItemData(InventoryContainerItem item) => (InventoryContainerItemData)item.GetItemData();

        #endregion
    }

}