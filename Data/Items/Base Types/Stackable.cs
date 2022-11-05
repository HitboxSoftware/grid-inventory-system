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
    }

    [Serializable]
    public abstract class StackableItemData : AdditionalItemData
    {
        public int currentStackCount;
    }
}