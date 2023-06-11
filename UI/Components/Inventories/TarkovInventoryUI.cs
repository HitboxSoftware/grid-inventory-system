using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using Hitbox.Inventory.Categories;
using Hitbox.Inventory.Inventories;
using Hitbox.Inventory.UI;
using UnityEngine;

public class TarkovInventoryUI : AbstractInventoryUI
{
    #region --- VARIABLES ---

    public TarkovInventory inventory;
    private InventoryUIStyle _style;

    [SerializeField] private bool buildOnStart;
    
    // Components
    
    // Grids
    [SerializeField] private InventoryUIGridGroup pockets;
    
    // - Inventory Slots -
    // Container Slots
    [SerializeField] private InventoryUIItemSlot rigSlot;
    [SerializeField] private ItemCategory rigCategory;
    [SerializeField] private InventoryUIItemSlot backpackSlot;
    [SerializeField] private ItemCategory backpackCategory;
    [SerializeField] private InventoryUIItemSlot pouchSlot;
    [SerializeField] private ItemCategory pouchCategory;

    // Weapon Slots
    [SerializeField] private InventoryUIItemSlot slingSlot; // Any Weapon, Excluding Pistols & Melee
    [SerializeField] private ItemCategory slingCategory;
    [SerializeField] private InventoryUIItemSlot backSlot; // Any Weapon, Excluding Pistols & Melee
    [SerializeField] private ItemCategory backCategory;
    [SerializeField] private InventoryUIItemSlot holsterSlot; // Pistol only
    [SerializeField] private ItemCategory holsterCategory;
    [SerializeField] private InventoryUIItemSlot scabbardSlot;
    [SerializeField] private ItemCategory scabbardCategory;

    // Clothing Slots
    [SerializeField] private InventoryUIItemSlot headwearSlot;
    [SerializeField] private ItemCategory headwearCategory;
    [SerializeField] private InventoryUIItemSlot armorSlot;
    [SerializeField] private ItemCategory armorCategory;
    [SerializeField] private InventoryUIItemSlot faceSlot;
    [SerializeField] private ItemCategory faceCategory;
    [SerializeField] private InventoryUIItemSlot eyewearSlot;
    [SerializeField] private ItemCategory eyewearCategory;
    [SerializeField] private InventoryUIItemSlot earpieceSlot;
    [SerializeField] private ItemCategory earpieceCategory;
    [SerializeField] private InventoryUIItemSlot armbandSlot;
    [SerializeField] private ItemCategory armbandCategory;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void Start()
    {
        if(buildOnStart) Build();
    }

    #endregion

    #region --- METHODS ---

    public override void Build()
    {
        inventory = new TarkovInventory(rigCategory, backpackCategory, pouchCategory, slingCategory, backCategory,
            holsterCategory, scabbardCategory, headwearCategory, armorCategory, faceCategory, eyewearCategory,
            earpieceCategory, armbandCategory);
        pockets.SetGrids(new InventoryGridGroup(4, Vector2Int.one), new []{4}, true);
        
        if (rigSlot != null) rigSlot.SetSlot(inventory.rigSlot);
        if (backpackSlot != null) backpackSlot.SetSlot(inventory.backpackSlot);
        if (pouchSlot != null) pouchSlot.SetSlot(inventory.pouchSlot);
        if (slingSlot != null) slingSlot.SetSlot(inventory.slingSlot);
        if (backSlot != null) backSlot.SetSlot(inventory.backSlot);
        if (holsterSlot != null) holsterSlot.SetSlot(inventory.holsterSlot);
        if (scabbardSlot != null) scabbardSlot.SetSlot(inventory.scabbardSlot);
        if (headwearSlot != null) headwearSlot.SetSlot(inventory.headgearSlot);
        if (armorSlot != null) armorSlot.SetSlot(inventory.armorSlot);
        if (faceSlot != null) faceSlot.SetSlot(inventory.faceSlot);
        if (eyewearSlot != null) eyewearSlot.SetSlot(inventory.eyewearSlot);
        if (earpieceSlot != null) earpieceSlot.SetSlot(inventory.earpieceSlot);
        if (armbandSlot != null) armbandSlot.SetSlot(inventory.armbandSlot);
    }

    public void Build(TarkovInventory newInventory)
    {
        if (newInventory == null) return;
        
        inventory = newInventory;

        pockets.SetGrids(new InventoryGridGroup(4, Vector2Int.one), new []{4}, true);

        if (rigSlot != null) rigSlot.SetSlot(inventory.rigSlot);
        if (backpackSlot != null) backpackSlot.SetSlot(inventory.backpackSlot);
        if (pouchSlot != null) pouchSlot.SetSlot(inventory.pouchSlot);
        if (slingSlot != null) slingSlot.SetSlot(inventory.slingSlot);
        if (backSlot != null) backSlot.SetSlot(inventory.backSlot);
        if (holsterSlot != null) holsterSlot.SetSlot(inventory.holsterSlot);
        if (scabbardSlot != null) scabbardSlot.SetSlot(inventory.scabbardSlot);
        if (headwearSlot != null) headwearSlot.SetSlot(inventory.headgearSlot);
        if (armorSlot != null) armorSlot.SetSlot(inventory.armorSlot);
        if (faceSlot != null) faceSlot.SetSlot(inventory.faceSlot);
        if (eyewearSlot != null) eyewearSlot.SetSlot(inventory.eyewearSlot);
        if (earpieceSlot != null) earpieceSlot.SetSlot(inventory.earpieceSlot);
        if (armbandSlot != null) armbandSlot.SetSlot(inventory.armbandSlot);
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override bool TryEquipItem(InventoryItem item)
    {
        if (rigSlot.LinkedSlot.InsertItem(item)) return true;
        if (backpackSlot.LinkedSlot.InsertItem(item)) return true;
        if (pouchSlot.LinkedSlot.InsertItem(item)) return true;
        if (slingSlot.LinkedSlot.InsertItem(item)) return true;
        if (backSlot.LinkedSlot.InsertItem(item)) return true;
        if (holsterSlot.LinkedSlot.InsertItem(item)) return true;
        if (scabbardSlot.LinkedSlot.InsertItem(item)) return true;
        if (headwearSlot.LinkedSlot.InsertItem(item)) return true;
        if (armorSlot.LinkedSlot.InsertItem(item)) return true;
        if (faceSlot.LinkedSlot.InsertItem(item)) return true;
        if (eyewearSlot.LinkedSlot.InsertItem(item)) return true;
        if (earpieceSlot.LinkedSlot.InsertItem(item)) return true;
        if (armbandSlot.LinkedSlot.InsertItem(item)) return true;
        
        return false;
    }

    #endregion
}
