using System.Collections.Generic;
using System.Linq;
using Hitbox.UGIS;
using Hitbox.UGIS.Interactions;
using Hitbox.UGIS.Items;
using Hitbox.UGIS.UI.ContextMenu;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.UI
{
    public class UIInventoryManager : MonoBehaviour
    {
        #region --- VARIABLES ---

        public static UIInventoryManager Instance;

        [SerializeField] private Transform dragItemContainer;

        // --- Runtime ---
        // Hover
        [SerializeField] private UIInventorySlot hoveredSlot;
        private UIInventorySlot previousHoveredSlot;
        
        // Drag n Drop
        private bool dragging;
        private ItemDragData draggedItem;
        private Vector2 previousDifference;
        
        // Item Highlighting
        private UIInventorySlot[] currentHighlightSlots;
        
        // Context Menu
        private UIInventoryContextMenu currentContextMenu;

        // Containers
        private readonly Dictionary<InventoryItem, UIWindow> openContainers = new();
        
        // --- Events ---
        public InventoryInteractionChannel openAction;
        
        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            UpdateDrag();
            UpdateRotation();
            UpdateContextMenu();
        }

        private void OnEnable()
        {
            Subscriptions();
        }

        #endregion

        #region --- METHODS ---

        private void Init()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Warning: Two UIInventoryManager instances in scene, destroying duplicate.", gameObject);
                Destroy(this);
            }

            Instance = this;
        }

        #region Drag & Drop

        private void UpdateDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (dragging) EndDrag();

                StartDrag();
            }

            if (dragging && Input.GetMouseButton(0))
                Drag();
            else if (dragging)
                EndDrag();

            Cursor.visible = !dragging;
        }
        
        private void StartDrag()
        {
            if (hoveredSlot == null || hoveredSlot.containedItem == null) return;

            dragging = true;
            
            UIInventoryItem uiItem = hoveredSlot.containedItem;
            
            draggedItem = new ItemDragData(uiItem, uiItem.InvItem.TakenSlots[0], uiItem.InvItem.ItemRuntimeData.rotated);

            if (dragItemContainer != null)
            {
                uiItem.transform.SetParent(dragItemContainer);
            }
            
            if(openContainers.ContainsKey(hoveredSlot.containedItem.InvItem))
                openContainers[hoveredSlot.containedItem.InvItem].Close();

            uiItem.UIGrid.RemoveUIItem(uiItem);
        }
        
        private void Drag()
        {
            if (!dragging || draggedItem == null) return;
            
            draggedItem.UIItem.transform.position = Input.mousePosition;
            
            if (currentHighlightSlots is { Length: > 0 })
            {
                foreach (UIInventorySlot currentHighlightSlot in currentHighlightSlots)
                {
                    currentHighlightSlot.HighlightSlot(new Color(0, 0, 0, 0.19f));
                }
            }

            if (hoveredSlot == null) return;
            
            // --- CALCULATE GRID OFFSET ---
            Vector2 difference = Input.mousePosition - hoveredSlot.transform.position;
            Vector2Int offset = draggedItem.GridOffset(difference);
            
            if (hoveredSlot == previousHoveredSlot && previousDifference == difference) return;

            List<Vector2Int> positions = new List<Vector2Int>();
            for (int y = 0; y < draggedItem.UIItem.InvItem.Size.y; y++)
            {
                for (int x = 0; x < draggedItem.UIItem.InvItem.Size.x; x++)
                {
                    positions.Add(new Vector2Int(
                        x + (hoveredSlot.slotPosition.x - offset.x), 
                        y + (hoveredSlot.slotPosition.y - offset.y)));
                }
            }

            // HIGHLIGHT SYSTEM IS STUPID, CHANGE.
            currentHighlightSlots = hoveredSlot.UIGrid.GetSlotsFromPositions(positions);
            
            if (currentHighlightSlots == null) return;

            foreach (UIInventorySlot inventorySlot in currentHighlightSlots)
            {
                if(inventorySlot.containedItem != null || currentHighlightSlots.Length != positions.Count)
                {
                    foreach (UIInventorySlot slot in currentHighlightSlots)
                    {
                        slot.HighlightSlot(Color.red);
                    }
                    break;
                }
                
                if(hoveredSlot != null)
                    inventorySlot.HighlightSlot(Color.green);
                
            }
            
            previousHoveredSlot = hoveredSlot;
        }

        private void EndDrag()
        {
            dragging = false;
            previousHoveredSlot = null;
            
            // If slots were highlighted, return to original state TODO: Improve Implementation.
            if (currentHighlightSlots is { Length: > 0 })
            {
                foreach (UIInventorySlot currentHighlightSlot in currentHighlightSlots)
                {
                    currentHighlightSlot.HighlightSlot(new Color(0, 0, 0, 0.19f));
                }
            }
            
            if (hoveredSlot != null)
            {
                Vector2 difference = Input.mousePosition - hoveredSlot.transform.position;
                if (hoveredSlot.UIGrid.AddUIItem(draggedItem.UIItem, hoveredSlot.slotPosition - draggedItem.GridOffset(difference)))
                {
                    draggedItem = null;
                    return;
                }
            }

            // TODO: REDUNDANCY IF ITEM CAN'T BE PLACED, i.e. Drop world item or move to new position in grid.
            if (draggedItem.UIItem.InvItem.ItemRuntimeData.rotated != draggedItem.OriginalRot)
            {
                draggedItem.UIItem.InvItem.ItemRuntimeData.rotated = draggedItem.OriginalRot;
            }
            draggedItem.UIItem.UIGrid.AddUIItem(draggedItem.UIItem, draggedItem.OriginalPos);

            draggedItem = null;

        }

        #endregion

        #region Rotation

        private void UpdateRotation()
        {
            if (!dragging) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                draggedItem.UIItem.InvItem.ItemRuntimeData.rotated =
                    !draggedItem.UIItem.InvItem.ItemRuntimeData.rotated;
                draggedItem.UIItem.UpdateItem();
                draggedItem.UIItem.transform.position = Input.mousePosition;
                previousHoveredSlot = null;
            }
        }

        #endregion

        #region Context Menu

        private void UpdateContextMenu()
        {
            if (dragging)
            {
                RemoveContextMenu();
                return;
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                CreateContextMenu(hoveredSlot, Input.mousePosition);
            }
        }

        public void CreateContextMenu(UIInventorySlot slot, Vector2 clickPos)
        {
            if (draggedItem != null) return;
            
            if (currentContextMenu != null)
            {
                Destroy(currentContextMenu.gameObject);
            }
            
            if (slot == null || slot.containedItem == null) return;
            
            GameObject contextMenuObj = Instantiate(slot.UIGrid.Style.menuObj, transform);

            // Rebuild Layout to Correctly Set Menu Position
            LayoutRebuilder.ForceRebuildLayoutImmediate(contextMenuObj.GetComponent<RectTransform>());

            contextMenuObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            contextMenuObj.transform.position = clickPos;

            UIInventoryContextMenu contextMenu = contextMenuObj.AddComponent<UIInventoryContextMenu>();

            contextMenu.invUIItem = slot.containedItem;

            currentContextMenu = contextMenu;
        }

        public void RemoveContextMenu()
        {
            if (currentContextMenu == null) return;
            
            Destroy(currentContextMenu.gameObject);
            Destroy(currentContextMenu);
        }

        #endregion

        #region Containers

        public void ContainerOpened()
        {
            // Guard Clauses
            if (currentContextMenu == null || currentContextMenu.invUIItem == null) return;
            UIInventoryItem containerUIItem = currentContextMenu.invUIItem;
            
            if (containerUIItem.InvItem.Item is not Container container) return;
            
            if(openContainers.ContainsKey(containerUIItem.InvItem)) return;
            

            // Getting current window manager instance.
            UIWindowManager windowManager;
            if (UIWindowManager.Instance != null) windowManager = UIWindowManager.Instance;
            else return;

            // Get new window's position
            UIWindow lastContainer = openContainers.LastOrDefault().Value;
            Vector2 newContainerPos = new Vector2(Screen.width / 2, Screen.height / 2);
            if (lastContainer != null)
                newContainerPos = lastContainer.GetComponent<RectTransform>().anchoredPosition + new Vector2(20, -20);
            
            // Creating new window.
            UIWindow window = windowManager.CreateWindow("Container", newContainerPos);
            
            ContainerItemRuntimeData itemData = (ContainerItemRuntimeData)containerUIItem.InvItem.ItemRuntimeData;

            GameObject gridObj = Instantiate(containerUIItem.UIGrid.Style.gridObj, window.Content);
            if (!gridObj.TryGetComponent(out UIInventoryGrid containerUIGrid))
                containerUIGrid = gridObj.AddComponent<UIInventoryGrid>();

            gridObj.transform.position = newContainerPos;
            
            // Setting Up Backend
            itemData.containerGrid ??= new InventoryGrid(container.itemStorageSize);
            itemData.containerGrid.ItemBlacklist.Add(containerUIItem.InvItem);
            containerUIGrid.AssignGrid(itemData.containerGrid, true);

            openContainers.Add(containerUIItem.InvItem, window);
        }

        public void ContainerUpdated()
        {
            
        }

        public void ContainerRemoved(UIWindow containerWindow)
        {
            // Getting Container Runtime
            InventoryItem containerItem = openContainers.Where(
                container => container.Value == containerWindow).Select(
                container => container.Key).FirstOrDefault();

            ContainerItemRuntimeData containerItemData = (ContainerItemRuntimeData)containerItem.ItemRuntimeData;

            if (containerItemData == null) return;
            
            // Return Held Item to Container if Held Item's original grid is being closed.
            if(draggedItem != null && draggedItem.UIItem.UIGrid.Grid == containerItemData.containerGrid)
            {
                draggedItem.UIItem.UIGrid.Grid.AddItemAtPosition(draggedItem.UIItem.InvItem, draggedItem.UIItem.InvItem.TakenSlots[0]);
                draggedItem = null;
            }

            openContainers.Remove(containerItem);

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

        #endregion

        #region Inspect Menu

        

        #endregion

        #region Interactions

        private void OnSlotEntered(UIInventorySlot slot)
        {
            hoveredSlot = slot;
        }

        private void OnSlotExited(UIInventorySlot slot)
        {
            if (hoveredSlot == slot) hoveredSlot = null;
        }

        #endregion

        #region Event Subscription

        private void Subscriptions()
        {
            UIInventorySlot.MouseEnter += OnSlotEntered;
            UIInventorySlot.MouseExit += OnSlotExited;

            openAction.Interact += ContainerOpened;
            UIWindow.Closed += ContainerRemoved;
        }
        
        private void RemoveSubscriptions()
        {
            UIInventorySlot.MouseEnter -= OnSlotEntered;
            UIInventorySlot.MouseExit -= OnSlotExited;
            
            openAction.Interact -= ContainerOpened;
            UIWindow.Closed -= ContainerRemoved;
        }

        #endregion
        
        // TODO: Context Menu, Inspect Menu

        #endregion
    }
    
    internal class ItemDragData // Used to store data related to the item being dragged.
    {
        public readonly UIInventoryItem UIItem;
        
        // Offsets
        public readonly Vector2Int OriginalPos;
        public readonly bool OriginalRot;
        public Vector2 ItemCentre => new ()
        {
            x = UIItem.InvItem.Size.x / 2 - .5f,
            y = UIItem.InvItem.Size.y / 2 - .5f
        };

        public Vector2Int GridOffset(Vector2 difference)
        {
            int xOffset;
            int yOffset;

            if (UIItem.InvItem.Size.x % 2 == 0)
            {
                xOffset = difference.x < 0
                    ? Mathf.CeilToInt(ItemCentre.x)
                    : Mathf.FloorToInt(ItemCentre.x);
            }
            else
            {
                xOffset = Mathf.CeilToInt(ItemCentre.x);
            }
            
            if (UIItem.InvItem.Size.y % 2 == 0)
            {
                yOffset = difference.y > 0
                    ? Mathf.CeilToInt(ItemCentre.y)
                    : Mathf.FloorToInt(ItemCentre.y);
            }
            else
            {
                yOffset = Mathf.CeilToInt(ItemCentre.y);
            }

            
            return new Vector2Int(xOffset, yOffset);
        }
        
        public ItemDragData(UIInventoryItem uiItem, Vector2Int originalPos, bool originalRot)
        {
            UIItem = uiItem;
            OriginalPos = originalPos;
            OriginalRot = originalRot;
        }
    }

}