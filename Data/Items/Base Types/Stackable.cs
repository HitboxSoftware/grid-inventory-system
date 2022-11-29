using System;
using UnityEngine;

namespace KoalaDev.UGIS.Items
{
    public abstract class Stackable : Item
    {
        /*
         * This sub-class makes an item stackable, this is kept separate from Item due to conflicts with Container and
         * Durability. Currently thinking about making a ComplexStackable item that supports Containers and Durability
         * but this is not coming soon. I'm sure you can figure it out, but i'd probably just use a List to represent
         * a stack of items, all of which can be durable or containers without issue.
         */
        #region --- VARIABLES ---

        public int stackCapacity;

        #endregion

        #region --- METHODS ---
        public override AdditionalItemData GetAdditionalData => new StackableItemData();

        public override (InventoryItem, InventoryItem) ItemToItem(InventoryItem invItem1, InventoryItem invItem2)
        {
            // --- RETURN CLAUSES ---
            
            // Return if either item doesn't exist
            if (invItem1 == null || invItem2 == null) return (invItem1, invItem2);
            // Return if items aren't the same
            if (invItem1.Item != invItem2.Item) return (invItem1, invItem2);
            // Return if the target is not stackable
            if (invItem1.Item is not Stackable stackItem) return (invItem1, invItem2);
            
            // --- LOGIC ---
            
            // Getting Stackable Item Data.
            StackableItemData stackData = (StackableItemData)invItem1.ItemData;

            // If no space, return.
            if (stackData.currentStackCount >= stackItem.stackCapacity) return (invItem1, invItem2);

            // Incrementing Stack Count
            stackData.currentStackCount += ((StackableItemData)invItem2.ItemData).currentStackCount;
            
            return (invItem1, null);
        }

        #endregion
    }

    [Serializable]
    public class StackableItemData : AdditionalItemData
    {
        public int currentStackCount = 1;
    }
}