using System;
using UnityEngine;

namespace Hitbox.UGIS.Items.WeaponSystem
{
    public class Attachment : Item
    {
        #region --- VARIABLES ---

        [Header("Attachment Properties")]
        public GameObject attachmentPrefab;
        public AttachmentType attachmentType;

        #endregion
    }

    public enum AttachmentType { Scope, Magazine }
    
    // Used for attachment slots on weapons.
    [Serializable]
    public class AttachmentSlot
    {
        public string name;
        public Vector3 slotPosition;
        public Vector3 slotRotation;
        public AttachmentType[] attachmentTypes;
    }
}