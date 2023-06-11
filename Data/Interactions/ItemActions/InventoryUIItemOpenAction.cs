using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.Items;
using UnityEngine;

namespace Hitbox.Inventory.UI.Actions
{
    [CreateAssetMenu(fileName = "New Item Open Action", menuName = "Hitbox/Inventory/Actions/Open")]

    public class InventoryUIItemOpenAction : InventoryUIItemAction
    {
        #region --- METHODS ---

        public override bool SupportsAction(InventoryItem invItem)
        {
            if (!base.SupportsAction(invItem)) return false;

            if (invItem is InventoryContainerItem) return true;

            return false;
        }

        #endregion
    }

}