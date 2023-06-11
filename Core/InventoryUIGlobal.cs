using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.UI;
using UnityEngine;

public static class InventoryUIGlobal
{
    #region --- VARIABLES ---

    private static InventoryUIManager _instance;
    private static bool _hasInstance;
    
    #endregion

    #region --- METHODS ---

    /// <summary>
    /// Sets the active inventory manager instance.
    /// </summary>
    public static void SetInstance(InventoryUIManager inventoryUIManager)
    {
        _hasInstance = inventoryUIManager != null;
        _instance = inventoryUIManager;
    }

    /// <summary>
    /// Try and get a reference to the active InventoryUIManager instance.
    /// </summary>
    /// <param name="manager"></param>
    /// <returns></returns>
    public static bool TryGetInstance(out InventoryUIManager manager)
    {
        if (_hasInstance)
        {
            manager = _instance;
            return true;
        }

        Debug.LogWarning("Attempted to get instance with no instance set, make sure a UIInventoryManager is in the scene!");
        manager = default;
        return false;
    }

    /// <summary>
    /// Tries to get the current style from the active instance.
    /// </summary>
    /// <param name="style">Output found style from the instance.</param>
    /// <returns>True if manager instance exists, false otherwise</returns>
    public static bool TryGetStyle(out InventoryUIStyle style)
    {
        if (_hasInstance)
        {
            style = _instance.inventoryStyle;
            return true;
        }

        Debug.LogWarning("Attempted to get inventory style with no instance set, make sure a UIInventoryManager is in the scene!");
        style = default;
        return false;
    }

    #endregion
}
