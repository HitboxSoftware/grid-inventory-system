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

        #endregion
    }

    // Runtime Data for the Magazine Class.
    public class MagazineItemData : AdditionalItemData
    {
        public Stack<Bullet> MagazineStack; // Index corresponds to position in magazine. First in, Last out
    }

}