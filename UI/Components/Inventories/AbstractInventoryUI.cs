using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.UI;
using UnityEngine;

public abstract class AbstractInventoryUI : MonoBehaviour
{
    #region --- VARIABLES ---

    private Inventory _inventory;

    #endregion

    #region --- METHODS ---

    public abstract void Build();
    public abstract void Clear();

    public abstract bool TryEquipItem(InventoryItem item);

    #endregion
}
