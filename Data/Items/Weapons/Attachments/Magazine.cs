using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.UGIS.Items.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Magazine", menuName = "Hitbox/UGIS/Items/Weapons/Attachments/Magazine")]
    public class Magazine : Attachment
    {
        #region --- VARIABLES ---

        [Header("Magazine Properties")]
        public int capacity;

        // Array as some magazines support multiple cartridges (556x45 & .300blk)
        public Caliber[] acceptedCartridges; 

        #endregion

        #region --- METHODS ---

        #region - MAGAZINE UTILITIES -

        // Clears, then Fills magazine to given value OR max capacity.
        public static void FillMagazine(Magazine mag, MagazineItemRuntimeData magData, Bullet bullet, int count = 0)
        {
            magData.MagazineStack.Clear();

            // Fill Magazine to Capacity if Count 0 or lower.
            if (count <= 0) count = mag.capacity;

            for (int i = 0; i < Mathf.Clamp(count, 0, mag.capacity); i++)
            {
                magData.MagazineStack.Push(bullet);
            }
        }

        // Loads Magazine with Given Bullet.
        public static bool LoadBullet(Magazine mag, MagazineItemRuntimeData magData, Bullet bullet)
        {
            if (magData.MagazineStack.Count >= mag.capacity) return false;
            
            magData.MagazineStack.Push(bullet);

            return true;
        }

        // Returns null if empty.
        public static Bullet UnloadBullet(MagazineItemRuntimeData magazineItemData)
        {
            magazineItemData.MagazineStack.TryPop(out Bullet bullet);

            return bullet;
        }

        public static Bullet[] UnloadMagazine(MagazineItemRuntimeData magazineItemData)
        {
            Bullet[] bullets = magazineItemData.MagazineStack.ToArray();

            magazineItemData.MagazineStack.Clear();
            
            return bullets;
        }

        #endregion
        
        public override (InventoryItem, InventoryItem) ResolveItemCombine(InventoryItem target, InventoryItem placedItem)
        {
            // --- RETURN CLAUSES ---
            
            // Return if either item doesn't exist
            if (target == null || placedItem == null) return (target, placedItem);
            // Return if the target is not magazine
            if (target.Item is not Magazine magazineItem) return (target, placedItem);
            // Return if placed item is not bullet.
            if (placedItem.Item is not Bullet bulletItem) return (target, placedItem);
            // Return if Bullet is not an accepted Caliber.
            if(!Array.Exists(magazineItem.acceptedCartridges, element => element == bulletItem.bulletCaliber)) 
                return (target, placedItem);

            
            // --- LOGIC ---
            
            StackableItemRuntimeData bulletData = (StackableItemRuntimeData)placedItem.ItemRuntimeData;

            for (int i = bulletData.currentStackCount; i > 0; i--)
            {
                if (LoadBullet(magazineItem, (MagazineItemRuntimeData)target.ItemRuntimeData, bulletItem))
                {
                    bulletData.currentStackCount--;
                }
                else
                {
                    placedItem.ItemRuntimeData = bulletData;
                    return (target, placedItem);
                }
            }

            return (target, null);
        }
        
        public override ItemRuntimeData GetRuntimeData => new MagazineItemRuntimeData();

        #endregion
    }

    // Runtime Data for the Magazine Class.
    public class MagazineItemRuntimeData : ItemRuntimeData
    {
        public Stack<Bullet> MagazineStack = new Stack<Bullet>(); // Index corresponds to position in magazine. First in, Last out
    }

}