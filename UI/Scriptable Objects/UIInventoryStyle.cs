using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hitbox.UGIS.UI
{
    [CreateAssetMenu(fileName = "New UI Style", menuName = "KoalaDev/UGIS/New UI Style")]
    public class UIInventoryStyle : ScriptableObject
    {
        #region --- VARIABLES ---

        [Header("Grid Objects")] 
        // Prefab used for each slot in the grid.
        public GameObject slotObj;
        // Prefab used for each item in the grid.
        public GameObject itemObj;
        // Prefab used for grid objects
        public GameObject gridObj;

        [Header("Grid Properties")] 
        public Vector2 cellSize = new (60, 60);

        public Vector2 cellSpacing = new (5, 5);
        
        [Header("Item Properties")] 
        public bool highlightOnHover = false;
        public Color slotHighlightColour = Color.white;

        [Header("Context Menu")] 
        public GameObject menuObj;

        public GameObject actionObj;

        #endregion
    }

}