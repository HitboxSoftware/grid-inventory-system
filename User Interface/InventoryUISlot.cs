using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KoalaDev.UGIS.UI
{
    public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region VARIABLES

        public InventoryUIGrid grid;
        public Vector2Int slotPosition;
        public InventoryUIItem containedItem;
        public Image itemIcon;

        #endregion

        #region UIEVENTS

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    InventoryUIManager.Instance.SlotClick(this);
                    break;
                case PointerEventData.InputButton.Right:
                    InventoryUIManager.Instance.CreateContextMenu(this, eventData.position);
                    break;
            }
        }

        #endregion
    }

}