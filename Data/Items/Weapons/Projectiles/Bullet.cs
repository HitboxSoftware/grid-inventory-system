using UnityEngine;

namespace KoalaDev.UGIS.Items.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Bullet", menuName = "KoalaDev/UGIS/Items/Weapons/Bullet")]
    public class Bullet : Stackable
    {
        #region --- VARIABLES ---
        
        [Header("Bullet Properties")]
        // Used to determine what guns can chamber the bullet, and general properties like muzzle velocity.
        public Caliber bulletCaliber;
        
        // Used to change edit the muzzle velocity received from the cartridge.
        public float muzzleVelocityOffset;
        public float muzzleVelocityMultiplier = 1f;

        #endregion
    }

}