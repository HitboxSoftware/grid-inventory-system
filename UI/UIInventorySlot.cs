using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hitbox.UI
{
    public class UIInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region --- VARIABLES ---

        public UIInventoryGrid UIGrid { get; private set; }
        public Vector2Int slotPosition;
        public UIInventoryItem containedItem;
        public Image background;
        
        // Events
        public static event Action<UIInventorySlot> MouseEnter;
        public static event Action<UIInventorySlot> MouseExit;

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

        public void AssignGrid(UIInventoryGrid newGrid)
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
            MouseEnter.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseExit.Invoke(this);
        }

        #endregion
    }

}