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
        
        private void Generate()
        {
            if (invUIItem.InvItem.Item.interactionProfile.Interactions.Length <= 0) 
                Destroy(gameObject);
            
            InventoryInteractionChannel[] interactions = (from interaction in invUIItem.InvItem.Item.interactionProfile.Interactions
                where interaction != null
                where interaction.GetType() == typeof(InventoryInteractionChannel)
                select (InventoryInteractionChannel)interaction).ToArray();

            foreach (InventoryInteractionChannel interaction in interactions)
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