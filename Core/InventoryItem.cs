using System;
using System.Linq;
using UnityEngine;

namespace KoalaDev.UGIS
{
    public class InventoryItem : ISerializationCallbackReceiver
    {
        #region --- VARIABLES ---

        public readonly Item Item;

        public Vector2Int[] TakenSlots; // All of the slots taken by this item. Index 0 is top left.

        private InventoryGrid parentGrid;

        #region - RUNTIME DATA -

        [NonSerialized]
        public AdditionalItemData ItemData; // Runtime Data of the given item.
        private string additionalItemData; // JSON Serialized ItemData
        private string itemDataType;

        #endregion

        #endregion

        #region --- METHODS ---

        public bool InSlot(Vector2Int slot) => TakenSlots.Contains(slot);

        public bool InGrid(InventoryGrid grid) => grid == parentGrid;

        public void SetGrid(InventoryGrid grid) => parentGrid = grid;

        #region --- CONSTRUCTOR ---
        
        public InventoryItem(Item item, Vector2Int[] takenSlots, InventoryGrid parentGrid)
        {
            Item = item;
            TakenSlots = takenSlots;
            this.parentGrid = parentGrid;
        }

        #endregion
        
        #region --- SERIALISATIONCALLBACKS ---

        public void OnBeforeSerialize()
        {
            if (ItemData == null) return;
            additionalItemData = JsonUtility.ToJson(ItemData); 
            itemDataType = ItemData.GetType().ToString(); // Used to get object type when deserializing data.
        }

        public void OnAfterDeserialize()
        {
            Type type = Type.GetType(itemDataType);
            ItemData = (AdditionalItemData)JsonUtility.FromJson(additionalItemData, type);
        }

        #endregion

        #endregion
    }

}