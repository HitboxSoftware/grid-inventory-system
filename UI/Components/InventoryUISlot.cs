using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hitbox.Inventory.UI
{
    public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region --- VARIABLES ---

        public InventoryUIAbstractGrid UIGrid { get; private set; }
        public Vector2Int slotPosition;
        public Image background;
        public InventoryUIItem containedItem;

        // Events
        public static event Action<InventoryUISlot> MouseEnter;
        public static event Action<InventoryUISlot> MouseExit;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            if (background == null)
            {
                 TryGetComponent(out background);
            }
        }

        #endregion
        
        #region --- METHODS ---

        public void AssignGrid(InventoryUIAbstractGrid newGrid)
        {
            if (newGrid == null) return;

            UIGrid = newGrid;
        }

        public void HighlightSlot(Color color)
        {
            if (background == null) return;

            background.color = color;
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

}