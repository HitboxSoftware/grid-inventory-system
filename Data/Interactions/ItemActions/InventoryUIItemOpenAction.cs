using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash;
using Hitbox.Stash.Items;
using UnityEngine;

namespace Hitbox.Stash.UI.Actions
{
    [CreateAssetMenu(fileName = "New Item Open Action", menuName = "Hitbox/Inventory/Actions/Open")]

    public class InventoryUIItemOpenAction : InventoryUIItemAction
    {
        #region Methods

        public override bool IsCompatible(InventoryItem invItem)
        {
            if (!base.IsCompatible(invItem)) return false;

            if (invItem is InventoryContainerItem) return true;

            return false;
        }

        #endregion
    }

}