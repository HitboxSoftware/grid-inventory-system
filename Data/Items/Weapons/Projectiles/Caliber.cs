using UnityEngine;

namespace KoalaDev.UGIS.Items.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Caliber", menuName = "KoalaDev/UGIS/Items/Weapons/Caliber")]
    public class Caliber : ScriptableObject
    {
        #region --- VARIABLES ---
        
        [Header("Caliber Properties")]
        // Kept separate from object name as full stops (".") could cause issues with file names
        public string caliberName;

        #endregion
    }
}