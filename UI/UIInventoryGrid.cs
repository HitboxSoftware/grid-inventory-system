using System;
using System.Collections.Generic;
using System.Linq;
using Hitbox.UGIS;
using Hitbox.UGIS.UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Hitbox.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class UIInventoryGrid : MonoBehaviour
    {
        #region --- VARIABLES ---

        public InventoryGrid Grid { private set; get; }
        
        // Properties
        [SerializeField] private UIInventoryStyle style;
        public UIInventoryStyle Style => style;

        [SerializeField] private Vector2Int size;

        [SerializeField] private bool generateOnStart;
        
        // - Components -
        private Transform gridTransform;
        private GridLayoutGroup gridLayout;
        private ContentSizeFitter sizeFitter;
        
        // Runtime
        private readonly Dictionary<Vector2Int, UIInventorySlot> slots = new();
        private readonly Dictionary<InventoryItem, UIInventoryItem> items = new();
        
        // - TEMP -
        public List<Item> startingItems;

        #endregion

        #region --- MONOBEHAVIOUR ---

        public void Start()
        {
            Init();
            
            // TEMP
            if (generateOnStart && startingItems is { Count: > 0 })
            {
                for (int i = 0; i < 4; i++)
                {
                    InventoryItem invItem = new (startingItems[Random.Range(0, startingItems.Count)], Grid);
                    if(Grid.AutoAddItem(invItem))
                        CreateUIItem(invItem);

                    if (Random.Range(1, 100) > 50)
                    {
                        Grid.TryRotateItem(invItem);
                    }
                }
            }
        }

        private void OnEnable()
        {
            InventoryGrid.ItemUpdated += CheckItemUpdate;
        }

        private void OnDisable()
        {
            InventoryGrid.ItemUpdated -= CheckItemUpdate;
        }

        #endregion

        #region --- METHODS ---

        #region Initiation

        private void Init()
        {
            if(generateOnStart)
                Generate();
            
            if (gridTransform == null) gridTransform = transform;
            
            // Getting Required Front-end Elements.
            gridLayout = gridTransform.GetComponent<GridLayoutGroup>(); //Getting Grid Layout

            // Setting Grid Constraints
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = Grid.Size.x;

            //Attempting to Get Size Fitter
            if (TryGetComponent(out sizeFitter))
            {
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        #endregion

        #region Generation

        // Generates Grid, overwrite will always make a new grid.
        public void Generate(bool overwrite = false)
        {
            if(Grid == null || overwrite)
                Grid = new InventoryGrid(size);

            // Generate UI Slots for Grid.
            if (Grid != null)
            {
                GenerateSlots();
            }

            // Generate Grid Items, if any found.
            if (Grid?.AllItems != null && Grid.AllItems.Any())
            {
                GenerateItems();
            }
        }

        private void GenerateSlots()
        {
            for (int y = 0; y < Grid.Size.y; y++)
            {
                for (int x = 0; x < Grid.Size.x; x++)
                {
                    // --- Create Slot Object ---
                    GameObject slotObj = Instantiate(style.slotObj, transform);
                    slotObj.name = "Slot [" + x + "," + y + "]"; //Set name to Slot Position

                    // --- Add Slot Component ---
                    if (!slotObj.TryGetComponent(out UIInventorySlot slotComponent)) //Add Slot Component if none exists
                    {
                        slotComponent = slotObj.AddComponent<UIInventorySlot>();
                    }

                    // --- Update Slot Component Values ---
                    Vector2Int slotPos = new(x, y);
                    slotComponent.slotPosition = slotPos;
                    slotComponent.AssignGrid(this);

                    slots.Add(slotPos, slotComponent);
                }
            }
            
            // Rebuild Layout to ensure correct layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        private void GenerateItems()
        {
            ClearUIItems();
            
            foreach (InventoryItem invItem in Grid.AllItems)
            {
                CreateUIItem(invItem);
            }
            
            // Rebuild Layout to ensure correct layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void AssignGrid(InventoryGrid newGrid, bool generateUI)
        {
            if (newGrid == null) return;

            Grid = newGrid;
            
            Init();
            
            if(generateUI)
                Generate();
        }

        #endregion

        #region Manipulation
        
        // - ALL FUNCTIONS RELATED TO ITEM MANIPULATION -

        // Creates, adds and returns newly created UI item based on given inventory item.
        public UIInventoryItem CreateUIItem(InventoryItem invItem)
        {
            GameObject itemObj = Instantiate(style.itemObj, transform);

            // --- Add Item Component ---
            if (!itemObj.TryGetComponent(out UIInventoryItem uiItem)) //Add Item Component if none exists
            {
                uiItem = itemObj.AddComponent<UIInventoryItem>();
            }

            // --- Update Slots ---
            foreach (Vector2Int slot in invItem.TakenSlots)
            {
                slots[slot].GetComponent<UIInventorySlot>().containedItem = uiItem;
            }
            
            uiItem.InvItem = invItem;
            uiItem.AssignGrid(this);

            items.Add(invItem, uiItem);
            
            return null;
        }

        public bool AddUIItem(UIInventoryItem uiItem, Vector2Int position)
        {
            if (!Grid.AddItemAtPosition(uiItem.InvItem, position)) return false;
            if (items.ContainsKey(uiItem.InvItem)) items.Remove(uiItem.InvItem);
            
            // --- Update Slots ---
            foreach (Vector2Int slot in uiItem.InvItem.TakenSlots)
            {
                slots[slot].GetComponent<UIInventorySlot>().containedItem = uiItem;
            }
            
            uiItem.AssignGrid(this);
            items.Add(uiItem.InvItem, uiItem);
            
            return true;
        }

        public void RotateUIItem(UIInventoryItem uiItem)
        {
            // UI auto updated by grid ITEMUPDATED Event.
            Grid.TryRotateItem(uiItem.InvItem);
        }
        
        public void SetUIItemRotation(UIInventoryItem uiItem, bool rotate)
        {
            if (uiItem.InvItem.ItemRuntimeData.rotated == rotate) return;

            // UI auto updated by grid ITEMUPDATED Event.
            Grid.TryRotateItem(uiItem.InvItem);
        }

        // Removes specified item and then returns the item.
        public UIInventoryItem RemoveUIItem(UIInventoryItem uiItem, bool updateBackend = true)
        {
            // --- Update Slots ---
            foreach (Vector2Int slot in uiItem.InvItem.TakenSlots)
            {
                slots[slot].GetComponent<UIInventorySlot>().containedItem = null;
            }
            
            items.Remove(uiItem.InvItem); // Removing from Dictionary
            
            if(updateBackend)
                Grid.RemoveItem(uiItem.InvItem);
            
            return uiItem;
        }

        public void DeleteUIItem(UIInventoryItem targetItem)
        {
            Destroy(RemoveUIItem(targetItem).gameObject);
        }

        // Clears grid of all UI items, no changes to backend.
        public void ClearUIItems()
        {
            if (items == null || items.Count == 0) return;

            List<UIInventoryItem> itemList = items.Values.ToList();

            foreach (UIInventoryItem uiInventoryItem in itemList)
            {
                items.Remove(uiInventoryItem.InvItem); // Removing from Dictionary
                Destroy(uiInventoryItem.gameObject); // Destroy Game Object
            }
        }

        #endregion

        private void CheckItemUpdate(InventoryItem invItem)
        {
            if (!items.ContainsKey(invItem)) return;

            UIInventoryItem uiItem = items[invItem];
            
            // --- Update Slots ---

            foreach (Vector2Int pos in uiItem.CurrentTakenSlots)
            {
                slots[pos].containedItem = null;
            }
            
            foreach (Vector2Int slot in invItem.TakenSlots)
            {
                slots[slot].GetComponent<UIInventorySlot>().containedItem = uiItem;
            }

            uiItem.UpdateItem();
            
            uiItem.CurrentTakenSlots = invItem.TakenSlots;
        }

        #region Utilities
        
        public Vector2 GetAveragePosition(IReadOnlyCollection<Vector2Int> input)
        {
            Vector2 sum = input.Aggregate(Vector2.zero,
                (current, slot) => current + slots[slot].GetComponent<RectTransform>().anchoredPosition);

            return sum / input.Count;
        }

        public UIInventorySlot[] GetSlotsFromPositions(IEnumerable<Vector2Int> positions)
        {
            return (from position in positions
                where slots.ContainsKey(position)
                select slots[position]).ToArray();
        }

        #endregion

        #endregion
    }

}