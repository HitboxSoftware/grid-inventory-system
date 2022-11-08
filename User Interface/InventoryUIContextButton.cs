using System;
using KoalaDev.UGIS.Interactions;
using KoalaDev.UGIS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIContextButton : MonoBehaviour
{
    #region --- VARIABLES ---

    // Components
    public TextMeshProUGUI label;
    public Button btn;
    
    // Inventory Scripts
    public InventoryInteraction action;
    public InventoryUIContextMenu parentMenu;

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
        if(parentMenu == null)
            Destroy(gameObject);
        
        if (btn == null && !TryGetComponent(out btn))
            InventoryUIContextMenu.RemoveMenu();
        
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        action.Invoke();
        InventoryUIContextMenu.RemoveMenu();
    }

    #endregion
}
