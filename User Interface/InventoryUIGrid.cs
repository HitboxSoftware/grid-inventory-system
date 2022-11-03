using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KoalaDev.UGIS.UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class InventoryUIGrid : MonoBehaviour
    {
        /*
         * This script is responsible for taking the Data from InventoryGrid and converting into
         * Unity UI Elements, it's been seperated this way to allow for compatability with other UI systems.
         */
        
        #region --- VARIABLES ---

        // --- Backend ---
        
        /*
         * Inventory Grid Info
         * If grid is unassigned it will attempt GetComponent, if none found, it will wait until one
         * is assigned via AssignGrid(). This should be useful if, for example, you want to have 
         * one grid for opening world containers.
        */
        
        [SerializeField] private InventoryGrid grid;

        // --- User Interface ---
        [SerializeField] private InventoryUIStyle style;
        
        // - Components
        private GridLayoutGroup gridLayout;
        private ContentSizeFitter sizeFitter;

        // - Generated Objects -

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            Generate();
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


            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = grid.size.x;
            
            if (TryGetComponent(out sizeFitter))
            {
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        // Generates the Grid UI
        public bool Generate()
        {
            GenerateSlots();
            return true;
        }

        // Generates all the slots in the UI.
        public bool GenerateSlots()
        {
            for (int y = 0; y < grid.size.y; y++)
            {
                for (int x = 0; x < grid.size.x; x++)
                {
                    // --- Create Slot Object ---
                    GameObject slotObj = Instantiate(style.slotObj, transform);
                    slotObj.name = "Slot [" + x + "," + y + "]"; //Set name to Slot Position
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

            return true;
        }

        public bool GenerateItems()
        {
            return true;
        }

        // Assigns new grid and goes through GridUI initialization.
        public void AssignGrid(InventoryGrid newGrid, bool generateUI)
        {
            if (newGrid == null) return;

            grid = newGrid;
            
            Init();

            if (generateUI) 
                Generate();
        }

        #endregion
    }

}