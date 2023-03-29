using System;
using UnityEngine;

namespace Hitbox.UGIS.Items
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
        public override ItemRuntimeData GetRuntimeData => new StackableItemRuntimeData();

        public override (InventoryItem, InventoryItem) ResolveItemCombine(InventoryItem target, InventoryItem placedItem)
        {
            // --- RETURN CLAUSES ---
            
            // Return if either item doesn't exist
            if (target == null || placedItem == null) return (target, placedItem);
            // Return if items aren't the same
            if (target.Item != placedItem.Item) return (target, placedItem);
            // Return if the target is not stackable
            if (target.Item is not Stackable stackItem) return (target, placedItem);
            
            // --- LOGIC ---
            
            // Getting Stackable Item Data.
            StackableItemRuntimeData stackData = (StackableItemRuntimeData)target.ItemRuntimeData;

            // If no space, return.
            if (stackData.currentStackCount >= stackItem.stackCapacity) return (target, placedItem);

            // Incrementing Stack Count
            stackData.currentStackCount += ((StackableItemRuntimeData)placedItem.ItemRuntimeData).currentStackCount;
            
            return (target, null);
        }

        #endregion
    }

    [Serializable]
    public class StackableItemRuntimeData : ItemRuntimeData
    {
        public int currentStackCount = 1;
    }
}