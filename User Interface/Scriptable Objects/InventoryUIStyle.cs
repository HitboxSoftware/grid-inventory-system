using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KoalaDev.UGIS.UI
{
    [CreateAssetMenu(fileName = "New UI Style", menuName = "KoalaDev/UGIS/New UI Style")]
    public class InventoryUIStyle : ScriptableObject
    {
        #region --- VARIABLES ---

        [Header("Grid Objects")] 
        // Prefab used for each slot in the grid.
        public GameObject slotObj;
        // Prefab used for each item in the grid.
        public GameObject itemObj;

        [Header("Item Properties")] 
        public bool highlightOnHover = false;
        public Color slotHighlightColour = Color.white;

        [Header("Context Menu")] 
        public GameObject menuObj;

        public GameObject actionObj;

        #endregion
    }

}