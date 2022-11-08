using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KoalaDev.UGIS.Items.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "KoalaDev/UGIS/Items/Weapons/Weapon")]
    public class Weapon : Item
    {
        #region --- VARIABLES ---

        [Header("Weapon Properties")]
        public GameObject weaponPrefab;
        public Magazine[] supportedMagazines;
        public AttachmentSlot[] attachmentSlots = Array.Empty<AttachmentSlot>();
        
        #endregion

        #region --- MONOBEHAVIOUR ---



        #endregion

        #region --- METHODS ---



        #endregion
    }
}