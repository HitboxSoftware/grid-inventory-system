using System.Collections;
using System.Collections.Generic;
using Hitbox.Inventory;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    #region --- VARIABLES ---

    private ItemDatabase _database;
    private SerializedProperty _itemsProperty;

    #endregion

    #region --- MONOBEHAVIOUR ---

    private void OnEnable()
    {
        _database = (ItemDatabase)target; 
        _itemsProperty = serializedObject.FindProperty("items");
    }

    #endregion

    #region --- METHODS ---
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if (GUILayout.Button(new GUIContent("Register All Items", "Automatically scan for items and populate database.")))
        {
            ScanForItems();
        }
        
        EditorGUILayout.Separator();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(_itemsProperty);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }

    private void ScanForItems()
    {
        string[] itemGUIDs = AssetDatabase.FindAssets("t:Item");
        List<Item> items = new List<Item>();

        foreach (string guid in itemGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            items.Add(item);
        }

        _itemsProperty.ClearArray();
        _itemsProperty.arraySize = ushort.MaxValue;
        
        foreach (Item item in items)
        {
            SerializedProperty arrayElement = _itemsProperty.GetArrayElementAtIndex(item.id);

            if (arrayElement.objectReferenceValue == null)
            {
                arrayElement.objectReferenceValue = item;
                item.parentDatabase = _database;
                continue;
            }
            
            if ((Item)arrayElement.objectReferenceValue != item)
            {
                int newId = _database.AddToItems(item);
                if (newId == -1)
                {
                    Debug.LogWarning($"Warning: {item.name} could not be added, no space in database!");
                    item.parentDatabase = null;
                    continue;
                }

                item.id = (ushort)newId;
                item.parentDatabase = _database;
                _itemsProperty.GetArrayElementAtIndex(newId).objectReferenceValue = item;
                Debug.LogWarning($"Warning: {item.name} ID was taken, new ID: {newId}");
            }
            
            
        }
    }

    #endregion
}
