using UnityEngine;

namespace Hitbox.Stash.Items
{
    public class InventoryContainerItem : InventoryItem
    {
        #region Fields

        public InventoryGridGroup GridGroup;

        #endregion

        #region Methods

        #region Data

        public override InventoryItemData GetItemData()
        {
            return new InventoryContainerItemData(this);
        }

        #endregion
        
        public override bool TryCombineItem(InventoryItem combinableItem)
        {
            if (!CanCombineItem(combinableItem)) return false;
            
            // Try and insert the item into one of the containers
            if (GridGroup.InsertItem(combinableItem)) return true;

            return false;
        }
        
        public override bool CanCombineItem(InventoryItem combinableItem)
        {
            // Return if either item doesn't exist
            if (combinableItem == null) return false;
            // Return if container grid doesn't exist
            if (GridGroup == null) return false;
            // Return if no space in container
            if (!GridGroup.CanInsertInGrids(combinableItem)) return false;

            return true;
        }

        #endregion

        #region Constructors

        public InventoryContainerItem(ContainerItemProfile profile, bool rotated = false, InventoryGridGroup gridGroup = null)
            : base(profile, rotated)
        {
            gridGroup ??= new InventoryGridGroup(profile.gridSizes);

            GridGroup = gridGroup;
        }

        public InventoryContainerItem(ItemProfile profile, InventoryGrid parentGrid, bool rotated = false,
            InventoryGridGroup gridGroup = null) : base(profile, parentGrid, rotated)
        {
            GridGroup = gridGroup;
        }

        #endregion
    }

    public class InventoryContainerItemData : InventoryItemData
    {
        public InventoryGridGroupData ContainerGrids;

        #region Methods

        public override InventoryItem Load()
        {
            ItemProfile profile = DatabaseManager.Instance.GetItemDatabase(dbName).items[id];

            if (profile is not ContainerItemProfile container) return base.Load();

            Debug.Log("Loading Container!");

            // Creating new item from profile.
            InventoryContainerItem item = new InventoryContainerItem(container, rotated);

            item.GridGroup ??= new InventoryGridGroup(container.gridSizes, new InventoryItem[] { item });

            item.GridGroup.Load(ContainerGrids);

            return item;
        }

        #endregion

        #region Constructors

        public InventoryContainerItemData(InventoryContainerItem item) : base(item)
        {
            if (item.GridGroup != null && item.GridGroup.AllItems.Length > 0)
            {
                ContainerGrids = item.GridGroup;
            }
            else
            {
                ContainerGrids = null;
            }
        }

        public InventoryContainerItemData(int index, ushort id, string dbName, bool rotated,
            InventoryGridGroupData containerGrids) : base(index, id, dbName, rotated)
        {
            this.ContainerGrids = containerGrids;
        }

        public InventoryContainerItemData()
        {
            
        }

        #endregion

        #region Conversions

        public static implicit operator InventoryContainerItemData(InventoryContainerItem item) =>
            (InventoryContainerItemData)item.GetItemData();

        #endregion
    }
}