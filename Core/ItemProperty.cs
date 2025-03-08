using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Stash
{
    public class ItemProperty
    {
        #region Fields

        public Sprite icon;
        public string tooltip;
        public string value;

        #endregion

        #region Methods

        public ItemProperty(Sprite icon, string tooltip, string value)
        {
            this.icon = icon;
            this.tooltip = tooltip;
            this.value = value;
        }

        #endregion
    }

}