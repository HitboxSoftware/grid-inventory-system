using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hitbox.UGIS
{
    public class InventoryItem : ISerializationCallbackReceiver
    {
        #region --- VARIABLES ---

        public readonly Item Item;

        public Vector2Int[] TakenSlots; // All of the slots taken by this item. Index 0 is top left.

        private InventoryGrid parentGrid;

        #region - RUNTIME DATA -

        [NonSerialized]
        public ItemRuntimeData ItemRuntimeData; // Runtime Data of the given item.
        private string additionalItemData; // JSON Serialized ItemData
        private string itemDataType;

        #endregion

        #endregion

        #region --- METHODS ---

        public bool InSlot(Vector2Int slot) => TakenSlots.Contains(slot);

        public bool InGrid(InventoryGrid grid) => grid == parentGrid;

        public void SetGrid(InventoryGrid grid) => parentGrid = grid;

        public Vector2Int Size => ItemRuntimeData.rotated ? 
            new Vector2Int(Item.size.y, Item.size.x) : Item.size;

        #region --- CONSTRUCTOR ---
        
        public InventoryItem(Item item, InventoryGrid parentGrid, Vector2Int[] takenSlots = null, ItemRuntimeData itemData = null)
        {
            Item = item;
            TakenSlots = takenSlots;
            this.parentGrid = parentGrid;
            
            ItemRuntimeData = itemData ?? Item.GetRuntimeData;
        }

        #endregion
        
        #region --- SERIALISATIONCALLBACKS ---

        public void OnBeforeSerialize()
        {
            if (ItemRuntimeData == null) return;
            additionalItemData = JsonUtility.ToJson(ItemRuntimeData); 
            itemDataType = ItemRuntimeData.GetType().ToString(); // Used to get object type when deserializing data.
        }

        public void OnAfterDeserialize()
        {
            Type type = Type.GetType(itemDataType);
            ItemRuntimeData = (ItemRuntimeData)JsonUtility.FromJson(additionalItemData, type);
        }

        #endregion

        #endregion
    }

}