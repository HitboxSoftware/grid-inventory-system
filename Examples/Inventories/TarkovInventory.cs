using System;
using Hitbox.Inventory.Categories;
using Hitbox.Inventory.Items;
using Newtonsoft.Json;
using UnityEngine;

namespace Hitbox.Inventory.Inventories
{
    /// <summary>
    /// Inventory with a style similar to Tarkovs' system.
    /// </summary>
    public class TarkovInventory : Inventory
    {
        #region --- VARIABLES ---
        
        public InventoryGridGroup pockets = new InventoryGridGroup(4, Vector2Int.one);
        public InventoryGridGroup special = new InventoryGridGroup(3, Vector2Int.one);
        
        // - Inventory Slots -
        // Container Slots
        public InventoryItemSlot rigSlot;
        public InventoryItemSlot backpackSlot;
        public InventoryItemSlot pouchSlot;
        
        // Weapon Slots
        public InventoryItemSlot slingSlot; // Any Weapon, Excluding Pistols & Melee
        public InventoryItemSlot backSlot; // Any Weapon, Excluding Pistols & Melee
        public InventoryItemSlot holsterSlot; // Pistol only
        public InventoryItemSlot scabbardSlot;

        // Clothing Slots
        public InventoryItemSlot headgearSlot;
        public InventoryItemSlot armorSlot;
        public InventoryItemSlot faceSlot;
        public InventoryItemSlot eyewearSlot;
        public InventoryItemSlot earpieceSlot;
        public InventoryItemSlot armbandSlot;

        #endregion

        #region --- METHODS ---

        public override bool InsertItem(InventoryItem invItem)
        {
            if (invItem.item is null) return false;
            
            // Gear
            if (rigSlot.InsertItem(invItem)) return true;
            if (backpackSlot.InsertItem(invItem)) return true;
            if (pouchSlot.InsertItem(invItem)) return true;
            
            // Weapons
            if (slingSlot.InsertItem(invItem)) return true;
            if (backSlot.InsertItem(invItem)) return true;
            if (holsterSlot.InsertItem(invItem)) return true;
            if (scabbardSlot.InsertItem(invItem)) return true;
            
            // Clothing
            if (headgearSlot.InsertItem(invItem)) return true;
            if (armorSlot.InsertItem(invItem)) return true;
            if (faceSlot.InsertItem(invItem)) return true;
            if (eyewearSlot.InsertItem(invItem)) return true;
            if (earpieceSlot.InsertItem(invItem)) return true;
            if (armbandSlot.InsertItem(invItem)) return true;

            if (rigSlot.HasItem() && rigSlot.AttachedItem is InventoryContainerItem)
            {
                InventoryContainerItem containerInvItem = (InventoryContainerItem)rigSlot.AttachedItem;
                
                if (containerInvItem.gridGroup.InsertElement(invItem)) return true;
            }
            
            if (backpackSlot.HasItem() && backpackSlot.AttachedItem is InventoryContainerItem)
            {
                InventoryContainerItem containerInvItem = (InventoryContainerItem)backpackSlot.AttachedItem;
                
                if (containerInvItem.gridGroup.InsertElement(invItem)) return true;
            }
            
            if (pouchSlot.HasItem() && pouchSlot.AttachedItem is InventoryContainerItem)
            {
                InventoryContainerItem containerInvItem = (InventoryContainerItem)pouchSlot.AttachedItem;
                
                if (containerInvItem.gridGroup.InsertElement(invItem)) return true;
            }

            return false;
        }

        public override bool ContainsItem(InventoryItem invItem)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsItem(Item item)
        {
            throw new NotImplementedException();
        }

        public override InventoryItem GetItem(Item item)
        {
            throw new NotImplementedException();
        }

        public override bool RemoveItem(InventoryItem invItem)
        {
            throw new NotImplementedException();
        }

        #region --- CONSTRUCTORS ---

        /// <summary>
        /// Generates an inventory from given data, slot categories will not be saved and should be done after loading
        /// </summary>
        /// <param name="data">Data to load</param>
        public TarkovInventory(TarkovInventoryData data)
        {
            rigSlot = data.rig;
            backpackSlot = data.backpack;
            pouchSlot = data.pouch;
            slingSlot = data.sling;
            backSlot = data.back;
            holsterSlot = data.holster;
            scabbardSlot = data.scabbard;
            headgearSlot = data.headgear;
            armorSlot = data.armor;
            faceSlot = data.face;
            eyewearSlot = data.eyewear;
            earpieceSlot = data.earpiece;
            armbandSlot = data.armband;
        }

        public TarkovInventory(InventoryItemSlot rigSlot, InventoryItemSlot backpackSlot, InventoryItemSlot pouchSlot,
            InventoryItemSlot slingSlot, InventoryItemSlot backSlot, InventoryItemSlot holsterSlot,
            InventoryItemSlot scabbardSlot, InventoryItemSlot headgearSlot, InventoryItemSlot armorSlot,
            InventoryItemSlot faceSlot, InventoryItemSlot eyewearSlot, InventoryItemSlot earpieceSlot,
            InventoryItemSlot armbandSlot)
        {
            this.rigSlot = rigSlot;
            this.backpackSlot = backpackSlot;
            this.pouchSlot = pouchSlot;
            this.slingSlot = slingSlot;
            this.backSlot = backSlot;
            this.holsterSlot = holsterSlot;
            this.scabbardSlot = scabbardSlot;
            this.headgearSlot = headgearSlot;
            this.armorSlot = armorSlot;
            this.faceSlot = faceSlot;
            this.eyewearSlot = eyewearSlot;
            this.earpieceSlot = earpieceSlot;
            this.armbandSlot = armbandSlot;
        }

        public TarkovInventory(ItemCategory rigCategory, ItemCategory backpackCategory, ItemCategory pouchCategory, ItemCategory slingCategory,
            ItemCategory backCategory, ItemCategory holsterCategory, ItemCategory sheathCategory, ItemCategory helmetCategory, ItemCategory vestCategory,
            ItemCategory faceCategory, ItemCategory glassesCategory, ItemCategory earpieceCategory, ItemCategory armbandCategory)
        {
            rigSlot = new InventoryItemSlot(rigCategory);
            backpackSlot = new InventoryItemSlot(backpackCategory);
            pouchSlot = new InventoryItemSlot(pouchCategory);
            slingSlot = new InventoryItemSlot(slingCategory);
            backSlot = new InventoryItemSlot(backCategory);
            holsterSlot = new InventoryItemSlot(holsterCategory);
            scabbardSlot = new InventoryItemSlot(sheathCategory);
            headgearSlot = new InventoryItemSlot(helmetCategory);
            armorSlot = new InventoryItemSlot(vestCategory);
            faceSlot = new InventoryItemSlot(faceCategory);
            eyewearSlot = new InventoryItemSlot(glassesCategory);
            earpieceSlot = new InventoryItemSlot(earpieceCategory);
            armbandSlot = new InventoryItemSlot(armbandCategory);
        }

        #endregion

        #endregion
    }

    public class  TarkovInventoryData : InventoryData
    {
        #region --- VARIABLES ---

        public InventoryGridGroupData pockets;
        public InventoryGridGroupData special;

        public InventoryItemSlotData rig;
        public InventoryItemSlotData backpack;
        public InventoryItemSlotData pouch;
        public InventoryItemSlotData sling;
        public InventoryItemSlotData back;
        public InventoryItemSlotData holster;
        public InventoryItemSlotData scabbard;
        public InventoryItemSlotData headgear;
        public InventoryItemSlotData armor;
        public InventoryItemSlotData face;
        public InventoryItemSlotData eyewear;
        public InventoryItemSlotData earpiece;
        public InventoryItemSlotData armband;

        #endregion

        #region --- CONSTRUCTOR ---

        public TarkovInventoryData(TarkovInventory inventory)
        {
            pockets = inventory.pockets;
            special = inventory.special;
            
            rig = inventory.rigSlot;
            backpack = inventory.backpackSlot;
            pouch = inventory.pouchSlot;
            sling = inventory.slingSlot;
            back = inventory.backSlot;
            holster = inventory.holsterSlot;
            scabbard = inventory.scabbardSlot;
            headgear = inventory.headgearSlot;
            armor = inventory.armorSlot;
            face = inventory.faceSlot;
            eyewear = inventory.eyewearSlot;
            earpiece = inventory.earpieceSlot;
            armband = inventory.armbandSlot;
        }

        [JsonConstructor]
        public TarkovInventoryData(InventoryGridGroupData pockets, InventoryGridGroupData special, InventoryItemSlotData rig, InventoryItemSlotData backpack, InventoryItemSlotData pouch, InventoryItemSlotData sling, InventoryItemSlotData back, InventoryItemSlotData holster, InventoryItemSlotData scabbard, InventoryItemSlotData headgear, InventoryItemSlotData armor, InventoryItemSlotData face, InventoryItemSlotData eyewear, InventoryItemSlotData earpiece, InventoryItemSlotData armband)
        {
            this.pockets = pockets;
            this.special = special;
            this.rig = rig;
            this.backpack = backpack;
            this.pouch = pouch;
            this.sling = sling;
            this.back = back;
            this.holster = holster;
            this.scabbard = scabbard;
            this.headgear = headgear;
            this.armor = armor;
            this.face = face;
            this.eyewear = eyewear;
            this.earpiece = earpiece;
            this.armband = armband;
        }

        #endregion
        
        #region --- CONVERSION ---

        public static implicit operator TarkovInventoryData(TarkovInventory inventory) => new (inventory);

        #endregion
        
    }

}