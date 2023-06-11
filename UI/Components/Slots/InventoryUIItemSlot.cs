using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryUIItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region --- VARIABLES ---

    public InventoryItemSlot LinkedSlot { get; protected set; }

    public Image itemImage;
    public Outline outline;
    
    // Events
    public static event Action<InventoryUIItemSlot> MouseEnter;
    public static event Action<InventoryUIItemSlot> MouseExit;

    #endregion

    #region --- MONOBEHAVIOUR ---
    
    protected virtual void Start()
    {
        if (outline == null)
        {
            TryGetComponent(out outline);
        }
    }
    
    #endregion
        
    #region --- METHODS ---

    public virtual void SetSlot(InventoryItemSlot itemSlot)
    {
        if (LinkedSlot != null)
        {
            LinkedSlot.Updated -= OnSlotUpdated;
        }

        LinkedSlot = itemSlot;
        
        if (LinkedSlot != null)
        {
            LinkedSlot.Updated += OnSlotUpdated;
        }
        
        UpdateSlot();
    }

    public virtual void HighlightSlot(Color color)
    {
        if (outline == null) return;

        outline.effectColor = color;
    }

    protected virtual void UpdateSlot()
    {
        if (LinkedSlot == null) return;

        if (!LinkedSlot.HasItem())
        {
            itemImage.sprite = null;
            itemImage.enabled = false;
            return;
        }
            
        itemImage.sprite = (LinkedSlot.AttachedItem.item).icon;
        itemImage.enabled = true;
    }
    
    protected virtual void OnSlotUpdated(InventoryItem invItem)
    {
        UpdateSlot();
    }

    #endregion

    #region --- UI EVENTS ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseEnter?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseExit?.Invoke(this);
    }

    #endregion
}
