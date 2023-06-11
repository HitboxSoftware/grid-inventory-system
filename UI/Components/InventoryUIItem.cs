using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace Hitbox.Inventory.UI
{
    public class InventoryUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region --- VARIABLES ---

        public InventoryUIAbstractGrid UIGrid { get; protected set; }
        public InventoryItem InvItem { get; protected set; }
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
        public virtual void UpdateItem()
        {
            if (UIGrid == null) return;
            if (UIGrid.Style == null) return;
            if (InvItem == null) return;
            
            transform.SetParent(UIGrid.transform);

            // Set rect size to match grid size
            GetComponent<RectTransform>().sizeDelta = new Vector2(
                UIGrid.CalculateCellSize().x * InvItem.Size.x,
                UIGrid.CalculateCellSize().y * InvItem.Size.y
            );

            // Set position to average position of slots
            GetComponent<RectTransform>().anchoredPosition = UIGrid.GetAveragePosition(InvItem.takenPositions);
            
            // Update Icon
            icon.sprite = InvItem.item.icon;
            icon.enabled = true;
            icon.preserveAspect = true;

            // Update Rotation
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            Vector2 size = iconTransform.rect.size;
            // Setting anchors to center.
            iconTransform.anchorMin = new Vector2(0.5f, 0.5f);
            iconTransform.anchorMax = iconTransform.anchorMin;
            
            // updating size & rotation.
            if (InvItem.rotated)
            {
                iconTransform.sizeDelta = new Vector2(size.y, size.x);
                iconTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
            else
            {
                iconTransform.sizeDelta = new Vector2(size.x, size.y);
                iconTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            
            //TODO: Rotate Icon if item is rotated.

            // TODO: Update Colours of UI Element
        }
        
        public virtual void AssignItem(InventoryItem item)
        {
            InvItem = item;
            
        }

        public virtual void AssignUIGrid(InventoryUIAbstractGrid newGrid)
        {
            if (newGrid == null)
            {
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to assign itself to non-existent grid!");
                return;
            }
            
            UIGrid = newGrid;

            // Update Style, as parent UI Grid Style could now be different.
            UpdateItem();
        }
        
        #endregion

        #region --- UI EVENTS ---

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        #endregion


    }

}