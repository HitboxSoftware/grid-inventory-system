using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Inventory
{
    public class ItemProperty
    {
        #region --- VARIABLES ---

        public Sprite icon;
        public string tooltip;
        public string value;

        #endregion

        #region --- METHODS ---

        public ItemProperty(Sprite icon, string tooltip, string value)
        {
            this.icon = icon;
            this.tooltip = tooltip;
            this.value = value;
        }

        #endregion
    }

}