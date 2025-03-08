using Hitbox.Stash.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Stash.UI
{
    public class InventoryUIContainerItemSlot : InventoryUIItemSlot
    {
        #region Fields

        [SerializeField] InventoryUIGridGroup uiGrids;

        #endregion

        #region Methods

        public override void SetSlot(InventoryItemSlot itemSlot)
        {
            base.SetSlot(itemSlot);

            if (itemSlot.IsItemAttached())
            {
                ViewContainer(itemSlot.AttachedItem);
            }
        }

        public void ViewContainer(InventoryItem item)
        {
            if (item == null) return;
            if (item.ItemProfile is not ContainerItemProfile container) return;
            if (item is not InventoryContainerItem containerInvItem) return;

            containerInvItem.GridGroup ??= new InventoryGridGroup(container.gridSizes, new [] { item });

            uiGrids.SetGrids(containerInvItem.GridGroup, container.rowCapacities, true);
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
            uiGrids.ClearGridUI();
            uiGrids.gameObject.SetActive(false);
            if (TryGetComponent(out RectTransform rect))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }

        #endregion
    }
}