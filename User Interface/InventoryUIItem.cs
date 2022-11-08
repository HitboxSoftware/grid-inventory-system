using System;
using KoalaDev.UGIS;
using KoalaDev.UGIS.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIItem : MonoBehaviour
{
    #region --- VARIABLES ---

    public InventoryItem InvItem;
    public InventoryUIGrid uiGrid;

    [SerializeField] private Image inventoryIcon;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        if (inventoryIcon != null)
        {
            inventoryIcon.sprite = InvItem.Item.icon;
        }
    }

    #endregion
}
