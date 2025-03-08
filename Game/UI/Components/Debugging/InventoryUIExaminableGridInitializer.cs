
namespace Hitbox.Stash.UI.Debugging
{
    public class InventoryUIExaminableGridInitializer : InventoryUIGridInitializer
    {
        public override InventoryGrid BuildGrid()
        {
            return new InventoryExaminableGrid(gridSize);
        }

        public override void Init()
        {
            base.Init();

            StartCoroutine(((InventoryExaminableGrid)uiGrid.Grid).ExamineAllItems());
        }
    }
}