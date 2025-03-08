using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hitbox.Stash.UI
{
    public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields

        public InventoryUISlotGrid UIGrid { get; private set; }
        public Vector2Int slotPosition;
        public Image background;
        public InventoryUIItem containedItem;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            if (background == null)
            {
                 TryGetComponent(out background);
            }
        }

        #endregion
        
        #region Methods

        public void AssignGrid(InventoryUISlotGrid newGrid)
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
            UIGrid.UpdateGridPos(slotPosition);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIGrid.ClearGridPos();
        }

        #endregion
    }

}