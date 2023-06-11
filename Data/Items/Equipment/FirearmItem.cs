using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using Hitbox.Inventory.Inventories;
using UnityEngine;

namespace Hitbox.Inventory.Items
{
    [CreateAssetMenu(fileName = "New Firearm Item", menuName = "Hitbox/Inventory/Items/Equipment/Firearm")]
    public class FirearmItem : Item
    {
        #region --- VARIABLES ---

        public FirearmProfile firearmProfile;

        #endregion

        #region --- METHODS ---
        
        public override InventoryItem CreateItem => new InventoryFirearmItem(this);

        #endregion
    }

}