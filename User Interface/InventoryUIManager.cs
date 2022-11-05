using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KoalaDev.UGIS.UI
{
    public class InventoryUIManager : MonoBehaviour
    {
        #region --- VARIABLES ---

        public static InventoryUIManager Instance;
        
        private HeldItemData heldItem;

        [SerializeField] private Transform heldItemContainer;

        public List<InventoryGrid> managedGrids;

        public List<Item> startingItems;

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
                grid.AutoAddItem(new InventoryItem(startingItems[Random.Range(0, startingItems.Count)], null, grid));
                grid.AutoAddItem(new InventoryItem(startingItems[Random.Range(0, startingItems.Count)], null, grid));
                grid.AutoAddItem(new InventoryItem(startingItems[Random.Range(0, startingItems.Count)], null, grid));
            }
        }

        private void Start()
        {
            
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
        
        private void PlaceItem(InventoryUISlot slot) //Places Attached Item to Clicked Slot
        {
            if(!slot.grid.AddItemAtPosition(heldItem.UIItem, slot.slotPosition - heldItem.GridOffset)) return;
            
            heldItem = null;
        }

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