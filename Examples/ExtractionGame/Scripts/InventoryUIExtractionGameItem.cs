using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash.Items;
using Hitbox.Stash.UI;
using TMPro;
using UnityEngine;

public class InventoryUIExtractionGameItem : InventoryUIItem
{
    #region Fields

    [Header("Tarkov Data")] 
    [SerializeField] TextMeshProUGUI amountDisplay;
    [SerializeField] TextMeshProUGUI calibreDisplay;

    #endregion

    #region Methods

    public override void UpdateDisplay()
    {
        base.UpdateDisplay();

        if (amountDisplay == null) return;

        if (InvItem is InventoryStackableItem stackableItem)
        {
            Color textColor = InvItem.ItemProfile is CurrencyItemProfile ? Color.green : Color.white;

            amountDisplay.enabled = true;
            amountDisplay.color = textColor;
            amountDisplay.text = stackableItem.CurrentStackAmount.ToString();
        }
        else
        {
            amountDisplay.enabled = false;
        }
        
    }

    #endregion
}
