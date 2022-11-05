using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace KoalaDev.UGIS.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryUIGrid : MonoBehaviour
    {
        #region --- VARIABLES ---

        // --- Backend ---
        [SerializeField] private InventoryGrid grid;

        // --- User Interface ---
        [SerializeField] private InventoryUIStyle style;
        [SerializeField] private bool generateOnStart;
        
        // - Components -
        private GridLayoutGroup gridLayout;
        private ContentSizeFitter sizeFitter;

        // - Generated Objects -
        private readonly Dictionary<Vector2Int, InventoryUISlot> slots = new ();
        private readonly Dictionary<GameObject, InventoryUIItem> items = new ();

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            if(generateOnStart) Generate();
        }

        #endregion

        #region --- METHODS ---
        
        private void Init()
        {
            // Return if no grid found.
            if (grid == null && !TryGetComponent(out grid)) return;
            
            // Getting Required Front-end Elements.
            gridLayout = GetComponent<GridLayoutGroup>(); //Getting Grid Layout

            // Setting Grid Constraints
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = grid.size.x;
            
            //Attempting to Get Size Fitter
            if (TryGetComponent(out sizeFitter))
            {
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        #region - GRID GENERATION -
        
        // Generate Grid
        public void Generate()
        {
            ClearGrid();
            for (int y = 0; y < grid.size.y; y++)
            {
                for (int x = 0; x < grid.size.x; x++)
                {
                    // --- Create Slot Object ---
                    GameObject slotObj = Instantiate(style.slotObj, transform);
                    slotObj.name = "Slot [" + x + "," + y + "]"; //Set name to Slot Position

                    // --- Add Slot Component ---
                    if (!slotObj.TryGetComponent(out InventoryUISlot slotComponent)) //Add Slot Component if none exists
                    {
                        slotComponent = slotObj.AddComponent<InventoryUISlot>();
                    }
                    
                    Vector2Int slotPos = new (x, y);

                    // --- Update Slot Component Values ---
                    slotComponent.slotPosition = slotPos;
                    slotComponent.grid = this;
                    
                    slots.Add(slotPos, slotComponent);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            
            foreach (InventoryItem invItem in grid.GetAllItems())
            {
                Item item = invItem.Item;
                GameObject itemObj = Instantiate(style.itemObj, transform);
                
                // --- Add Item Component ---
                if (!itemObj.TryGetComponent(out InventoryUIItem uiItem)) //Add Item Component if none exists
                {
                    uiItem = itemObj.AddComponent<InventoryUIItem>();
                }
                
                uiItem.InvItem = invItem;
                uiItem.uiGrid = this;

                // --- Update Slots ---
                foreach (Vector2Int slot in invItem.TakenSlots)
                {
                    slots[slot].GetComponent<InventoryUISlot>().containedItem = uiItem;
                }
                
                items.Add(itemObj, uiItem);

                // Set Rect Size to Match Grid Size
                itemObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    gridLayout.cellSize.x * item.size.x + gridLayout.spacing.x * (item.size.x - 1),
                    gridLayout.cellSize.y * item.size.y + gridLayout.spacing.y * (item.size.y - 1));
                
                // --- Set Position to Average Position of Slots. ---
                itemObj.GetComponent<RectTransform>().anchoredPosition = GetAveragePosition(invItem.TakenSlots);
            }
        }

        #endregion

        #region - GRID UTILITIES -
        
        private void ClearGrid()
        {
            foreach (InventoryUISlot uiSlot in slots.Values)
            {
                Destroy(uiSlot.gameObject);
            }
            
            slots.Clear();
            
            ClearItems();
        }

        private void ClearItems()
        {
            foreach (InventoryUIItem uiItem in items.Values)
            {
                Destroy(uiItem.gameObject);
            }
            
            items.Clear();
        }
        
        public bool RemoveItem(InventoryUIItem uiItem)
        {
            if (grid.RemoveItem(uiItem.InvItem))
            {
                foreach (Vector2Int takenSlot in uiItem.InvItem.TakenSlots)
                {
                    slots[takenSlot].containedItem = null;
                }
                
                items.Remove(uiItem.gameObject);
                return true;
            }

            return false;
        }

        public bool AddItemAtPosition(InventoryUIItem uiItem, Vector2Int pos)
        {
            if (grid.AddItemAtPosition(uiItem.InvItem, pos))
            {
                uiItem.transform.SetParent(transform);
                uiItem.uiGrid = this; //Set Item Parent Grid to New Grid

                Item item = uiItem.InvItem.Item;
                
                // --- Set Rect Size to Match Grid Size, Including Spacing. ---
                uiItem.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    gridLayout.cellSize.x * item.size.x + gridLayout.spacing.x * (item.size.x - 1),
                    gridLayout.cellSize.y * item.size.y + gridLayout.spacing.y * (item.size.y - 1));
                
                uiItem.GetComponent<RectTransform>().anchoredPosition = GetAveragePosition(uiItem.InvItem.TakenSlots);
                
                // --- Update Slots ---
                foreach (Vector2Int slot in uiItem.InvItem.TakenSlots)
                {
                    slots[slot].GetComponent<InventoryUISlot>().containedItem = uiItem;
                }
                
                items.Add(uiItem.gameObject, uiItem);
                return true;
            }

            return false;
        }

        // Assigns new grid and goes through GridUI initialization.
        public void AssignGrid(InventoryGrid newGrid, bool generateUI)
        {
            if (newGrid == null) return;
            
            grid = newGrid;
            
            Init();

            if (generateUI)
            {
                Generate();
            }
        }

        private Vector2 GetAveragePosition(IReadOnlyCollection<Vector2Int> input)
        {
            Vector2 sum = input.Aggregate(Vector2.zero,
                (current, slot) => current + slots[slot].GetComponent<RectTransform>().anchoredPosition);

            return sum / input.Count;
        }

        #endregion
        
        #endregion
    }
}