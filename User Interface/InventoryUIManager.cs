using System.Collections.Generic;
using KoalaDev.Utilities.Data;
using UnityEngine;
using UnityEngine.UI;

namespace KoalaDev.UGIS.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        #region --- VARIABLES ---

        public static InventoryUIManager Instance;
        
        private HeldItemData heldItem;
        private InventoryUIItem selectedItem;

        [SerializeField] private Transform heldItemContainer;

        public List<InventoryGrid> managedGrids;

        public List<Item> startingItems;

        [SerializeField] private Color highlightColour;

        #region - INTERACTIONS -

        private InventoryUIContextMenu currentContextMenu;

        public Interaction dropItemInteraction;
        public Interaction equipItemInteraction;

        #endregion

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Warning: Reassigned [Player Inventory Manager] Instance");
            }
            
            Instance = this;

            foreach (InventoryGrid grid in managedGrids)
            {
                for (int i = 0; i < 12; i++)
                {
                    grid.AutoAddItem(new InventoryItem(startingItems[Random.Range(0, startingItems.Count)], null, grid));
                }
            }

        }

        private void OnEnable()
        {
            dropItemInteraction.Interact += DropItem;
        }

        private void OnDisable()
        {
            dropItemInteraction.Interact -= DropItem;
        }

        private void Update()
        {
            if (heldItem != null)
            {
                heldItem.UIItem.transform.position = Input.mousePosition + heldItem.MouseOffset;
            }
        }

        #endregion

        #region --- METHODS ---

        public void SlotClick(InventoryUISlot slot)
        {
            if (currentContextMenu != null)
            {
                InventoryUIContextMenu.RemoveMenu();
                UnselectItem();
            }
            
            if (heldItem == null && slot.containedItem != null)
            {
                GrabItem(slot);
                return;
            }

            if (heldItem != null)
            {
                PlaceItem(slot);
            }
        }
        private void GrabItem(InventoryUISlot slot) //Attaches the Item to the Mouse
        {
            if (heldItem != null) return;

            InventoryUIItem invItem = slot.containedItem;

            // --- Item Offsets ---
            Vector2Int heldItemGridOffset = slot.slotPosition - invItem.InvItem.TakenSlots[0];
            Vector3 heldItemMouseOffset = invItem.transform.position - Input.mousePosition;
            
            heldItem = new HeldItemData(invItem, heldItemMouseOffset, heldItemGridOffset);

            if (heldItemContainer != null)
            {
                invItem.transform.SetParent(heldItemContainer);
            }

            invItem.uiGrid.RemoveItem(invItem);
        }

        private void SelectItem(InventoryUIItem item)
        {
            if (selectedItem != null)
            {
                UnselectItem();
            }

            if (item == null) return;

            selectedItem = item; 
            item.Highlight(highlightColour);
        }

        private void UnselectItem()
        {
            if (selectedItem == null) return;
            
            // Remove Current Context Menu, This could cause problems?
            if (currentContextMenu != null) Destroy(currentContextMenu);

            selectedItem.RemoveHighlight();
            selectedItem = null;
        }
        
        private void PlaceItem(InventoryUISlot slot) //Places Attached Item to Clicked Slot
        {
            if(!slot.grid.AddItemAtPosition(heldItem.UIItem, slot.slotPosition - heldItem.GridOffset)) return;
            
            heldItem = null;
        }

        public void DropItem()
        {
            Debug.Log($"Dropped Item! {selectedItem.InvItem.Item.name}");
            selectedItem.uiGrid.RemoveItem(selectedItem, true);
            UnselectItem();
        }

        #endregion

        #region --- ITEM ACTIONS ---

        #region - CONTEXT MENU -

        public void CreateContextMenu(InventoryUISlot slot, Vector2 clickPos)
        {
            if (heldItem != null) return;
            
            if (currentContextMenu != null)
            {
                Destroy(currentContextMenu.gameObject);
            }

            SelectItem(slot.containedItem);
            
            if (selectedItem == null) return;
            
            GameObject contextMenuObj = Instantiate(slot.grid.GetStyle.menuObj, transform);

            // Rebuild Layout to Correctly Set Menu Position
            LayoutRebuilder.ForceRebuildLayoutImmediate(contextMenuObj.GetComponent<RectTransform>());

            contextMenuObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            contextMenuObj.transform.position = clickPos;

            InventoryUIContextMenu contextMenu = contextMenuObj.AddComponent<InventoryUIContextMenu>();

            contextMenu.invUIItem = slot.containedItem;

            currentContextMenu = contextMenu;
        }

        public void RemoveContextMenu()
        {
            if (currentContextMenu == null) return;
            
            UnselectItem();
            Destroy(currentContextMenu.gameObject);
        }

        #endregion

        #endregion
    }

    internal class HeldItemData // Used to store data related to the current held item (item attached to mouse cursor)
    {
        public readonly InventoryUIItem UIItem;
        
        // Offsets
        public readonly Vector3 MouseOffset;
        public readonly Vector2Int GridOffset;
        public readonly Vector2Int OriginalSlot;
        public readonly InventoryUIGrid OriginalGrid;

        public HeldItemData(InventoryUIItem uiItem, Vector3 mouseOffset, Vector2Int gridOffset)
        {
            UIItem = uiItem;
            MouseOffset = mouseOffset;
            GridOffset = gridOffset;
        }
    }

}