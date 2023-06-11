using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using UnityEngine;

public class DatabaseManager
{
    #region --- VARIABLES ---

    private static DatabaseManager _instance;
    public static DatabaseManager Instance // Get the active instance, or create one if none exists. NOT THREAD SAFE. 
    {
        get { return _instance ??= new DatabaseManager(); }
    }

    private Dictionary<string, ItemDatabase> _itemDatabases = new ();

    #endregion
    
    #region --- METHODS ---

    public bool InsertDatabase(ItemDatabase itemDatabase)
    {
        _itemDatabases.Add(itemDatabase.name, itemDatabase);
        return false;
    }
    public ItemDatabase GetItemDatabase(string name)
    {
        if (!_itemDatabases.ContainsKey(name)) return null;

        return _itemDatabases[name];
    }

    #endregion

    #region --- CONSTRUCTORS ---
    
    protected DatabaseManager() { }

    #endregion
}
