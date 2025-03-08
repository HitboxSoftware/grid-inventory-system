using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hitbox.Stash.UI
{
    [DefaultExecutionOrder(-1000)]
    public class InventoryUIManager : MonoBehaviour
    {
        #region Fields
        
        private static InventoryUIManager _instance;
        public static InventoryUIManager Instance => _instance;
        

        // Inventory Style
        public InventoryUIStyle inventoryStyle;
        
        // Drag n Drop
        [Header("Drag n Drop Options")]
        [Tooltip("Manages drag visuals.")]
        [SerializeField] InventoryUIDragContainer dragContainer;
        
        [Tooltip("Should item be rotatable whilst dragging.")]
        [SerializeField] protected bool canRotateDraggedItem;
        
        [Tooltip("Should it be hold to drag or click to drag.")]
        [SerializeField] protected bool toggleableDragging;

        [Tooltip("Should the cursor be hidden whilst dragging.")]
        [SerializeField] protected bool hideCursorDuringDrag = true;
        
        [SerializeField] protected Canvas canvas;

        // - Runtime
        // Drag Data
        public bool IsDragging { protected set; get; }
        public InventoryItemDragData CurrentDragData { protected set; get; }
        private float _dragStartTime;
        
        // Hover Data
        public (Vector2Int gridPos, InventoryUIAbstractGrid uiGrid) HoveredSlot { get; protected set; }
        public InventoryUIItemSlot HoveredItemSlot { get; protected set; }

        public event Action<InventoryItemDragData> OnDragStart;
        public event Action OnDragEnd;

        public event Action<bool> InventoryToggled;
        
        #endregion

        #region MonoBehaviour
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"Warning: {gameObject.name} ({name}) is being removed as an InventoryUIManager instance is already defined ({_instance.gameObject.name}).");
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        protected virtual void Update()
        {
            DetectDrag();
            UpdateRotation();
        }

        protected virtual void OnEnable()
        {
            SubscribeEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        #region Methods

        public virtual void ToggleInventory(bool active)
        {
            canvas.enabled = active;

            if (active && IsDragging)
            {
                EndDrag();
            }
            
            InventoryToggled?.Invoke(active);
        }

        #region Drag & Drop
        
        /// <summary>
        /// Detect and manage current drag activity.
        /// </summary>
        protected virtual void DetectDrag()
        { 
            //TODO: InputSystem support.
            
            if (IsDragging && Time.realtimeSinceStartup > _dragStartTime)
            {
                // End Toggle Drag
                if (toggleableDragging && Input.GetMouseButtonDown(0))
                {
                    EndDrag();
                }

                // End Hold Drag
                if (!toggleableDragging && !Input.GetMouseButton(0))
                    EndDrag();
            }
            else if(Input.GetMouseButtonDown(0)) // Try Start Drag
            {
                StartDrag();
            }

            if(hideCursorDuringDrag)
                Cursor.visible = !IsDragging;
        }
        
        protected virtual bool StartDrag()
        {
            _dragStartTime = Time.realtimeSinceStartup;
            
            if (HoveredItemSlot != null && HoveredItemSlot.LinkedSlot.IsItemAttached())
            {
                InventoryItem equipmentItem = HoveredItemSlot.LinkedSlot.DetachItem();

                if (equipmentItem != null)
                {
                    IsDragging = true;
                    CurrentDragData = new InventoryItemDragData(equipmentItem, HoveredItemSlot.LinkedSlot);
                    
                    dragContainer.SetDraggedItem(CurrentDragData);
                    
                    OnDragStart?.Invoke(CurrentDragData);
                    
                    return true;
                }
            }
            
            if (HoveredSlot.uiGrid == null || !HoveredSlot.uiGrid.Grid.ItemAtPosition(HoveredSlot.gridPos)) return false;

            if (!HoveredSlot.uiGrid.TryGetUIItemAtPosition(HoveredSlot.gridPos, out InventoryUIItem invUIItem))
            {
                return false;
            }
            
            // Dragging Logic
            IsDragging = true;
            CurrentDragData = new InventoryItemDragData(invUIItem.InvItem, invUIItem.InvItem.ParentContainer, invUIItem.InvItem.Index, invUIItem.InvItem.Rotated);            
            dragContainer.SetDraggedItem(CurrentDragData);
            
            invUIItem.UIGrid.Grid.RemoveItem(invUIItem.InvItem);
            
            OnDragStart?.Invoke(CurrentDragData);

            return true;
        }

        protected virtual void EndDrag()
        {
            IsDragging = false;
            OnDragEnd?.Invoke();
            
            // Clearing UI
            dragContainer.ClearDraggedItem();
            
            // Clearing Current Drag Data Reference
            InventoryItemDragData dragData = CurrentDragData; // Temporary reference to drag data, so current can be cleared.
            CurrentDragData = null;

            // Ensuring that the dragged item exists.
            if (dragData.InvItem == null) return;

            // Attempting to insert into hovered equipment slot.
            if (HoveredItemSlot != null)
            {
                if (HoveredItemSlot.LinkedSlot.InsertItem(dragData.InvItem, true)) return;
            }

            // Attempting to insert into hovered grid.
            if (HoveredSlot.uiGrid != null && HoveredSlot.uiGrid.Grid != null)
            {
                Vector2 difference = (Vector2)Input.mousePosition - HoveredSlot.uiGrid.GridToCanvasPoint(HoveredSlot.gridPos, new(0.5f, 0.5f));
                
                if (HoveredSlot.uiGrid.Grid.TryGetItemAtPosition(HoveredSlot.gridPos,
                        out InventoryItem foundInvItem) && foundInvItem != dragData.InvItem)
                {
                    bool combined = foundInvItem.TryCombineItem(dragData.InvItem);
                    if (combined) return;
                }
                else if(HoveredSlot.uiGrid.Grid.InsertItemAtPosition(dragData.InvItem, HoveredSlot.gridPos - dragData.CalculateGridOffset(difference), false))
                {
                    return;
                }
            }
            
            // Returning the item to original rotation.
            if (!dragData.InvItem.Rotated == dragData.OriginalRot)
            {
                dragData.InvItem.Rotated = dragData.OriginalRot;
            }
            
            // Try to return to previous container
            if (dragData.ParentContainer != null)
            {
                // Try and insert at previous item position.
                if (dragData.ParentContainer.InsertItemAtIndex(dragData.InvItem, dragData.OriginalIndex)) return;

                // Try and insert at any available position in grid, without combining.
                if (dragData.ParentContainer.InsertItem(dragData.InvItem)) return;
                
                // Try and insert into any available combination
                if (dragData.ParentContainer.InsertItem(dragData.InvItem, true)) return;
            }
            
            Debug.LogError("Error: Unable to find place for dragged item, item has been deleted :(");
            
            // If we've got this far, something's gone very wrong.
            // Currently just going to delete the item, but a temporary storage of some kind would be
            // a good idea.
        }

        #endregion

        #region Rotation

        private void UpdateRotation()
        {
            if (!canRotateDraggedItem) return;
            if (!IsDragging) return;

            //TODO: Add InputSystem support.
            if (Input.GetKeyDown(KeyCode.R))
            {
                CurrentDragData.InvItem.Rotated = !CurrentDragData.InvItem.Rotated;
            }
        }

        #endregion

        public virtual bool PickupItem(InventoryItem item)
        {
            Debug.LogError("Item Pickups have not been implemented!!!");
            return false;
        }

        #region Event Subscription
        
        protected virtual void SubscribeEvents()
        {
            InventoryUIItemSlot.MouseEnter += OnItemSlotMouseEnter;
            InventoryUIItemSlot.MouseExit += OnItemSlotMouseExit;
            
            InventoryUIAbstractGrid.GridSlotMouseEnter += OnGridSlotMouseEnter;
            InventoryUIAbstractGrid.GridSlotMouseExit += OnGridSlotMouseExit;
        }

        protected virtual void UnsubscribeEvents()
        {
            InventoryUIItemSlot.MouseEnter -= OnItemSlotMouseEnter;
            InventoryUIItemSlot.MouseExit -= OnItemSlotMouseExit;

            InventoryUIAbstractGrid.GridSlotMouseEnter -= OnGridSlotMouseEnter;
            InventoryUIAbstractGrid.GridSlotMouseExit -= OnGridSlotMouseExit;
        }
        
        private void OnGridSlotMouseEnter(InventoryUIAbstractGrid grid, Vector2Int pos)
        {
            HoveredSlot = (pos, grid);
        }

        private void OnGridSlotMouseExit(InventoryUIAbstractGrid grid)
        {
            if (HoveredSlot.uiGrid != grid) return;
            HoveredSlot = default;
        }
        
        // Item Slots
        private void OnItemSlotMouseEnter(InventoryUIItemSlot itemSlot)
        {
            HoveredItemSlot = itemSlot;
        }
        
        private void OnItemSlotMouseExit(InventoryUIItemSlot itemSlot)
        {
            if (HoveredItemSlot == itemSlot) HoveredItemSlot = null;
        }
        
        #endregion
        
        #endregion
    }

    public class InventoryItemDragData // Used to store data related to the item being dragged.
    {
        public readonly InventoryItem InvItem;
        public readonly InventoryContainer ParentContainer;

        // Offsets
        public readonly int OriginalIndex;
        public readonly bool OriginalRot;

        private Vector2 ItemCentre => new ()
        {
            x = InvItem.Size.x / 2 - .5f,
            y = InvItem.Size.y / 2 - .5f
        };

        public Vector2Int CalculateGridOffset(Vector2 difference)
        {
            int xOffset;
            int yOffset;

            if (InvItem.Size.x % 2 == 0)
            {
                xOffset = difference.x < 0
                    ? Mathf.CeilToInt(ItemCentre.x)
                    : Mathf.FloorToInt(ItemCentre.x);
            }
            else
            {
                xOffset = Mathf.CeilToInt(ItemCentre.x);
            }
            
            if (InvItem.Size.y % 2 == 0)
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
        
        public InventoryItemDragData(InventoryItem invItem, InventoryContainer parentContainer, int originalIndex = default, bool originalRot = false)
        {
            this.InvItem = invItem;
            this.OriginalIndex = originalIndex;
            this.OriginalRot = originalRot;
            this.ParentContainer = parentContainer;
        }
    }

}