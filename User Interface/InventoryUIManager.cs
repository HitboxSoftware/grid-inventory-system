using System;
using System.Collections.Generic;
using System.Linq;
using KoalaDev.UGIS.Items;
using KoalaDev.UI;
using KoalaDev.Utilities.Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace KoalaDev.UGIS.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        #region --- VARIABLES ---

        public static InventoryUIManager Instance { get; private set; }
        
        // - Temporary - 
        public List<Item> startingItems;
        public List<InventoryGrid> managedGrids = new List<InventoryGrid>();

        [SerializeField] private Transform heldItemContainer;
        [SerializeField] private Transform gridObjContainer;
        
        // - Runtime Data -
        private HeldItemData heldItem;
        private InventoryUIItem selectedItem;
        private InventoryUIContextMenu currentContextMenu;
        private InventoryUIGrid lastAddedContainer;
        private Dictionary<InventoryItem, UIWindow> openContainers = new Dictionary<InventoryItem, UIWindow>();

        #region - INTERACTIONS -

        [Header("Interaction Channels")] 
        [SerializeField] private InteractionChannel dropItemInteractionChannel;
        [SerializeField] private InteractionChannel openItemInteractionChannel;

        private event Action<InventoryUIGrid> ItemMoved;

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
                    grid.AutoAddItem(new InventoryItem(startingItems[Random.Range(0, startingItems.Count)], grid));
                }
            }

        }

        private void Update()
        {
            if (heldItem != null)
            {
                heldItem.UIItem.transform.position = Input.mousePosition + heldItem.MouseOffset;
            }
        }
        
        private void OnEnable()
        {
            SubscribeActions();
        }

        private void OnDisable()
        {
            UnsubscribeActions();
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

            if (heldItem != null && slot.containedItem == true)
            {
                (InventoryItem slotInvItem, InventoryItem heldInvItem) = slot.containedItem.InvItem.Item.ItemToItem(slot.containedItem.InvItem, heldItem.UIItem.InvItem);

                if (slotInvItem == null)
                {
                    Destroy(slot.containedItem.gameObject);
                    slot.containedItem = null;
                } else slot.containedItem.InvItem.Item.Updated();
                
                if (heldInvItem == null)
                {
                    Destroy(heldItem.UIItem.gameObject);
                    heldItem = null;
                } else heldItem.UIItem.InvItem.Item.Updated();
                
                return;
            }

            if (heldItem == null && slot.containedItem != null)
            {
                GrabItem(slot);
                return;
            }

            if (heldItem != null)
            {
                PlaceItem(slot);
                return;
            }
        }

        #region - ITEM MOVEMENT -

        private void GrabItem(InventoryUISlot slot) //Attaches the Item to the Mouse
        {
            if (heldItem != null) return;
            
            // Close Container window if picking up container item.
            if (openContainers.ContainsKey(slot.containedItem.InvItem))
            {
                if (openContainers[slot.containedItem.InvItem] != null)
                    openContainers[slot.containedItem.InvItem].Close();

                openContainers.Remove(slot.containedItem.InvItem);
            }

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

        private void PlaceItem(InventoryUISlot slot) //Places Attached Item to Clicked Slot
        {
            if (openContainers.ContainsKey(heldItem.UIItem.InvItem)) return;
            if(!slot.grid.AddUIItemAtPosition(heldItem.UIItem, slot.slotPosition - heldItem.GridOffset)) return;

            heldItem = null;
        }

        #endregion

        #region - ITEM SELECTION -

        private void SelectItem(InventoryUIItem item)
        {
            if (selectedItem != null)
            {
                UnselectItem();
            }

            if (item == null) return;

            selectedItem = item; 
        }

        private void UnselectItem()
        {
            if (selectedItem == null) return;
            
            // Remove Current Context Menu, This could cause problems? EDIT: CAN'T REMEMBER WHY THIS WOULD CAUSE PROBLEMS.
            if (currentContextMenu != null) Destroy(currentContextMenu);

            selectedItem.RemoveHighlight();
            selectedItem = null;
        }

        #endregion

        #region - ITEM FUNCTIONS -

        public void DropItem()
        {
            Debug.Log($"Dropped Item! {selectedItem.InvItem.Item.name}");
            if (openContainers.ContainsKey(selectedItem.InvItem))
            {
                openContainers[selectedItem.InvItem].Close();
            }
            selectedItem.uiGrid.RemoveItem(selectedItem, true);
            UnselectItem();
        }

        public void OpenInventoryContainer()
        {
            if (openContainers.ContainsKey(selectedItem.InvItem))
            {
                if (openContainers[selectedItem.InvItem] != null) return; // Return if window still open.
                
                openContainers.Remove(selectedItem.InvItem);
            }

            UIWindowManager windowManager = UIWindowManager.Instance;

            if (windowManager == null)
            {
                Debug.LogWarning("Warning: Can't open inventory container, no UI Window Manager in Scene.");
                return;
            }

            UIWindow lastContainer = openContainers.LastOrDefault().Value;
            Vector2 newContainerPos = Vector2.zero;

            if (lastContainer != null)
            {
                newContainerPos = lastContainer.gameObject.transform.localPosition + new Vector3(50, -50);
            }
            
            UIWindow window = windowManager.CreateWindow("Container", newContainerPos);

            InventoryUIItem uiItem = selectedItem;
            if (uiItem.InvItem.Item is not Container container) return;
            
            ContainerItemData itemData = (ContainerItemData)uiItem.InvItem.ItemData;
            
            GameObject gridObj = Instantiate(uiItem.uiGrid.GetStyle.gridObj, window.Content);

            if (!TryGetComponent(out InventoryUIGrid uiGrid))
            {
                uiGrid = gridObj.AddComponent<InventoryUIGrid>();
            }

            if (lastAddedContainer == null)
            {
                gridObj.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
            }
            else
            {
                gridObj.transform.position = lastAddedContainer.transform.position + new Vector3(20, -20);
            }

            itemData.containerGrid ??= new InventoryGrid(container.itemStorageSize);

            itemData.containerGrid.ItemBlacklist.Add(uiItem.InvItem);
            uiGrid.SetStyle(uiItem.uiGrid.GetStyle);
            uiGrid.AssignGrid(itemData.containerGrid, true);

            lastAddedContainer = uiGrid;

            openContainers.Add(uiItem.InvItem, window);
            window.Closed += OnContainerClosed;
        }

        public void OnContainerClosed(UIWindow window)
        {
            ContainerItemData containerItemData = openContainers.Where(
                container => container.Value == window).Select(
                container => (ContainerItemData)container.Key.ItemData).FirstOrDefault();

            if (containerItemData == null) return;

            // Return Held Item to Container if Held Item's original grid is being closed.
            if(heldItem != null && heldItem.UIItem.uiGrid.GetGrid == containerItemData.containerGrid)
            {
                heldItem.UIItem.uiGrid.AddUIItemAtPosition(heldItem.UIItem, heldItem.UIItem.InvItem.TakenSlots[0]);
                heldItem = null;
            }

            // Closing Container Windows that are children of current window being closed, this is recursive.
            List<InventoryItem> childContainers = 
                openContainers.Keys.Where(item => containerItemData.containerGrid.ContainsItem(item)).ToList();
            
            for (int i = childContainers.Count - 1; i >= 0; i--)
            {
                if(openContainers[childContainers[i]] != null)
                    openContainers[childContainers[i]].Close();
                
                openContainers.Remove(childContainers[i]);
            }
        }

        /*private void UnloadMagazine()
        {
            if (selectedItem.InvItem.Item is not Magazine) return;

            Bullet[] bullets = Magazine.UnloadMagazine((MagazineItemData)selectedItem.InvItem.ItemData);

            foreach (Bullet bullet in bullets)
            {
                selectedItem.uiGrid.AutoAddUIItem(selectedItem.uiGrid.CreateUIItem(bullet));
            }
            
            selectedItem.InvItem.Item.Updated();
            UnselectItem();
        }*/

        #endregion

        #endregion

        #region --- ITEM ACTIONS ---

        #region - SUBSCRIPTIONS -

        private void SubscribeActions()
        {
            dropItemInteractionChannel.Interact += DropItem;
            openItemInteractionChannel.Interact += OpenInventoryContainer;
        }
        
        private void UnsubscribeActions()
        {
            dropItemInteractionChannel.Interact -= DropItem;
            openItemInteractionChannel.Interact -= OpenInventoryContainer;  
        }

        #endregion

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

        public HeldItemData(InventoryUIItem uiItem, Vector3 mouseOffset, Vector2Int gridOffset)
        {
            UIItem = uiItem;
            MouseOffset = mouseOffset;
            GridOffset = gridOffset;
        }
    }

}