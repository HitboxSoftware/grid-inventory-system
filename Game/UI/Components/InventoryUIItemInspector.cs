using System.Collections;
using System.Collections.Generic;
using Hitbox.Stash;
using Hitbox.Stash.Items;
using Hitbox.Stash.UI;
using Hitbox.Stash.UI.Actions;
using Hitbox.UI.Windows;
using UnityEngine;

public class InventoryUIItemInspector : MonoBehaviour
{
    #region Fields
    
    [SerializeField] UIWindowManager windowManager;
    private readonly Dictionary<InventoryItem, UIWindow> _openInspectorWindows = new();

    public InventoryUIItemAction inspectAction;

    #endregion
    
    #region MonoBehaviour

    private void OnEnable()
    {
        if (inspectAction != null)
        {
            inspectAction.Interact += CreateInspectMenu;
        }
        
    }

    private void OnDisable()
    {
        if (inspectAction != null)
        {
            inspectAction.Interact -= CreateInspectMenu;
        }
    }

    #endregion

    #region Methods

    private void CreateInspectMenu(InventoryItem invItem)
    {
        if (_openInspectorWindows.ContainsKey(invItem))
        {
            _openInspectorWindows[invItem].Focus();
            return;
        }
        
        if (windowManager == null) // Making sure window manager is defined.
        {
            Debug.LogError($"{GetType().Name} ({gameObject.name}) attempted to open inspector with no window manager set!", this);
            return;
        }

        // Creating new window.
        //TODO: Set window position to "newContainerPos" once it's correctly set.
        UIWindow window = windowManager.CreateWindow(invItem.ItemProfile.name, Input.mousePosition, new Vector2(0.5f, 0.5f));
        
        _openInspectorWindows.Add(invItem, window);
    }

    #endregion
}
