using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hitbox.Stash;
using Hitbox.Stash.Items;
using Hitbox.Stash.UI;
using Hitbox.Stash.UI.Actions;
using Hitbox.UI.Windows;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIContainerManager : MonoBehaviour
{
    #region Fields

    private InventoryUIManager _inventoryManager;
    
    [SerializeField] UIWindowManager windowManager;
    private readonly Dictionary<InventoryContainerItem, UIWindow> _openContainerWindows = new();

    public InventoryUIItemAction containerAction;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        if (!TryGetComponent(out _inventoryManager))
        {
            Debug.LogError($"{GetType().Name} ({gameObject.name}) requires an InventoryUIManager!", this);
        }
    }

    private void OnEnable()
    {
        _inventoryManager.OnDragStart += OnDrag;
        
        if (containerAction != null)
        {
            containerAction.Interact += ContainerOpened;
        }
        
    }

    private void OnDisable()
    {
        _inventoryManager.OnDragStart -= OnDrag;
        
        if (containerAction != null)
        {
            containerAction.Interact -= ContainerOpened;
        }
    }

    #endregion
    
    #region Methods

    public void OnDrag(InventoryItemDragData dragData)
    {
        InventoryItem draggedItem = dragData.InvItem;
        
        // Checking if dragged item was linked to open container window.
        if (draggedItem is not InventoryContainerItem containerInventoryItem) return;
        
        
        // Closing the container item's linked UI window, if open.
        if (_openContainerWindows.TryGetValue(containerInventoryItem, out UIWindow container))
        {
            container.Close();
        }
                        
        // Closing Container Windows that are children of current window being closed, *this is recursive*.
        List<InventoryContainerItem> childContainers = _openContainerWindows.Keys.Where(item =>
            ((InventoryContainerItem)draggedItem).GridGroup.ContainsItem(item)).ToList();
            
        for (int i = childContainers.Count - 1; i >= 0; i--)
        {
            if(_openContainerWindows[childContainers[i]] != null)
                _openContainerWindows[childContainers[i]].Close();
                
            _openContainerWindows.Remove(childContainers[i]);
        }
                    
        if (_openContainerWindows.TryGetValue(containerInventoryItem, out UIWindow openContainer))
        {
            openContainer.Close();
        }
    }

    private void ContainerOpened(InventoryItem invItem)
    {        
        if (windowManager == null) // Making sure window manager is defined.
        {
            Debug.LogError($"{GetType().Name} ({gameObject.name}) attempted to open container with no window manager set!", this);
            return;
        }
        
        if (invItem is not InventoryContainerItem containerItem) return; // invItem must be a container
        if (containerItem.ItemProfile is not ContainerItemProfile container) return; // Getting container reference.
        if (_openContainerWindows.ContainsKey(containerItem)) return; // Checking if container is already open.

        
        // Creating new UI window.
        UIWindow window = windowManager.CreateWindow(container.name, Mouse.current.position.ReadValue(), new Vector2(0.5f, 0.97f));

        // Setting up Container Grid object
        GameObject gridObj = new("ContainerGrid");
        
        if (!gridObj.TryGetComponent(out InventoryUIGridGroup containerUIGrid))
        {
            containerUIGrid = gridObj.AddComponent<InventoryUIGridGroup>();

            containerUIGrid.style = _inventoryManager.inventoryStyle;
        }
        
        window.AddContent(gridObj.transform);
        
        window.SetTitle(containerItem.ItemProfile.name);
        
        //TODO: USE A CACHED VERSION OF THE ICON!!!!
        IconGenerator.Instance.SetAngle(containerItem.ItemProfile.modelIconAngle);
        window.SetIcon(IconGenerator.Instance.GenerateSpriteFromPrefab(containerItem.ItemProfile.worldObject, true));
        
        // Setting Up Backend
        containerItem.GridGroup ??= new InventoryGridGroup(container.gridSizes, new InventoryItem[] { containerItem });

        containerUIGrid.SetGrids(containerItem.GridGroup, container.rowCapacities, true);

        _openContainerWindows.Add(containerItem, window);
        window.Closed += _ => containerUIGrid.ClearGridUI();
        window.Closed += ContainerRemoved;
        
        containerItem.Updated += HandleUpdated;
    }

    private void HandleUpdated()
    {
        List<InventoryContainerItem> containers = _openContainerWindows.Keys.Where(container => container.ParentContainer == null).ToList();
        foreach (var container in containers)
        {
            if (!_openContainerWindows.ContainsKey(container)) continue;
            _openContainerWindows[container].Close();
        }
    }

    private void ContainerRemoved(UIWindow containerWindow)
    {
        // Getting Container Runtime
        InventoryContainerItem containerInvItem = _openContainerWindows.Where(
            container => container.Value == containerWindow).Select(
            container => container.Key).FirstOrDefault();

        if (containerInvItem == null) return;

        containerInvItem.Updated -= HandleUpdated;

        //TODO: Return dragged item to container if dragged item's original grid is being closed.

        // Closing Container Windows that are children of current window being closed, *this is recursive*.
        // TODO: Had stack overflow from this, if all is well this won't happen, but implement checks just in case.
        List<InventoryContainerItem> childContainers =
            _openContainerWindows.Keys.Where(item => containerInvItem.GridGroup.ContainsItem(item)).ToList();

        for (int i = childContainers.Count - 1; i >= 0; i--)
        {
            if (_openContainerWindows[childContainers[i]] != null)
                _openContainerWindows[childContainers[i]].Close();

            _openContainerWindows.Remove(childContainers[i]);
        }

        _openContainerWindows.Remove(containerInvItem);
    }

    #endregion
}
