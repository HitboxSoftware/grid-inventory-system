using UnityEngine;

namespace Hitbox.UGIS.Items.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Caliber", menuName = "Hitbox/UGIS/Items/Weapons/Caliber")]
    public class Caliber : ScriptableObject
    {
        #region --- VARIABLES ---
        
        [Header("Caliber Properties")]
        // Kept separate from object name as full stops (".") could cause issues with file names
        public string caliberName;

        #endregion
    }
}