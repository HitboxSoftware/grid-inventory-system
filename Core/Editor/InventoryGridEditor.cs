using System.Collections.Generic;
using KoalaDev.UGIS;
using UnityEditor;
using UnityEngine;

namespace InventorySystem.UI
{
    [CustomEditor(typeof(InventoryGrid))]

    public class InventoryGridEditor : Editor
    {
        #region --- VARIABLES ---

        private InventoryGrid controller;
        
        private GUISkin skin;

        
        // Inventory Grid Display Properties
        private readonly Dictionary<InventoryItem, Color> itemColor = new (); // Stores random colour for each inventory item
        private string hoveredItemTooltip; // Tooltip of currently hovered item
        private Vector2 scrollPos; // Current Scroll Position of inventory grid.

        #region - SERIALIZED -

        private SerializedObject soTarget;
        
        private SerializedProperty gridSize;

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
            EditorGUILayout.PropertyField(gridSize);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Inventory Grid Display
            GUILayout.Label( "Display", skin.GetStyle("header"));
            
            Vector2Int size = controller.size;
            float viewWidth = EditorGUIUtility.currentViewWidth - 10;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Color defaultColour = GUI.color;

            for (int y = 0; y < size.y; y++)
            {
                GUILayout.BeginHorizontal(skin.GetStyle("grid"), GUILayout.MaxHeight(50), GUILayout.Height(Mathf.Clamp(viewWidth / (size.x + 1), 41, 50)));
                GUILayout.FlexibleSpace();
                for (int x = 0; x < size.x; x++)
                {
                    if (controller.ItemAtSlot(new Vector2Int(x, y)))
                    {
                        InventoryItem storedItem = controller.GetItemAtSlot(new Vector2Int(x, y));
                        if (storedItem != null)
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

                        GUILayout.Button(new GUIContent("", storedItem.Item.name), GUILayout.ExpandHeight(true),
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

            if (EditorGUI.EndChangeCheck())
            {
                soTarget.ApplyModifiedProperties();
            }
        }

        #endregion

        #region --- METHODS ---

        private void Init()
        {
            skin = Resources.Load<GUISkin>("Skins/KoalaGUISkin");
            // --- Get Target ---
            controller = (InventoryGrid)target;
            soTarget = new SerializedObject(target);
        }

        private void GetProperties()
        {
            // Inventory Grid Properties
            gridSize = soTarget.FindProperty("size");
        }

        #endregion
    }

}