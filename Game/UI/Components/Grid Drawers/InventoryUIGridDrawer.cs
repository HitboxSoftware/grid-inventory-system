using System.Collections;
using System.Collections.Generic;
using Hitbox.Collections;
using Hitbox.Collections.Data;
using Hitbox.Stash.UI;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIGridDrawer : MonoBehaviour
{
    #region Fields

    [SerializeField] protected InventoryUIAbstractGrid uiGrid;
    
    // Runtime
    private List<GameObject> _currentPanels = new List<GameObject>();
    private Queue<GameObject> _panelPool = new Queue<GameObject>();

    #endregion
    
    #region MonoBehaviour

    private void Awake()
    {
        Init();
    }
    
    #endregion

    #region Methods

    public GameObject DrawRect(GridRect rect, Sprite sprite, Color color)
    {
        return DrawRect(rect.position, rect.size, sprite, color);
    }
    
    public GameObject DrawRect(Vector2Int pos, Vector2Int size, Sprite sprite, Color color)
    {
        GridRect rect = GetClampedRect(pos, size);
        
        // Getting panel from pool or creating new one.
        GameObject panel;
        Image panelImg;
        if (_panelPool.Count == 0)
        {
            panel = new GameObject();
            
            panel.transform.SetParent(transform, false);
            
            panelImg = panel.AddComponent<Image>();
            panelImg.type = Image.Type.Tiled;
        }
        else
        {
            panel = _panelPool.Dequeue();
            panel.SetActive(true);
            panelImg = panel.GetComponent<Image>();
        }

        panelImg.color = color;
        panelImg.sprite = sprite;
        
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        
        rectTransform.sizeDelta = rect.size * uiGrid.Style.cellSize;
        
        rectTransform.anchoredPosition = uiGrid.GridToLocalPoint(rect.position);

        _currentPanels.Add(panel);

        return panel;
    }

    public void Clear()
    {
        if (_currentPanels == null) return;
        
        foreach (GameObject panel in _currentPanels)
        {
            panel.SetActive(false);
            _panelPool.Enqueue(panel);
        }
        
        _currentPanels.Clear();
    }

    public void RemoveRect(GameObject panel)
    {
        panel.SetActive(false);
        _panelPool.Enqueue(panel);
        _currentPanels.Remove(panel);
    }

    public void SetGrid(InventoryUIAbstractGrid newGrid)
    {
        uiGrid = newGrid;

        Init();
    }

    private GridRect GetClampedRect(Vector2Int pos, Vector2Int size)
    {
        return GetClampedRect(new GridRect(pos, size));
    }
    
    private GridRect GetClampedRect(GridRect rect)
    {
        // Clamping x to zero and reducing size of rect using overflow.
        if (rect.xMin < 0)
        {
            rect.xMax += rect.xMin;
            rect.xMin = 0;
        }
        
        // Clamping y to zero and reducing size of rect using overflow.
        if (rect.yMin < 0)
        {
            rect.yMax += rect.yMin;
            rect.yMin = 0;
        }

        // Clamping x to max grid x and reducing size of rect using overfow.
        if (rect.xMax > uiGrid.Grid.Size.x)
        {
            int difference = rect.xMax - uiGrid.Grid.Size.x;

            rect.xMax -= difference;
        }
        
        // Clamping y to max grid y and reducing size of rect using overfow.
        if (rect.yMax > uiGrid.Grid.Size.y)
        {
            int difference = rect.yMax - uiGrid.Grid.Size.y;

            rect.yMax -= difference;
        }

        return rect;
    }

    private void Init()
    {
        if (uiGrid == null) return;
        
        _currentPanels = new List<GameObject>();

        RectTransform rectTransform = GetComponent<RectTransform>();

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
    }

    #endregion
}
