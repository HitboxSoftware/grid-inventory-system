using System;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Stash.UI
{
    public class InventoryUIDragContainer : MonoBehaviour
    {
        #region Fields

        private InventoryItemDragData _dragData;
        [SerializeField] InventoryUIStyle style;
        [SerializeField] Image image;

        private RectTransform _rectTransform;
        private bool _currentRotation;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            Init();
        }
        

        private void LateUpdate()
        {
            transform.position = Input.mousePosition;

            if (_dragData is { InvItem: not null } && _dragData.InvItem.Rotated != _currentRotation)
            {
                _currentRotation = _dragData.InvItem.Rotated;
                
                // Updating rotation.
                if (_currentRotation)
                {
                    _rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                }
                else
                {
                    _rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
            }
        }

        #endregion

        #region Methods

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

            if (image != null)
            {
                image.raycastTarget = false;
                image.preserveAspect = true;
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
                style.cellSize.x * _dragData.InvItem.ItemProfile.size.x + style.cellSpacing.x * (_dragData.InvItem.ItemProfile.size.x - 1),
                style.cellSize.y * _dragData.InvItem.ItemProfile.size.y + style.cellSpacing.y * (_dragData.InvItem.ItemProfile.size.y - 1));

            _rectTransform.localRotation = Quaternion.Euler(!data.InvItem.Rotated ? new Vector3(0, 0, 0) : new Vector3(0, 0, -90));

            if (_dragData.InvItem.ItemProfile.worldObject != null)
            {
                IconGenerator.Instance.SetAngle(_dragData.InvItem.ItemProfile.modelIconAngle);
                image.sprite = IconGenerator.Instance.GenerateSpriteFromPrefab(_dragData.InvItem.ItemProfile.worldObject, true);
            }
            else
            {
                image.sprite = _dragData.InvItem.ItemProfile.icon;
            }

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