using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hitbox.Inventory.UI
{
    /// <summary>
    /// Abstract class containing all the core functionality for the InventoryUIGrids. Inherit from this to add
    /// custom functionality.
    /// </summary>
    public abstract class InventoryUIAbstractGrid : MonoBehaviour
    {
        #region --- VARIABLES ---
        
        /// <summary>
        /// Backend grid that the UI will represent.
        /// </summary>
        public InventoryGrid Grid { protected set; get; }

        /// <summary>
        /// Contains all data used to determine a grid's appearance.
        /// </summary>
        [SerializeField] protected InventoryUIStyle style;
        
        /// <summary>
        /// Readonly access to the Grid UI style.
        /// </summary>
        public InventoryUIStyle Style => style;

        /// <summary>
        /// Should the grid initiate on start?
        /// </summary>
        [SerializeField] protected bool initialiseOnStart = true;
        
        /// <summary>
        /// Dictionary of the ui items represented in the grid, with the key being the backend item.
        /// </summary>
        protected readonly Dictionary<InventoryItem, InventoryUIItem> items = new ();

        /// <summary>
        /// Rect Transform of the UI grid, used for getting local mouse position within the grid as well as
        /// allowing for dynamically sizing the grid.
        /// </summary>
        protected RectTransform rectTransform;

        #endregion

        #region --- MONOBEHAVIOUR ---

        public virtual void Start()
        {
            if(initialiseOnStart) Init();
        }

        #endregion

        #region --- METHODS ---

        #region Setup

        /// <summary>
        /// This should be called before the grid is used, by default it will run on start.
        /// </summary>
        protected virtual void Init()
        {
            if (!TryGetComponent(out rectTransform))
            {
                Debug.LogError($"Error: {gameObject.name} ({name}) has no RectTransform, this is required, is it a UI Element?");
            }
        }

        #endregion

        #region Creation

        /// <summary>
        /// Link an InventoryGrid to the UI and optionally generate it's UI.
        /// </summary>
        /// <param name="newGrid">new grid to link</param>
        /// <param name="generateUI">whether to generate UI elements</param>
        /// <returns>if assignment was successful.</returns>
        public virtual bool Assign(InventoryGrid newGrid, bool generateUI)
        {
            // - Guard Clauses- 
            if (newGrid == null) // Ensuring given grid exists.
            {
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to assign non-existent InventoryGrid to UI grid!");
                return false;
            }
            
            if (style == null) // Ensuring grid has style.
            {
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to assign an InventoryGrid to a UI grid with no style!");
                return false;
            }

            if (Grid != null) // Clearing Grid if replacing existing grid. 
            {
                ClearUI();
            }

            // Assigning new grid
            Grid = newGrid;

            // Begin listening to grid updates to keep UI parity.
            Grid.Updated += OnItemUpdated;

            Init();

            if (generateUI)
            {
                Generate();
            }

            return true;
        }
        
        // Generates Grid, overwrite will always make a new grid.
        public virtual void Generate()
        {
            if (Grid == null)
            {
                // Cancel Generation.
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to generate grid with no linked InventoryGrid!");
                return;
            }

            // Generate Grid Items, if any found.
            if (Grid?.AllItems != null && Grid.AllItems.Any())
            {
                GenerateItems();
            }
        }
        
        /// <summary>
        /// (Re)Generate all items retrieved from the linked InventoryGrid.
        /// </summary>
        protected virtual void GenerateItems()
        {
            if (Grid == null)
            {
                // Cancel Item Generation.
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to generate items with no linked InventoryGrid!");
                return;
            }
            
            ClearUIItems();
            
            foreach (InventoryItem invItem in Grid.AllItems)
            {
                CreateUIItem(invItem);
            }
        }

        /// <summary>
        /// Update the style of the grid, and regenerate UI if linked to InventoryGrid.
        /// </summary>
        /// <param name="newStyle">New style to assign.</param>
        public virtual void SetStyle(InventoryUIStyle newStyle)
        {
            if (newStyle == null)
            {
                Debug.LogError($"Error: {gameObject.name} ({name}) attempted to set non-existent UI style!");
                return;
            }
            
            style = newStyle;
            
            if (Grid != null)
            {
                Generate();
            }
        }

        /// <summary>
        /// Create a new UI item attached to the grid in the correct position.
        /// </summary>
        /// <param name="invItem">inventory item to create in the UI</param>
        /// <returns>newly created UI item</returns>
        public virtual InventoryUIItem CreateUIItem(InventoryItem invItem)
        {
            GameObject itemObj = Instantiate(style.itemObj, transform);

            // --- Add Item Component ---
            if (!itemObj.TryGetComponent(out InventoryUIItem uiItem)) //Add Item Component if none exists
            {
                uiItem = itemObj.AddComponent<InventoryUIItem>();
            }
            
            items.Add(invItem, uiItem);
            uiItem.AssignUIGrid(this);
            uiItem.AssignItem(invItem);

            return uiItem;
        }

        #endregion

        #region Deletion
        
        /// <summary>
        /// Clear all UI elements, and unlink backend InventoryGrid.
        /// </summary>
        public virtual void ClearUI()
        {
            ClearUIItems();

            // Stop listening to Grids item updates.
            Grid.Updated -= OnItemUpdated;
            
            Grid = null;
        }

        /// <summary>
        /// Removes the given UI item from the grid, doesn't effect backend.
        /// </summary>
        /// <param name="invUIItem">Item to remove</param>
        /// <returns>if successfully removed</returns>
        public virtual bool RemoveUIItem(InventoryUIItem invUIItem)
        {
            // This will auto update this grid via OnItemUpdate call.
            items.Remove(invUIItem.InvItem); // Removing from Dictionary

            return true;
        }

        /// <summary>
        /// Removes the given UI item from the grid and destroys it
        /// </summary>
        /// <param name="targetItem"></param>
        /// <returns>if item deleted</returns>
        public virtual void DeleteUIItem(InventoryUIItem targetItem)
        {
            RemoveUIItem(targetItem);
            
            //TODO: Implement object pooling
            Destroy(targetItem.gameObject);
        }

        /// <summary>
        /// Deletes every registered UI item.
        /// </summary>
        public virtual void ClearUIItems()
        {
            if (items == null || items.Count == 0) return;

            List<InventoryUIItem> itemList = items.Values.ToList();

            foreach (InventoryUIItem uiInventoryItem in itemList)
            {
                DeleteUIItem(uiInventoryItem);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Called anytime the backend grid is updated to keep parity with backend, usually linked to InventoryGrid's "ItemUpdated" event.
        /// </summary>
        /// <param name="invItem">Inventory item that was updated</param>
        protected virtual void OnItemUpdated(InventoryItem invItem)
        {
            if (Grid == null)
            {
                //Note, if this happens the InventoryGrid "ItemUpdated" event hasn't been unsubscribed from before being unassigned.
                
                Debug.LogError($"Error: \"{gameObject.name}\" ({name}) attempted to update item when backend" +
                               $" is null, make sure any events for Grid have been unsubscribed from!");
                
                return;
            }
            
            if (!items.ContainsKey(invItem)) // Checking if UI item should be added.
            {
                if (!Grid.ContainsItem(invItem)) return; // Don't create item, ItemUpdate call was for removal.

                // UI Item needs to be created.
                CreateUIItem(invItem);
            }

            // Checking if UI item should be removed.
            if (!Grid.ContainsItem(invItem) && items.TryGetValue(invItem, out InventoryUIItem uiInvItem))
            {
                DeleteUIItem(uiInvItem);
                return;
            }
            
            // If UI item already exists for given item, update the item.
            uiInvItem = items[invItem];
            uiInvItem.UpdateItem();
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Returns the average of the given positions.
        /// </summary>
        /// <param name="points">points to average</param>
        /// <returns>Average position of the collection</returns>
        public virtual Vector2 GetAveragePosition(IReadOnlyCollection<Vector2> points)
        {
            Vector2 sum = points.Aggregate(Vector2.zero,
                (current, slot) => current + GridToCellPoint(slot));

            return sum / points.Count;
        }

        /// <summary>
        /// Returns the average position of the given integer points
        /// </summary>
        /// <param name="points">integer points to average</param>
        /// <returns>Average position of the collection</returns>
        public virtual Vector2 GetAveragePosition(IEnumerable<Vector2Int> points)
        {
            // Converting the Vector2Int collection into Vector2s, as they can't be converted.
            List<Vector2> convertedPositions = new List<Vector2>();
            foreach (Vector2Int position in points)
            {
                convertedPositions.Add(position);
            }

            // Returning the average position of the now converted positions.
            return GetAveragePosition(convertedPositions);
        }

        /// <summary>
        /// Calculates the cell position from the given grid position, taking into account
        /// cell size and spacing. Used for UI calculations.
        /// </summary>
        /// <param name="gridPoint">grid position to convert</param>
        /// <returns>local position of the given grid position</returns>
        public virtual Vector2 GridToCellPoint(Vector2 gridPoint)
        {
            if (rectTransform == null)
            {
                Debug.LogError($"Error: \"{gameObject.name}\" ({name}) attempted to convert grid point to cell point with no RectTransform set!");
                return Vector2.zero;
            }

            //TODO: Make more dynamic, i.e. have pivot directly effect the grid point.
            // Retrieving size of each cell.
            Vector2 cellSize = CalculateCellSize();
            // Calculating cell point, assuming pivot is top left.
            Vector2 cellPoint = new Vector2(
                x: cellSize.x * (gridPoint.x + 0.5f), // If pivot is on the right, this should be negated
                y: cellSize.y * -(gridPoint.y + 0.5f) // If pivot is on the top, this should be negated
            );

            return cellPoint;
        }

        /// <summary>
        /// Converts a cell position into the nearest grid position, used in the backend calculations.
        /// </summary>
        /// <param name="cellPosition">cell position to convert</param>
        /// <returns>Closest grid position to the given cell position</returns>
        public virtual Vector2Int CellToGridPoint(Vector2 cellPosition)
        {
            Vector2 cellSize = CalculateCellSize();
            Vector2 pos = cellPosition / cellSize;

            Vector2Int gridPos = new Vector2Int(
                x: Mathf.FloorToInt(pos.x),
                y: Mathf.FloorToInt(pos.y)
            );

            return gridPos;
        }

        /// <summary>
        /// Simplifies the conversion, if using PointerEventData use pressEventCamera for clicks and enterEventCamera for entries.
        /// </summary>
        /// <param name="screenPoint">Screen position to convert</param>
        /// <param name="cam">Camera used to get screen point</param>
        /// <param name="invertX">Whether the x position should be negated, false by default</param>
        /// <param name="invertY">Whether the y position should be negated, true by default</param>
        /// <returns>screen position in local space of UI grid</returns>
        public virtual Vector2 ScreenToLocalPoint(Vector2 screenPoint, Camera cam, bool invertX = false, bool invertY = true)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, cam, out Vector2 localPoint);

            if (invertX) localPoint.x = -localPoint.x;
            if (invertY) localPoint.y = -localPoint.y;

            return localPoint;
        }

        /// <summary>
        /// Calculates the current cell size to determine UI slot and item sizes.
        /// </summary>
        /// <returns>current cell size for the grid</returns>
        public virtual Vector2 CalculateCellSize()
        {
            if (Grid == null)
            {
                Debug.LogError($"Error: \"{gameObject.name}\" ({name}) attempted to calculate cell size with no InventoryGrid set!");
                return Vector2.one;
            }
            
            return rectTransform.rect.size / Grid.size;
            
            //Code for accounting for cell spacing, only really relevant in gridlayout based inventories.
            //UIGrid.Style.cellSpacing.y * (invItem.Size.y - 1)
        }

        #endregion

        #endregion
    }

}