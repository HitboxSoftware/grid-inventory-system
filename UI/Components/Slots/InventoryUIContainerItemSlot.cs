using Hitbox.Inventory.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Inventory.UI
{
    public class InventoryUIContainerItemSlot : InventoryUIItemSlot
    {
        #region --- VARIABLES ---

        [SerializeField] private InventoryUIGridGroup uiGrids;

        #endregion

        #region --- METHODS ---

        public override void SetSlot(InventoryItemSlot itemSlot)
        {
            base.SetSlot(itemSlot);

            if (itemSlot.HasItem())
            {
                ViewContainer(itemSlot.AttachedItem);
            }
        }

        public void ViewContainer(InventoryItem item)
        {
            if (item == null) return;
            if (item.item is not ContainerItem container) return;
            if (item is not InventoryContainerItem containerInvItem) return;

            containerInvItem.gridGroup ??= new InventoryGridGroup(container.gridSizes, new [] { item });

            uiGrids.SetGrids(containerInvItem.gridGroup, container.rowCapacities, true);
            uiGrids.gameObject.SetActive(true);

            if (transform.parent.TryGetComponent(out RectTransform rect))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        protected override void OnSlotUpdated(InventoryItem invItem)
        {
            base.OnSlotUpdated(invItem);
            
            ClearContainer();
            
            if(LinkedSlot.AttachedItem != null)
                ViewContainer(LinkedSlot.AttachedItem);
        }
        
        protected void ClearContainer()
        {
            uiGrids.ClearGrids();
            uiGrids.gameObject.SetActive(false);
            if (TryGetComponent(out RectTransform rect))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        #endregion
    }
}