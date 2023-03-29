using System.Linq;
using Hitbox.UGIS.Interactions;
using Hitbox.UI;
using UnityEngine;

namespace Hitbox.UGIS.UI.ContextMenu
{
    public class UIInventoryContextMenu : MonoBehaviour
    {
        #region --- VARIABLES ---

        public UIInventoryItem invUIItem;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            Generate();
        }

        #endregion

        #region --- METHODS ---
        
        private void Generate()
        {
            if (invUIItem.InvItem.Item.interactionProfile.interactions.Length <= 0) 
                Destroy(gameObject);
            
            InventoryInteractionChannel[] interactions = (from interaction in invUIItem.InvItem.Item.interactionProfile.interactions
                where interaction != null
                where interaction.GetType() == typeof(InventoryInteractionChannel)
                select (InventoryInteractionChannel)interaction).ToArray();

            foreach (InventoryInteractionChannel interaction in interactions)
            {
                UIInventoryContextButton interactionBtn = Instantiate(invUIItem.UIGrid.Style.actionObj, transform).GetComponent<UIInventoryContextButton>();
                interactionBtn.action = interaction;
                interactionBtn.label.text = interaction.name;
                interactionBtn.parentMenu = this;
            }
        }

        public static void RemoveMenu()
        {
            UIInventoryManager.Instance.RemoveContextMenu();
        }

        #endregion
    }
}