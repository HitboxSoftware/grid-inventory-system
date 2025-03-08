using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Slider = UnityEngine.UIElements.Slider;

namespace Hitbox.Stash.UI
{
    [RequireComponent(typeof(LayoutElement))]
    public class InventoryUIItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Fields

        public InventoryUIAbstractGrid UIGrid { get; protected set; }
        public InventoryItem InvItem { get; protected set; }
        [SerializeField] Image icon;
        [SerializeField] Image backgroundImage;

        private RectTransform _rectTransform;
        private LayoutElement _layoutElement;
        
        [Header("Data")] 
        [SerializeField] TextMeshProUGUI nameTextDisplay;
        
        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            UpdateItem();
        }
        
        private void OnEnable()
        {
            if(InvItem != null)
                InvItem.Updated += OnItemUpdated;
        }
        
        private void OnDisable()
        {
            if(InvItem != null)
                InvItem.Updated -= OnItemUpdated;
        }
        
        #endregion

        #region Methods

        protected virtual void Init()
        {
            _rectTransform = GetComponent<RectTransform>();
            _layoutElement = GetComponent<LayoutElement>();

            // Items should always ignore layout.
            _layoutElement.ignoreLayout = true;
        }

        // Retrieved from Parent Grid.
        public virtual void UpdateItem()
        {
            if (UIGrid == null) return;
            if (UIGrid.Style == null) return;
            if (InvItem == null) return;
            
            transform.SetParent(UIGrid.transform);

            // Set rect size to match grid size
            _rectTransform.sizeDelta = new Vector2(
                UIGrid.GetSlotSize().x * InvItem.Size.x,
                UIGrid.GetSlotSize().y * InvItem.Size.y
            );

            // Setting anchor.
            _rectTransform.anchorMin = new Vector2(0, 1);
            _rectTransform.anchorMax = _rectTransform.anchorMin;

            // Set position to average position of slots
            if (InvItem.ParentContainer is InventoryGrid grid)
            {
                _rectTransform.anchoredPosition = UIGrid.GridToLocalPoint(grid.GetItemCentre(InvItem));
            }
            
            UpdateDisplay();

            if (icon == null) return;
            
            // Update Icon
            if (InvItem.ItemProfile.icon != null)
            {
                icon.sprite = InvItem.ItemProfile.icon;
            }
            else
            {
                IconGenerator.Instance.SetAngle(InvItem.ItemProfile.modelIconAngle);
                icon.sprite = IconGenerator.Instance.GenerateSpriteFromPrefab(InvItem.ItemProfile.worldObject, true);
            }
            
            icon.enabled = true;
            icon.preserveAspect = true;

            // Update Rotation
            RectTransform iconTransform = icon.GetComponent<RectTransform>();
            Vector2 size = iconTransform.rect.size;
            
            // Setting anchors to center.
            iconTransform.anchorMin = new Vector2(0.5f, 0.5f);
            iconTransform.anchorMax = iconTransform.anchorMin;
            
            // updating size & rotation.
            if (InvItem.Rotated)
            {
                iconTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.y, _rectTransform.sizeDelta.x) - new Vector2(10, 10);
                iconTransform.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                iconTransform.sizeDelta = _rectTransform.sizeDelta - new Vector2(10, 10);
                iconTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
            
            
            // TODO: Update Colours of UI Element
        }

        public virtual void UpdateDisplay()
        {
            if (InvItem?.ItemProfile is null) return;
            
            if(nameTextDisplay != null)
                nameTextDisplay.text = InvItem.ItemProfile.name;
        }
        
        public virtual void AssignItem(InventoryItem item)
        {
            if ((item == null || item != InvItem) && InvItem != null)
            {
                InvItem.Updated -= OnItemUpdated;
            }
            
            InvItem = item;

            if (InvItem != null)
            {
                InvItem.Updated += OnItemUpdated;
            }
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

        public virtual void Highlight(Color highlightColor)
        {
            if (backgroundImage == null) return;

            backgroundImage.color = highlightColor;
        }
        
        private void OnItemUpdated()
        {
            UpdateDisplay();
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