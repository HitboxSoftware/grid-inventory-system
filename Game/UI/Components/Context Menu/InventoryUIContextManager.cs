using System;
using Hitbox.Stash.UI.Actions;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Stash.UI.ContextMenu
{
    public class InventoryUIContextManager : MonoBehaviour
    {
        #region Fields
        
        // - Components
        private InventoryUIManager _inventoryManager;
        
        // - Properties
        [Header("Context Menu Actions")]
        [Tooltip("Any actions to add to context menu, will only appear if action is supported by item.")]
        public InventoryUIItemAction[] itemActions;

        // - Runtime
        /// <summary>
        /// Currently active context menu.
        /// </summary>
        private InventoryUIContextMenu _currentContextMenu;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            if(!TryGetComponent(out _inventoryManager))
            {
                Debug.LogError($"{GetType().Name} ({gameObject.name}) requires an InventoryUIManager!", this);
            }
        }

        private void Update()
        {
            UpdateContextMenu();
        }

        #endregion

        #region Methods

        private void UpdateContextMenu()
        {
            //TODO: Input system support.
            if (_inventoryManager.IsDragging)
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
                if (_inventoryManager.HoveredSlot.uiGrid != null &&
                    _inventoryManager.HoveredSlot.uiGrid.Grid.TryGetItemAtPosition(_inventoryManager.HoveredSlot.gridPos,
                        out InventoryItem clickedItem))
                {
                    CreateContextMenu(clickedItem, Input.mousePosition);
                }
                else if (_inventoryManager.HoveredItemSlot != null && _inventoryManager.HoveredItemSlot.LinkedSlot.AttachedItem != null)
                {
                    CreateContextMenu(_inventoryManager.HoveredItemSlot.LinkedSlot.AttachedItem, Input.mousePosition);
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
            if (_inventoryManager.CurrentDragData != null) return;

            if (_currentContextMenu != null)
            {
                RemoveContextMenu();
            }

            if (invUIItem == null) return;

            GameObject contextMenuObj = Instantiate(_inventoryManager.inventoryStyle.menuObj, transform);

            // Rebuild Layout to Correctly Set Menu Position
            LayoutRebuilder.ForceRebuildLayoutImmediate(contextMenuObj.GetComponent<RectTransform>());

            contextMenuObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            contextMenuObj.transform.position = clickPos;

            InventoryUIContextMenu contextMenu = contextMenuObj.AddComponent<InventoryUIContextMenu>();

            contextMenu.invItem = invUIItem;
            contextMenu.actions = itemActions;

            contextMenu.SetStyle(_inventoryManager.inventoryStyle);

            _currentContextMenu = contextMenu;
        }

        public void RemoveContextMenu()
        {
            if (_currentContextMenu == null) return;

            _currentContextMenu.Remove();
        }

        #endregion

    }

}