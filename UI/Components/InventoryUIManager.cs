using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hitbox.Inventory.Items;
using Hitbox.Inventory.UI.Actions;
using Hitbox.Inventory.UI.ContextMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Inventory.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        #region --- VARIABLES ---
        
        public InventoryUIStyle inventoryStyle;

        [SerializeField] private InventoryUIDragContainer dragContainer;

        // --- Runtime ---
        
        // Hover
        private InventoryUISlot _hoveredSlot;
        private InventoryUIItemSlot _hoveredItemSlot;
        private InventoryUISlot _previousHoveredSlot;
        
        // Drag n Drop
        private bool _dragging;
        private InventoryItemDragData _currentDragData;
        private Vector2 _previousDifference;
        
        // Item Highlighting
        private InventoryUISlot[] _currentHighlightSlots;
        
        // Context Menu
        private InventoryUIContextMenu _currentContextMenu;

        // Containers
        private readonly Dictionary<InventoryContainerItem, UIWindow> _openContainers = new();

        [SerializeField] private AbstractInventoryUI[] managedInventories;
        
        // --- Events ---
        [Header("Item Actions")]
        public InventoryUIItemAction openAction;
        public InventoryUIItemAction equipAction;
        public InventoryUIItemAction inspectAction;
        public InventoryUIItemAction[] itemActions;

        //TEMP
        [SerializeField] private List<Item> startingItems;
        [SerializeField] private InventoryUIAbstractGrid startingGrid;
        
        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            InventoryUIGlobal.SetInstance(this);
        }

        private IEnumerator Start()
        {
            yield return 0;
            if (startingGrid.Grid == null)
            {
                startingGrid.Assign(new InventoryGrid(new Vector2Int(10, 60)), true);
            }
            
            foreach (Item invItem in startingItems)
            {
                startingGrid.Grid.InsertItem(invItem.CreateItem);
            }
        }

        private void Update()
        {
            DetectDrag();
            UpdateRotation();
            UpdateContextMenu();
        }

        private void OnEnable()
        {
            Subscriptions();
        }

        #endregion

        #region --- METHODS ---

        #region Drag & Drop
        
        /// <summary>
        /// Detect and manage current drag activity.
        /// </summary>
        private void DetectDrag()
        {
            //TODO: InputSystem support.
            if (Input.GetMouseButtonDown(0))
            {
                if (_dragging) EndDrag();

                StartDrag();
            }

            if (_dragging && !Input.GetMouseButton(0))
                EndDrag();

            Cursor.visible = !_dragging;
        }
        
        private void StartDrag()
        {
            UIWindow container;
            if (_hoveredItemSlot != null && _hoveredItemSlot.LinkedSlot.HasItem())
            {
                InventoryItem equipmentItem = _hoveredItemSlot.LinkedSlot.DetachItem();

                if (equipmentItem != null)
                {
                    if (equipmentItem is InventoryContainerItem containerInventoryItem)
                    {
                        // Closing the container item's linked UI window, if open.
                        if (_openContainers.TryGetValue(containerInventoryItem, out container))
                        {
                            container.Close();
                        }
                        
                        // Closing Container Windows that are children of current window being closed, *this is recursive*.
                        List<InventoryContainerItem> childContainers = _openContainers.Keys.Where(item =>
                            ((InventoryContainerItem)equipmentItem).gridGroup.ContainsItem(item)).ToList();
            
                        for (int i = childContainers.Count - 1; i >= 0; i--)
                        {
                            if(_openContainers[childContainers[i]] != null)
                                _openContainers[childContainers[i]].Close();
                
                            _openContainers.Remove(childContainers[i]);
                        }
                    
                        if (_openContainers.TryGetValue(containerInventoryItem, out UIWindow openContainer))
                        {
                            openContainer.Close();
                        }
                    }

                    _dragging = true;
                    _currentDragData = new InventoryItemDragData(equipmentItem, _hoveredItemSlot.LinkedSlot);
                    
                    dragContainer.SetDraggedItem(_currentDragData);
                    
                    return;
                }
            }
            
            if (_hoveredSlot == null || _hoveredSlot.containedItem == null) return;
            
            // If the item is a container, check if a UI Window is linked and close it.
            if (_hoveredSlot.containedItem.InvItem is InventoryContainerItem invItem &&
                _openContainers.TryGetValue(invItem, out container))
            {
                container.Close();
            } 
            
            // Dragging Logic
            _dragging = true;

            InventoryUIItem invUIItem  = _hoveredSlot.containedItem;
            _currentDragData = new InventoryItemDragData(invUIItem.InvItem, invUIItem.InvItem.ParentContainer, invUIItem.InvItem.takenPositions[0], invUIItem.InvItem.rotated);            
            dragContainer.SetDraggedItem(_currentDragData);
            
            //invUIItem.UIGrid.Grid.RemoveItem(invUIItem.InvItem);
        }
        
        private void Drag()
        {
            if (!_dragging || _currentDragData == null) return;
            
            if (_hoveredSlot == null)
            {
                return;
            }
        }

        private void EndDrag()
        {
            _dragging = false;
            _previousHoveredSlot = null;
            
            // Clearing UI
            dragContainer.ClearDraggedItem();
            
            // Clearing Current Drag Data Reference
            InventoryItemDragData dragData = _currentDragData; // Temporary reference to drag data, so current can be cleared.
            _currentDragData = null;
            
            // If slots were highlighted, return to original state TODO: Improve Implementation.
            if (_currentHighlightSlots is { Length: > 0 })
            {
                foreach (InventoryUISlot currentHighlightSlot in _currentHighlightSlots)
                {
                    currentHighlightSlot.HighlightSlot(_hoveredSlot.UIGrid.Style.slotColour);
                }
            }
            
            // Ensuring that the dragged item exists.
            if (dragData.invItem == null) return;

            // Attempting to insert into hovered equipment slot.
            if (_hoveredItemSlot != null)
            {
                if (_hoveredItemSlot.LinkedSlot.InsertItem(dragData.invItem, true)) return;
            }

            // Attempting to insert into hovered grid.
            if (_hoveredSlot != null)
            {
                Vector2 difference = Input.mousePosition - _hoveredSlot.transform.position;

                if (_hoveredSlot.UIGrid.Grid.GetItemAtPosition(_hoveredSlot.slotPosition) is { } foundInvItem &&
                    foundInvItem != dragData.invItem)
                {
                    (bool combined, _) = foundInvItem.item.TryCombineItems(dragData.invItem, foundInvItem);
                    if (combined) return;
                }
                
                if (_hoveredSlot.UIGrid.Grid.InsertItemAtPosition(dragData.invItem, _hoveredSlot.slotPosition - dragData.GridOffset(difference), false))
                {
                    return;
                }
            }
            
            // Returning the item to original rotation.
            if (!dragData.invItem.rotated == dragData.originalRot)
            {
                dragData.invItem.rotated = dragData.originalRot;
            }
            
            // Try to return to previous container
            if (dragData.parentContainer != null)
            {
                // Try and insert at previous item position.
                if (dragData.parentContainer.InsertItemAtPosition(dragData.invItem, dragData.originalPos, false)) return;

                // Try and insert at any available position in grid, without combining.
                if (dragData.parentContainer.InsertItem(dragData.invItem)) return;
                
                // Try and insert into any available combination
                if (dragData.parentContainer.InsertItem(dragData.invItem, true)) return;
            }
            
            Debug.LogError("Error: Unable to find place for dragged item, item has been deleted :(");
            
            // If we've got this far, something's gone very wrong.
            // Currently just going to delete the item, but a temporary storage of some kind would be
            // a good idea I think.
        }

        #endregion

        #region Rotation

        private void UpdateRotation()
        {
            if (!_dragging) return;

            //TODO: Add InputSystem support.
            if (Input.GetKeyDown(KeyCode.R))
            {
                _currentDragData.invItem.rotated = !_currentDragData.invItem.rotated;
                
                _previousHoveredSlot = null;
            }
        }

        #endregion

        #region Context Menu

        private void UpdateContextMenu()
        {
            //TODO: Input system support.
            if (_dragging)
            {
                RemoveContextMenu();
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                RemoveContextMenu();
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                if (_hoveredSlot != null && _hoveredSlot.containedItem != null)
                {
                    CreateContextMenu(_hoveredSlot.containedItem.InvItem, Input.mousePosition);
                } else if (_hoveredItemSlot != null && _hoveredItemSlot.LinkedSlot.AttachedItem != null)
                {
                    CreateContextMenu(_hoveredItemSlot.LinkedSlot.AttachedItem, Input.mousePosition);
                }
                else
                {
                    RemoveContextMenu();
                    return;
                }
            }
        }

        private void CreateContextMenu(InventoryItem invUIItem, Vector2 clickPos)
        {
            if (_currentDragData != null) return;
            
            if (_currentContextMenu != null)
            {
                Destroy(_currentContextMenu.gameObject);
            }
            
            if (invUIItem == null) return;
            
            GameObject contextMenuObj = Instantiate(inventoryStyle.menuObj, transform);

            // Rebuild Layout to Correctly Set Menu Position
            LayoutRebuilder.ForceRebuildLayoutImmediate(contextMenuObj.GetComponent<RectTransform>());

            contextMenuObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            contextMenuObj.transform.position = clickPos;

            InventoryUIContextMenu contextMenu = contextMenuObj.AddComponent<InventoryUIContextMenu>();
            
            contextMenu.invItem = invUIItem;
            contextMenu.actions = itemActions;

            contextMenu.SetStyle(inventoryStyle);

            _currentContextMenu = contextMenu;
        }

        public void RemoveContextMenu()
        {
            if (_currentContextMenu == null) return;
            
            Destroy(_currentContextMenu.gameObject);
            Destroy(_currentContextMenu);
        }

        #endregion

        #region Containers

        private void ContainerOpened(InventoryItem invItem)
        {
            // Guard Clauses
            if (invItem is not InventoryContainerItem containerItem) return;
            if (containerItem.item is not ContainerItem container) return;
            if (_openContainers.ContainsKey(containerItem)) return;


            // Getting current window manager instance.
            UIWindowManager windowManager;
            if (UIWindowManager.Instance != null) windowManager = UIWindowManager.Instance;
            else return;

            /* TODO: Add function to get related item's position 
             * Set new window's position
             * Vector2 newContainerPos = containerUIItem.transform.position;
            */

            // Creating new window.
            //TODO: Set window position to "newContainerPos" once it's correctly set.
            UIWindow window = windowManager.CreateWindow(container.name, Input.mousePosition, new Vector2(0.5f, 0.5f));

            GameObject gridObj = new("ContainerGrid");

            // Setting parent, ensuring that the object doesn't get rescaled.
            gridObj.transform.SetParent(window.Content, false);

            if (!gridObj.TryGetComponent(out InventoryUIGridGroup containerUIGrid))
            {
                containerUIGrid = gridObj.AddComponent<InventoryUIGridGroup>();

                containerUIGrid.style = inventoryStyle;
            }

            // Setting Up Backend
            containerItem.gridGroup ??=
                new InventoryGridGroup(container.gridSizes, new InventoryItem[] { containerItem });

            containerUIGrid.SetGrids(containerItem.gridGroup, container.rowCapacities, true);

            _openContainers.Add(containerItem, window);
            window.Closed += _ => containerUIGrid.ClearGrids();
            window.Closed += ContainerRemoved;
        }

        private void ContainerRemoved(UIWindow containerWindow)
        {
            // Getting Container Runtime
            InventoryContainerItem containerInvItem = _openContainers.Where(
                container => container.Value == containerWindow).Select(
                container => container.Key).FirstOrDefault();

            if (containerInvItem == null) return;


            //TODO: Return dragged item to container if dragged item's original grid is being closed.

            // Closing Container Windows that are children of current window being closed, *this is recursive*.
            // TODO: Had stack overflow from this, if all is well this won't happen, but implement checks just in case.
            List<InventoryContainerItem> childContainers =
                _openContainers.Keys.Where(item => containerInvItem.gridGroup.ContainsItem(item)).ToList();

            for (int i = childContainers.Count - 1; i >= 0; i--)
            {
                if (_openContainers[childContainers[i]] != null)
                    _openContainers[childContainers[i]].Close();

                _openContainers.Remove(childContainers[i]);
            }

            _openContainers.Remove(containerInvItem);
        }

        #endregion

        #region Inspect Menu

        private void CreateInspectMenu(InventoryItem invUIItem)
        {
            // Getting current window manager instance.
            UIWindowManager windowManager;
            if (UIWindowManager.Instance != null) windowManager = UIWindowManager.Instance;
            else return;

            /* TODO: Add function to get related item's position 
             * Set new window's position
             * Vector2 newContainerPos = containerUIItem.transform.position;
            */

            // Creating new window.
            //TODO: Set window position to "newContainerPos" once it's correctly set.
            UIWindow window = windowManager.CreateWindow(invUIItem.item.name, Input.mousePosition, new Vector2(0.5f, 0.5f));
            
        }

        #endregion

        #region Actions

        public void TryEquipItem(InventoryItem item)
        {
            foreach (AbstractInventoryUI managedInventory in managedInventories)
            {
                if (managedInventory.TryEquipItem(item))
                {
                    return;
                }
            }
        }

        #endregion

        #region Event Subscription
        
        protected void Subscriptions()
        {
            //TODO: Maybe try and compress equipment and grid slot events into one?
            InventoryUISlot.MouseEnter += OnGridSlotMouseEnter;
            InventoryUISlot.MouseExit += OnGridSlotMouseExit;

            InventoryUIItemSlot.MouseEnter += OnEquipmentSlotMouseEnter;
            InventoryUIItemSlot.MouseExit += OnEquipmentSlotMouseExit;
            
            openAction.Interact += ContainerOpened;
            equipAction.Interact += TryEquipItem;
        }
        
        // Grid Slots
        private void OnGridSlotMouseEnter(InventoryUISlot slot)
        {
            _hoveredSlot = slot;
        }

        private void OnGridSlotMouseExit(InventoryUISlot slot)
        {
            if (_hoveredSlot == slot) _hoveredSlot = null;
        }
        
        // Equipment Slots
        private void OnEquipmentSlotMouseEnter(InventoryUIItemSlot itemSlot)
        {
            _hoveredItemSlot = itemSlot;
        }
        
        private void OnEquipmentSlotMouseExit(InventoryUIItemSlot itemSlot)
        {
            if (_hoveredItemSlot == itemSlot) _hoveredItemSlot = null;
        }
        
        #endregion
        
        #endregion
    }

    public class InventoryItemDragData // Used to store data related to the item being dragged.
    {
        public readonly InventoryItem invItem;
        public readonly InventoryContainer parentContainer;

        // Offsets
        public readonly Vector2Int originalPos;
        public readonly bool originalRot;

        private Vector2 ItemCentre => new ()
        {
            x = invItem.Size.x / 2 - .5f,
            y = invItem.Size.y / 2 - .5f
        };

        public Vector2Int GridOffset(Vector2 difference)
        {
            int xOffset;
            int yOffset;

            if (invItem.Size.x % 2 == 0)
            {
                xOffset = difference.x < 0
                    ? Mathf.CeilToInt(ItemCentre.x)
                    : Mathf.FloorToInt(ItemCentre.x);
            }
            else
            {
                xOffset = Mathf.CeilToInt(ItemCentre.x);
            }
            
            if (invItem.Size.y % 2 == 0)
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
        
        public InventoryItemDragData(InventoryItem invItem, InventoryContainer parentContainer, Vector2Int originalPos = default, bool originalRot = false)
        {
            this.invItem = invItem;
            this.originalPos = originalPos;
            this.originalRot = originalRot;
            this.parentContainer = parentContainer;
        }
    }

}