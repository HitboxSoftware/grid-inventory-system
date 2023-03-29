using System;
using Hitbox.UGIS.Interactions;
using Hitbox.UGIS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hitbox.UGIS.UI.ContextMenu
{
    public class UIInventoryContextButton : MonoBehaviour
    {
        #region --- VARIABLES ---

        // Components
        public TextMeshProUGUI label;
        public Button btn;

        // Inventory Scripts
        public InventoryInteractionChannel action;
        public UIInventoryContextMenu parentMenu;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            Init();
        }

        #endregion

        #region --- METHODS ---

        private void Init()
        {
            if (parentMenu == null)
                Destroy(gameObject);

            if (btn == null && !TryGetComponent(out btn))
                UIInventoryContextMenu.RemoveMenu();

            btn.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            action.Invoke();
            UIInventoryContextMenu.RemoveMenu();
        }

        #endregion
    }

}