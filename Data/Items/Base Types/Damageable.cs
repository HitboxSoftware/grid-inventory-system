using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoalaDev.UGIS.Items
{
    public class Damageable : Item
    {
        #region --- VARIABLES ---

        public int maxDurability = 100;

        #endregion
    }

    [Serializable]
    public abstract class DamageableItemData : AdditionalItemData
    {
        public float currentDurability;
    }

}