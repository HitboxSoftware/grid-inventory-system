using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.UGIS.Items
{
    public class Damageable : Item
    {
        #region --- VARIABLES ---

        public int maxDurability = 100;

        #endregion

        #region --- METHODS ---

        public override ItemRuntimeData GetRuntimeData => new DamageableItemRuntimeData();

        #endregion
    }

    [Serializable]
    public class DamageableItemRuntimeData : ItemRuntimeData
    {
        public float currentDurability;
    }

}