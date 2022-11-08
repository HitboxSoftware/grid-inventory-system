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

    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;

    private Color backgroundColor;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        if (itemIcon != null)
        {
            itemIcon.sprite = InvItem.Item.icon;
        }
        
        if (itemBackground == null)
            TryGetComponent(out itemBackground);

        if (itemBackground != null) 
            backgroundColor = itemBackground.color;
    }

    #endregion

    #region --- METHODS ---

    public void Highlight(Color color)
    {
        if (itemBackground == null) return;

        itemBackground.color = color;
    }

    public void RemoveHighlight()
    {
        if (itemBackground == null) return;

        itemBackground.color = backgroundColor;
    }

    #endregion
}
