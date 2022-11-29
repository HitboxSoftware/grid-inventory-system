using System;
using KoalaDev.UGIS;
using KoalaDev.UGIS.Items;
using KoalaDev.UGIS.Items.WeaponSystem;
using KoalaDev.UGIS.UI;
using KoalaDev.Utilities.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIItem : MonoBehaviour
{
    #region --- VARIABLES ---

    public InventoryItem InvItem;
    public InventoryUIGrid uiGrid;

    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemBackground;
    [SerializeField] private TextMeshProUGUI stackCounterLabel;

    private Color backgroundColor;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        Init();
        UpdateItem();
        
        InvItem.Item.OnUpdate += UpdateItem;
    }

    private void OnEnable()
    {
        if (InvItem != null)
        {
            InvItem.Item.OnUpdate += UpdateItem;
        }
    }

    private void OnDisable()
    {
        if (InvItem != null)
        {
            InvItem.Item.OnUpdate -= UpdateItem;
        }
    }

    #endregion

    #region --- METHODS ---

    private void Init()
    {
        if (itemIcon != null)
            itemIcon.sprite = InvItem.Item.icon;

        if (itemBackground == null)
            TryGetComponent(out itemBackground);

        if (itemBackground != null)
            backgroundColor = itemBackground.color;
    }

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

    public void UpdateItem()
    {
        UpdateStackCounter();
    }

    private void UpdateStackCounter()
    {
        if (stackCounterLabel == null) return;

        stackCounterLabel.text = InvItem.ItemData switch
        {
            StackableItemData itemData => itemData.currentStackCount.ToString(),
            MagazineItemData magazineData => magazineData.MagazineStack.Count.ToString(),
            _ => ""
        };
    }

    #endregion
}
