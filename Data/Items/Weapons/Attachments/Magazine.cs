using System;
using System.Collections.Generic;
using UnityEngine;

namespace KoalaDev.UGIS.Items.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Magazine", menuName = "KoalaDev/UGIS/Items/Weapons/Attachments/Magazine")]
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
        public static void FillMagazine(Magazine mag, MagazineItemData magData, Bullet bullet, int count = 0)
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
        public static bool LoadBullet(Magazine mag, MagazineItemData magData, Bullet bullet)
        {
            if (magData.MagazineStack.Count >= mag.capacity) return false;
            
            magData.MagazineStack.Push(bullet);

            return true;
        }

        // Returns null if empty.
        public static Bullet UnloadBullet(MagazineItemData magazineData)
        {
            magazineData.MagazineStack.TryPop(out Bullet bullet);

            return bullet;
        }

        public static Bullet[] UnloadMagazine(MagazineItemData magazineData)
        {
            Bullet[] bullets = magazineData.MagazineStack.ToArray();

            magazineData.MagazineStack.Clear();
            
            return bullets;
        }

        #endregion
        
        public override (InventoryItem, InventoryItem) ItemToItem(InventoryItem invItem1, InventoryItem invItem2)
        {
            // --- RETURN CLAUSES ---
            
            // Return if either item doesn't exist
            if (invItem1 == null || invItem2 == null) return (invItem1, invItem2);
            // Return if the target is not magazine
            if (invItem1.Item is not Magazine magazineItem) return (invItem1, invItem2);
            // Return if second item is not bullet.
            if (invItem2.Item is not Bullet bulletItem) return (invItem1, invItem2);
            // Return if Bullet is not an accepted Caliber.
            if(!Array.Exists(magazineItem.acceptedCartridges, element => element == bulletItem.bulletCaliber)) 
                return (invItem1, invItem2);

            
            // --- LOGIC ---
            
            StackableItemData bulletData = (StackableItemData)invItem2.ItemData;

            for (int i = bulletData.currentStackCount; i > 0; i--)
            {
                if (LoadBullet(magazineItem, (MagazineItemData)invItem1.ItemData, bulletItem))
                {
                    bulletData.currentStackCount--;
                }
                else
                {
                    invItem2.ItemData = bulletData;
                    return (invItem1, invItem2);
                }
            }

            return (invItem1, null);
        }
        
        public override AdditionalItemData GetAdditionalData => new MagazineItemData();

        #endregion
    }

    // Runtime Data for the Magazine Class.
    public class MagazineItemData : AdditionalItemData
    {
        public Stack<Bullet> MagazineStack = new Stack<Bullet>(); // Index corresponds to position in magazine. First in, Last out
    }

}