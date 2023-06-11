using System.Collections.Generic;
using System.Linq;
using Hitbox.Inventory.UI.Actions;
using UnityEngine;

namespace Hitbox.Inventory.UI.ContextMenu
{
    public class InventoryUIContextMenu : MonoBehaviour
    {
        #region --- VARIABLES ---

        /// <summary>
        /// Inventory Item to generate context actions from.
        /// </summary>
        public InventoryItem invItem;
        
        /// <summary>
        /// All the actions to generate for the item, will only show actions that are supported by the item.
        /// </summary>
        public InventoryUIItemAction[] actions;


        // Style of the context menu, contains prefab for action buttons.
        private InventoryUIStyle _activeStyle;
        
        // Contains all generated context buttons.
        private List<InventoryUIContextButton> _contextButtons = new();

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            Generate();
        }

        #endregion

        #region --- METHODS ---
        
        private void Generate()
        {
            foreach (InventoryUIItemAction action in actions)
            {
                if (!action.SupportsAction(invItem)) continue;
                InventoryUIContextButton interactionBtn = Instantiate(_activeStyle.actionObj, transform).GetComponent<InventoryUIContextButton>();
                interactionBtn.action = action;
                interactionBtn.parentMenu = this;
                
                _contextButtons.Add(interactionBtn);
            }
        }
        
        // Clears all referenced items
        private void Clear()
        {
            if (_contextButtons == null) return;

            foreach (InventoryUIContextButton button in _contextButtons)
            {
                
            }
        }

        public bool SetStyle(InventoryUIStyle style, bool regenerate = false)
        {
            if (style == null) return false;
            _activeStyle = style;

            if (regenerate)
            {
                Generate();
            }

            return true;
        }

        public static void RemoveMenu()
        {
            if (InventoryUIGlobal.TryGetInstance(out InventoryUIManager manager))
            {
                manager.RemoveContextMenu();
            }
        }

        #endregion
    }
}