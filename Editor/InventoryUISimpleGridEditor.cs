using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hitbox.Stash.UI
{
    public class InventoryUISimpleGridEditor : Editor
    {
        #region Fields

        //private EDITORTYPE controller;
        private InventoryUISimpleGrid uiGrid;
        
        private GUISkin skin;

        
        // Inventory Grid Display Properties
        private readonly Dictionary<InventoryItem, Color> itemColor = new (); // Stores random colour for each inventory item
        private string hoveredItemTooltip; // Tooltip of currently hovered item
        private Vector2 scrollPos; // Current Scroll Position of inventory grid.

        #region Serialised

        private SerializedObject soTarget;
        private SerializedObject soGrid;
        
        private SerializedProperty style;

        #endregion

        #endregion

        #region --- EDITOR ---

        private void OnEnable()
        {
            Init();
            GetProperties();
        }
        
        public override void OnInspectorGUI()
        {
            if (soTarget == null)
                Init();
            
            soTarget.Update();
            GUILayout.Label("Inventory Grid", skin.GetStyle("title"));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Inventory Properties", skin.label);
            EditorGUILayout.PropertyField(style);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            
            // Inventory Grid Display
            GUILayout.Label( "Display", skin.GetStyle("header"));

            if (uiGrid != null)
            {
                BuildInventoryGridDisplay();
            }
            else
            {
                GUILayout.Label( "NO GRID ASSIGNED", skin.label);
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
            }
        }

        private void BuildInventoryGridDisplay()
        {
            Vector2Int size = uiGrid.Grid.Size;
            float viewWidth = EditorGUIUtility.currentViewWidth - 10;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Color defaultColour = GUI.color;

            for (int y = 0; y < size.y; y++)
            {
                GUILayout.BeginHorizontal(skin.GetStyle("grid"), GUILayout.MaxHeight(50), GUILayout.Height(Mathf.Clamp(viewWidth / (size.x + 1), 41, 50)));
                GUILayout.FlexibleSpace();
                for (int x = 0; x < size.x; x++)
                {
                    if (uiGrid.Grid.ItemAtPosition(new Vector2Int(x, y)))
                    {
                        
                        if (uiGrid.Grid.TryGetItemAtPosition(new Vector2Int(x, y), out InventoryItem storedItem))
                        {
                            Color color;
                            if (!itemColor.ContainsKey(storedItem))
                            {
                                color = new Color(Random.Range(.1f, .9f), Random.Range(.1f, .9f), Random.Range(.1f, .9f));
                                itemColor[storedItem] = color;
                            }
                            else
                                color = itemColor[storedItem];


                            GUI.color = color;
                        }

                        GUILayout.Button(new GUIContent("", storedItem.ItemProfile.name), GUILayout.ExpandHeight(true),
                            GUILayout.Width(Mathf.Clamp(viewWidth / (size.x + 1), 41, 51)));

                        GUI.color = defaultColour;
                    }
                    else
                    {
                        GUILayout.Button(new GUIContent("Empty"), GUILayout.ExpandHeight(true),
                            GUILayout.Width(Mathf.Clamp(viewWidth / (size.x + 1), 41, 51)));
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region Methods

        private void Init()
        {
            skin = Resources.Load<GUISkin>("Skins/KoalaGUISkin");
            // --- Get Target ---
            //controller = (InventoryGridComponent)target;
            soTarget = new SerializedObject(target);

            //grid = controller.Grid;
        }

        private void GetProperties()
        {
            // Inventory Grid Properties
            style = soTarget.FindProperty("style");
        }

        #endregion
    }

}