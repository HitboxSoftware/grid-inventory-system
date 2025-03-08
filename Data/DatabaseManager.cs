using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash;
using UnityEngine;

public class DatabaseManager
{
    #region Fields

    private static DatabaseManager _instance;
    public static DatabaseManager Instance // Get the active instance, or create one if none exists. NOT THREAD SAFE. 
    {
        get { return _instance ??= new DatabaseManager(); }
    }

    private Dictionary<string, ItemDatabase> _itemDatabases = new ();

    #endregion
    
    #region Methods

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

    #region Constructors
    
    protected DatabaseManager() { }

    #endregion
}
