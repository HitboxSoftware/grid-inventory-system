using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Inventory.UI
{
    public class InventoryUIDragContainer : MonoBehaviour
    {
        #region --- VARIABLES ---

        private InventoryItemDragData _dragData;
        [SerializeField] private InventoryUIStyle style;
        [SerializeField] private Image image;

        private RectTransform _rectTransform;
        private bool _currentRotation;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            Init();
        }
        

        private void LateUpdate()
        {
            transform.position = Input.mousePosition;

            if (_dragData is { invItem: not null } && _dragData.invItem.rotated != _currentRotation)
            {
                _currentRotation = _dragData.invItem.rotated;
                
                Vector2 size = _rectTransform.sizeDelta;
                // Updating size & rotation.
                if (_currentRotation)
                {
                    _rectTransform.sizeDelta = new Vector2(size.y, size.x);
                    _rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                }
                else
                {
                    _rectTransform.sizeDelta = new Vector2(size.x, size.y);
                    _rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
            }
        }

        #endregion

        #region --- METHODS ---

        private void Init()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
                // Update Rotation
                Vector2 size = _rectTransform.rect.size;
                // Setting anchors to center.
                _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                _rectTransform.anchorMax = _rectTransform.anchorMin;
                _rectTransform.sizeDelta = size;
            }
        }

        public void SetDraggedItem(InventoryItemDragData data)
        {
            if (style == null) return;
            _dragData = data;
            
            if (_rectTransform == null)
            {
                Init();
            }

            // Set rect size to match item size
            _rectTransform.sizeDelta = new Vector2(
                style.cellSize.x * _dragData.invItem.Size.x + style.cellSpacing.x * (_dragData.invItem.Size.x - 1),
                style.cellSize.y * _dragData.invItem.Size.y + style.cellSpacing.y * (_dragData.invItem.Size.y - 1));
            
            image.sprite = _dragData.invItem.item.icon;
            gameObject.SetActive(true);
        }

        public void ClearDraggedItem()
        {
            _dragData = null;
            image.sprite = null;
            gameObject.SetActive(false);
        }

        #endregion
    }


}