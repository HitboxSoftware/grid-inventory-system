using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIInspectMenu : MonoBehaviour
{
    #region --- VARIABLES ---

    public InventoryItem invItem;

    public Image iconImage;
    public TextMeshProUGUI descriptionText;

    #endregion

    #region --- MONOBEHAVIOUR ---

    protected virtual void Start()
    {
        Init();
    }

    #endregion

    #region --- METHODS ---

    protected virtual void Init()
    {
        if (iconImage == null && !TryGetComponent(out iconImage))
        {
            Debug.LogError($"{gameObject.name} ({name}) has no Image component for iconImage!");
            return;
        }

        if (descriptionText == null && !TryGetComponent(out descriptionText))
        {
            Debug.LogError($"{gameObject.name} ({name}) has no TextMeshProUGUI component for descriptionText!");
            return;
        }

        if (invItem == null)
        {
            Debug.LogWarning($"{gameObject.name} ({name}) has no inventory item assigned on initialisation!");
        }
        else
        {
            descriptionText.text = invItem.item.description;
            iconImage.sprite = invItem.item.icon;
        }
    }

    #endregion
}
