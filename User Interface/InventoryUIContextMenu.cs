using System.Linq;
using KoalaDev.UGIS.Interactions;
using UnityEngine;

namespace KoalaDev.UGIS.UI
{
    public class InventoryUIContextMenu : MonoBehaviour
    {
        #region --- VARIABLES ---

        public InventoryUIItem invUIItem;

        #endregion

        #region --- MONOBEHAVIOUR ---

        private void Start()
        {
            Generate();
        }

        #endregion

        #region --- METHODS ---

        public InventoryUIContextMenu(InventoryUIItem invUIItem)
        {
            this.invUIItem = invUIItem;
        }

        private void Generate()
        {
            if (invUIItem.InvItem.Item.InteractionProfile.Interactions.Length <= 0) 
                Destroy(gameObject);
            
            InventoryInteraction[] interactions = (from interaction in invUIItem.InvItem.Item.InteractionProfile.Interactions
                where interaction != null
                where interaction.GetType() == typeof(InventoryInteraction)
                select (InventoryInteraction)interaction).ToArray();

            foreach (InventoryInteraction interaction in interactions)
            {
                InventoryUIContextButton interactionBtn = Instantiate(invUIItem.uiGrid.GetStyle.actionObj, transform).GetComponent<InventoryUIContextButton>();
                interactionBtn.action = interaction;
                interactionBtn.label.text = interaction.name;
                interactionBtn.parentMenu = this;
            }
        }

        public static void RemoveMenu()
        {
            InventoryUIManager.Instance.RemoveContextMenu();
            
        }

        #endregion
    }
}