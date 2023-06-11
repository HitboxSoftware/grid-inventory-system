using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hitbox.Inventory.UI
{
    /// <summary>
    /// This UI grid also handles the generation of UI Slots utilising GridLayoutGroup to position them, items must have
    /// a LayoutElement set to ignore layout to make sure they're positioned correctly..
    /// <para/>
    /// Less performant for very large grids but allows for more complex visuals.
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class InventoryUISlotGrid : InventoryUIAbstractGrid
    {
        #region --- VARIABLES ---

        // Layout Components
        private GridLayoutGroup _gridLayout;
        private ContentSizeFitter _sizeFitter;
        
        // UI Slots
        protected readonly Dictionary<Vector2Int, InventoryUISlot> slots = new();

        #endregion

        #region --- METHODS ---

        #region Initiation

        protected override void Init()
        {
            // - Basic Initialisation -
            base.Init();

            // - Initialise Components -
            // Get Required Front-end Elements.
            if (!TryGetComponent(out _gridLayout))
            {
                _gridLayout = gameObject.AddComponent<GridLayoutGroup>();
            }
            
            if (!TryGetComponent(out _sizeFitter))
            {
                _sizeFitter = gameObject.AddComponent<ContentSizeFitter>();
            }
            
            _sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            _sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // - Complete Initialisation (Only possible once Grid and Style assigned!) -
            if (Grid == null)
            {
                // Don't log error, grid is allowed to be null on init.
                return;
            }

            if (Style == null)
            {
                // Don't log error, style is allowed to be null on init.
                return;
            }

            // Setting Grid Constraints
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.cellSize = style.cellSize;
            _gridLayout.constraintCount = Grid.size.x;
            _gridLayout.spacing = style.cellSpacing;
        }

        #endregion

        #region Generation

        // Overriden as sl
        public override void Generate()
        {
            // Generate UI Slots for grid *before* generating items.
            if (Grid != null)
            {
                GenerateSlots();
            }

            base.Generate();
            
            // Rebuild Layout to ensure correct grid layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }

        private void GenerateSlots()
        {
            if (Grid == null)
            {
                // Cancel Slot Generation.
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to generate UI Slots with no linked InventoryGrid!");
                return;
            }
            
            ClearUISlots();
            
            for (int y = 0; y < Grid.size.y; y++)
            {
                for (int x = 0; x < Grid.size.x; x++)
                {
                    // --- Create Slot Object ---
                    GameObject slotObj = Instantiate(style.slotObj, transform);
                    slotObj.name = "Slot [" + x + "," + y + "]"; //Set name to Slot Position

                    // --- Add Slot Component ---
                    if (!slotObj.TryGetComponent(out InventoryUISlot slotComponent)) //Add Slot Component if none exists
                    {
                        slotComponent = slotObj.AddComponent<InventoryUISlot>();
                    }

                    // --- Update Slot Component Values ---
                    Vector2Int slotPos = new(x, y);
                    slotComponent.slotPosition = slotPos;
                    slotComponent.AssignGrid(this);

                    if (slotComponent.TryGetComponent(out Image image))
                    {
                        image.color = style.slotColour;
                    }
                    
                    slots.Add(slotPos, slotComponent);
                }
            }
            
            // Rebuild Layout to ensure correct layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }
        
        // Overriden as Slots must 
        protected override void GenerateItems()
        {
            ClearUIItems();
            
            foreach (InventoryItem invItem in Grid.AllItems)
            {
                CreateUIItem(invItem);
            }
            
            // Rebuild Layout to ensure correct layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }

        public override bool Assign(InventoryGrid newGrid, bool generateUI)
        {
            if (!base.Assign(newGrid, generateUI)) return false;
            
            // Rebuild Layout to ensure correct layout.
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            
            return true;
        }

        public override void ClearUI()
        {
            base.ClearUI();
            
            ClearUISlots();
        }

        #endregion

        #region Manipulation

        public override bool RemoveUIItem(InventoryUIItem uiItem)
        {
            foreach (Vector2Int slot in uiItem.InvItem.takenPositions)
            {
                if (!slots.ContainsKey(slot)) continue; // Ignore.
                
                slots[slot].containedItem = null;
            }
            
            items.Remove(uiItem.InvItem); // Removing from Dictionary

            return true;
        }

        public override InventoryUIItem CreateUIItem(InventoryItem invItem)
        {
            InventoryUIItem uiItem = base.CreateUIItem(invItem);
            UpdateSlotsWithItem(uiItem);
            return uiItem;
        }

        public void ClearUISlots()
        {
            if (slots == null || slots.Count == 0) return;
            
            ClearUIItems();
            
            List<Vector2Int> slotList = slots.Keys.ToList();

            foreach (Vector2Int slot in slotList)
            {
                Destroy(slots[slot].gameObject); // Destroy Game Object
                slots.Remove(slot); // Removing from Dictionary
            }
        }

        #endregion

        protected override void OnItemUpdated(InventoryItem invItem)
        {
            if (Grid == null)
            {
                //Note, if this happens the InventoryGrid "ItemUpdated" event hasn't been unsubscribed from before being unassigned.
                
                Debug.LogError($"Error: \"{gameObject.name}\" ({name}) attempted to update item when backend" +
                               $" is null, make sure any events for Grid have been unsubscribed from!");
                
                return;
            }
            
            if (!items.ContainsKey(invItem))
            {
                if(!Grid.ContainsItem(invItem)) return;

                // UI Item needs to be created.
                CreateUIItem(invItem);
            }

            if (!Grid.ContainsItem(invItem) && items.TryGetValue(invItem, out InventoryUIItem uiInvItem))
            {
                DeleteUIItem(uiInvItem);
                return;
            }

            InventoryUIItem uiItem = items[invItem];
            
            foreach (Vector2Int slot in uiItem.InvItem.takenPositions)
            {
                slots[slot].containedItem = uiItem;
            }
            
            uiItem.UpdateItem();
        }

        #region Utilities
        
        public override Vector2 GetAveragePosition(IReadOnlyCollection<Vector2> points)
        {
            Vector2 sum = points.Aggregate(Vector2.zero,
                (current, slot) => current + slot);

            return sum / points.Count;
        }
        
        public override Vector2 GetAveragePosition(IEnumerable<Vector2Int> points)
        {
            // Getting UI slot position from each point in collection
            List<Vector2> positions = new ();
            foreach (Vector2Int point in points)
            {
                if (!slots.ContainsKey(point))
                {
                    Debug.LogError($"Error: {gameObject.name} ({name}) attempted to get average position of UI slot that " +
                                   $"wasn't registered! ({point.x},{point.y})");
                    
                    continue;
                }
                positions.Add(slots[point].GetComponent<RectTransform>().anchoredPosition);
            }

            // Returning average of UI slot positions.
            return GetAveragePosition(positions);
        }

        public override Vector2 GridToCellPoint(Vector2 gridPoint)
        {
            // Converting decimal grid point to int to get UI slot from dictionary.
            Vector2Int gridPointInt = new Vector2Int(Mathf.FloorToInt(gridPoint.x), Mathf.FloorToInt(gridPoint.y));
            
            return slots[gridPointInt].GetComponent<RectTransform>().anchoredPosition;
        }

        public InventoryUISlot[] GetSlotsFromPositions(IEnumerable<Vector2Int> positions)
        {
            return (from position in positions
                where slots.ContainsKey(position)
                select slots[position]).ToArray();
        }

        public void UpdateSlotsWithItem(InventoryUIItem uiItem)
        {
            foreach (Vector2Int slot in uiItem.InvItem.takenPositions)
            {
                if (slots.TryGetValue(slot, out InventoryUISlot uiSlot)) uiSlot.containedItem = uiItem;
            }
        }

        #endregion

        #endregion
    }

}