using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


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
        
        private readonly Dictionary<Vector2Int, GameObject> slots = new();
        
        // - Components
        private GridLayoutGroup gridLayout;
        private ContentSizeFitter sizeFitter;

        // - Generated Objects -
        private readonly Dictionary<InventoryItem, InventoryUIItem> uiItems = new ();

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            GenerateSlots();
            GenerateItems();
        }

        #endregion

        #region --- METHODS ---
        
        private void Init()
        {
            // Return if no grid found.
            if (grid == null && !TryGetComponent(out grid)) return;
            
            // Getting Required Front-end Elements.
            gridLayout = GetComponent<GridLayoutGroup>(); //Getting Grid Layout
            sizeFitter = GetComponent<ContentSizeFitter>(); //Getting Size Fitter, Not Required.

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

        // Generates all the slots in the UI.
        public void GenerateSlots()
        {
            for (int y = 0; y < grid.size.y; y++)
            {
                for (int x = 0; x < grid.size.x; x++)
                {
                    // --- Create Slot Object ---
                    GameObject slotObj = Instantiate(style.slotObj, transform);
                    slotObj.name = "Slot [" + x + "," + y + "]"; //Set name to Slot Position

                    slots.Add(new Vector2Int(x, y), slotObj);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void GenerateItems()
        {
            ClearItems();
            foreach (InventoryItem invItem in grid.GetAllItems())
            {
                GenericItem item = invItem.Item;
                GameObject itemObj = Instantiate(style.itemObj, transform);
                
                // Get / Add Item Component
                InventoryUIItem itemComponent = itemObj.GetComponent<InventoryUIItem>();
                if (itemComponent == null) //Add Slot Component if none exists
                {
                    itemComponent = itemObj.AddComponent<InventoryUIItem>();
                }
                
                uiItems.Add(invItem, itemComponent);

                // Set Rect Size to Match Grid Size
                itemObj.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    gridLayout.cellSize.x * item.size.x + gridLayout.spacing.x * (item.size.x - 1),
                    gridLayout.cellSize.y * item.size.y + gridLayout.spacing.y * (item.size.y - 1));

                Vector2 sum = Vector2.zero; //Used for Calculating Average Position of Slots.
                foreach(Vector2Int slot in invItem.TakenSlots)
                {
                    sum += slots[slot].GetComponent<RectTransform>().anchoredPosition; //Calculate Average Position of Slots
                }
            
                // --- Set Position to Average Position of Slots. ---
                itemObj.GetComponent<RectTransform>().anchoredPosition = sum / invItem.TakenSlots.Length;
            }
        }

        #endregion

        #region - GRID UTILITIES -

        private void ClearItems()
        {
            foreach (InventoryUIItem item in uiItems.Values)
            {
                Destroy(item.gameObject);
            }
            
            uiItems.Clear();
        }

        // Assigns new grid and goes through GridUI initialization.
        public void AssignGrid(InventoryGrid newGrid, bool generateUI)
        {
            if (newGrid == null) return;
            
            grid = newGrid;
            
            Init();

            if (generateUI)
            {
                GenerateSlots();
                GenerateItems();
            }
        }

        #endregion

        #endregion
    }

}