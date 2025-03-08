using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hitbox.Stash.UI
{
    public abstract class InventoryUIAbstractContainer : MonoBehaviour
    {
        public abstract InventoryContainer LinkedContainer();
    }

}