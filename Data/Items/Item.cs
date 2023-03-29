using System;
using System.Collections.Generic;
using Hitbox.Utilities.Data;
using UnityEngine;

namespace Hitbox.UGIS
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Hitbox/UGIS/Items/Base")]
    public abstract class Item : ScriptableObject
    {
        #region --- VARIABLES ---

        [Header("Item Properties")] public Vector2Int size = Vector2Int.one;
        public Sprite icon;
        public GameObject worldObject;
        
        // Stores all the possible interactions related to the item.
        public InteractionProfile interactionProfile;
        
        // --- EVENTS ---
        public event Action OnUpdate;

        #endregion
        
        #region --- METHODS ---

        public virtual ItemRuntimeData GetRuntimeData => new ();
        public virtual (InventoryItem, InventoryItem) ResolveItemCombine(InventoryItem target, InventoryItem placedItem) { return (target, placedItem); }

        public void Updated()
        {
            OnUpdate?.Invoke();
        }
        
        #endregion
    }

    [Serializable]
    public class ItemRuntimeData
    {
        public bool rotated;
    }
}