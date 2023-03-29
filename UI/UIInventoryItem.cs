using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.UGIS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hitbox.UI
{
    public class UIInventoryItem : MonoBehaviour
    {
        #region --- VARIABLES ---

        public UIInventoryGrid UIGrid { get; private set; }
        public InventoryItem InvItem;
        public Vector2Int[] CurrentTakenSlots;

        [Header("Components")] 
        [SerializeField] private Image icon;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            UpdateItem();

        }
        
        #endregion

        #region --- METHODS ---

        // Retrieved from Parent Grid.
        public void UpdateItem(bool updatePosition = true)
        {
            if (UIGrid == null) return;
            if (UIGrid.Style == null) return;
            
            transform.SetParent(UIGrid.transform, worldPositionStays:false);

            // Set rect size to match grid size
            GetComponent<RectTransform>().sizeDelta = new Vector2(
                UIGrid.Style.cellSize.x * InvItem.Size.x + UIGrid.Style.cellSpacing.x * (InvItem.Size.x - 1),
                UIGrid.Style.cellSize.y * InvItem.Size.y + UIGrid.Style.cellSpacing.y * (InvItem.Size.y - 1));

            // Set position to average position of slots
            GetComponent<RectTransform>().anchoredPosition = UIGrid.GetAveragePosition(InvItem.TakenSlots);
            
            if (InvItem.ItemRuntimeData.rotated)
            {
                icon.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else
            {
                icon.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            icon.sprite = InvItem.Item.icon;

            // TODO: Update Colours of UI Element
        }

        public void AssignGrid(UIInventoryGrid newGrid)
        {
            if (newGrid == null)
            {
                Debug.LogWarning("Attempted to Assign Item to Non-Existent Grid", this);
                return;
            }
            
            UIGrid = newGrid;
            
            // Update Style, as parent UI Grid Style could now be different.
            UpdateItem();
        }

        #endregion


    }

}