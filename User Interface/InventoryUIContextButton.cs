using KoalaDev.UGIS.Interactions;
using KoalaDev.UGIS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUIContextButton : MonoBehaviour, IPointerClickHandler
{
    #region --- VARIABLES ---

    public TextMeshProUGUI label;
    public InventoryInteraction action;
    public InventoryUIContextMenu parentMenu;

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        action.Invoke();
        parentMenu.RemoveMenu();
    }
}
