using UnityEngine;

namespace KoalaDev.UGIS
{
    [CreateAssetMenu(fileName = "New Item", menuName = "KoalaDev/UGIS/Items/Generic")]
    public class GenericItem : ScriptableObject
    {
        #region --- VARIABLES ---

        public Vector2Int size = Vector2Int.one;
        public Sprite icon;
        public GameObject worldObject;

        #endregion

        #region --- METHODS ---



        #endregion
    }

}