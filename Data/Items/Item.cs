using System;
using System.Collections.Generic;
using KoalaDev.Utilities.Data;
using UnityEngine;

namespace KoalaDev.UGIS
{
    [CreateAssetMenu(fileName = "New Item", menuName = "KoalaDev/UGIS/Items/Base")]
    public abstract class Item : ScriptableObject
    {
        #region --- VARIABLES ---

        [Header("Item Properties")]
        public Vector2Int size = Vector2Int.one;
        public Sprite icon;
        public GameObject worldObject;

        // Stores all the possible interactions related to the item.
        public InteractionProfile InteractionProfile;

        #endregion
    }
    
    [Serializable]
    public abstract class AdditionalItemData
    {
        
    }
}