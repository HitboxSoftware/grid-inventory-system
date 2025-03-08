using System;
using Hitbox.Stash;
using Hitbox.Stash.Categories;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hitbox.Stash.UI
{
    public class InventoryUIItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields

        /// <summary>
        /// Linked back-end slot
        /// </summary>
        public InventoryItemSlot LinkedSlot { get; protected set; }

        /// <summary>
        /// What item categories can be inserted into the slot
        /// </summary>
        public ItemCategory acceptedCategory;

        /// <summary>
        /// 
        /// </summary>
        public Image itemImageUI;

        // Events
        public static event Action<InventoryUIItemSlot> MouseEnter;
        public static event Action<InventoryUIItemSlot> MouseExit;

        #endregion

        #region MonoBehaviour

        public void Start()
        {
            SetSlot(new InventoryItemSlot(acceptedCategory));
        }

        #endregion

        #region Methods

        public virtual void SetSlot(InventoryItemSlot itemSlot)
        {
            if (LinkedSlot != null)
            {
                LinkedSlot.OnUpdated -= OnSlotUpdated;
            }

            LinkedSlot = itemSlot;

            if (LinkedSlot != null)
            {
                LinkedSlot.OnUpdated += OnSlotUpdated;
            }

            UpdateSlot();
        }

        protected virtual void UpdateSlot()
        {
            if (LinkedSlot == null || itemImageUI == null) return;

            itemImageUI.preserveAspect = true;

            if (!LinkedSlot.IsItemAttached())
            {
                itemImageUI.sprite = null;
                itemImageUI.enabled = false;
                return;
            }

            if (LinkedSlot.AttachedItem.ItemProfile.worldObject != null)
            {
                IconGenerator.Instance.SetAngle(LinkedSlot.AttachedItem.ItemProfile.modelIconAngle);
                itemImageUI.sprite = IconGenerator.Instance.GenerateSpriteFromPrefab(LinkedSlot.AttachedItem.ItemProfile.worldObject, true);
            }
            else
            {
                itemImageUI.sprite = LinkedSlot.AttachedItem.ItemProfile.icon;
            }
            itemImageUI.enabled = true;
        }

        protected virtual void OnSlotUpdated(InventoryItem invItem)
        {
            UpdateSlot();
        }

        #endregion

        #region UI Events

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

}