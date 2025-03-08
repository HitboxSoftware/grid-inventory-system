using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hitbox.Stash;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    #region Fields

    private ItemDatabase _database;
    private SerializedProperty _itemsProperty;
    
    #endregion

    #region MonoBehaviour

    private void OnEnable()
    {
        _database = (ItemDatabase)target; 
        _itemsProperty = serializedObject.FindProperty("items");
    }

    #endregion

    #region Methods
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        if (GUILayout.Button(new GUIContent("Register All Items", "Automatically scan project for items and populate database.")))
        {
            if (EditorUtility.DisplayDialog("Scan Confirmation",
        "This operation will scan the entire project for items, if you have the example content in the project it will add this as well. It's recommended to select a specific path instead!", 
             "Ok", "Cancel"))
            {
                ScanForItems();
            }
        }

        if (GUILayout.Button(new GUIContent("Register from Path", "Automatically scan path for items and populate database.")))
        {
            string assetPath = AssetDatabase.GetAssetPath(target.GetInstanceID());
            string absolutePath = EditorUtility.OpenFolderPanel("Set Path", Path.GetDirectoryName(assetPath), "");
            
            string searchPath = "";
            if (absolutePath != null && absolutePath.StartsWith(Application.dataPath)) {
                searchPath = "Assets" + absolutePath[Application.dataPath.Length..];
            }

            if (searchPath.Length > 0)
            {
                ScanForItems(searchPath);
            }
        }
        
        EditorGUILayout.Separator();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(_itemsProperty);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }

    private void ScanForItems(string searchPath = "")
    {
        string[] itemGUIDs = searchPath.Length > 0
            ? AssetDatabase.FindAssets("t:Item", new[] { searchPath })
            : AssetDatabase.FindAssets("t:Item");
         
        List<ItemProfile> items = new List<ItemProfile>();

        foreach (string guid in itemGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            ItemProfile itemProfile = AssetDatabase.LoadAssetAtPath<ItemProfile>(path);
            items.Add(itemProfile);
        }

        _itemsProperty.ClearArray();
        _itemsProperty.arraySize = ushort.MaxValue;
        
        foreach (ItemProfile item in items)
        {
            SerializedProperty arrayElement = _itemsProperty.GetArrayElementAtIndex(item.id);

            if (arrayElement.objectReferenceValue == null)
            {
                arrayElement.objectReferenceValue = item;
                item.parentDatabase = _database;
                
                // Marking the updated item as dirty, this ensures the update is saved.
                EditorUtility.SetDirty(item);
                
                continue;
            }
            
            if ((ItemProfile)arrayElement.objectReferenceValue != item)
            {
                int newId = _database.AddToItems(item);
                if (newId == -1)
                {
                    Debug.LogWarning($"Warning: {item.name} could not be added, no space in database!");
                    item.parentDatabase = null;
                    
                    // Marking the updated item as dirty, this ensures the update is saved.
                    EditorUtility.SetDirty(item);
                    
                    continue;
                }

                item.id = (ushort)newId;
                item.parentDatabase = _database;
                
                // Marking the updated item as dirty, this ensures the update is saved.
                EditorUtility.SetDirty(item);
                
                _itemsProperty.GetArrayElementAtIndex(newId).objectReferenceValue = item;
                Debug.LogWarning($"Warning: {item.name} ID was taken, new ID: {newId}");
            }
            
            
        }
    }

    #endregion
}
