using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hitbox.Stash.UI
{
    public class InventoryUIInspectMenu : MonoBehaviour
    {
        #region Fields

        public InventoryItem invItem;

        #endregion

        #region MonoBehaviour

        protected virtual void Start()
        {
            Init();
        }

        #endregion

        #region Methods

        protected virtual void Init()
        {
            if (invItem == null)
            {
                Debug.LogWarning($"{gameObject.name} ({name}) has no inventory item assigned on initialisation!");
            }
        }

        #endregion
    }

}