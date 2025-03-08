using Hitbox.Stash.UI.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Stash.UI.ContextMenu
{
    public class InventoryUIContextButton : MonoBehaviour
    {
        #region Fields

        // Components
        public TextMeshProUGUI label;
        public Button btn;
        public Image icon;

        /// <summary>
        /// Action that this button is linked to.
        /// </summary>
        public InventoryUIItemAction action;
        
        /// <summary>
        /// Parent context menu of this button.
        /// </summary>
        public InventoryUIContextMenu parentMenu;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            if (parentMenu == null)
                Destroy(gameObject);

            if (btn == null && !TryGetComponent(out btn))
            {
                parentMenu.Remove();
                return;
            }

            if (icon != null)
            {
                icon.sprite = action.icon;
            }

            if (label != null)
            {
                label.text = action.name;
            }

            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            action.Invoke(parentMenu.invItem);
            parentMenu.Remove();        
        }

        #endregion
    }

}