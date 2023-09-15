#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ItemEditor : EditorWindow
{
    public List<Item> Items { get; set; }
    private Vector2 scrollPosition;
    private Dictionary<int, bool> itemFoldoutStates = new Dictionary<int, bool>();

    [MenuItem("Text Adventure/Item Editor")]
    public static void ShowWindow()
    {
        GetWindow<ItemEditor>("Item Editor");
    }

    private void OnEnable()
    {
        LoadItems();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Items", EditorStyles.boldLabel);
        if (GUILayout.Button("Save Items"))
        {
            SaveItems();
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < Items.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            if (!itemFoldoutStates.ContainsKey(i))
            {
                itemFoldoutStates.Add(i, false);
            }

            itemFoldoutStates[i] = EditorGUILayout.Foldout(itemFoldoutStates[i], Items[i].Name);

            if (itemFoldoutStates[i])
            {
                Items[i].Name = EditorGUILayout.TextField("Name", Items[i].Name);
                Items[i].Description = EditorGUILayout.TextField("Description", Items[i].Description);
                Items[i].Slot = EditorGUILayout.TextField("Slot", Items[i].Slot);
                Items[i].Power = EditorGUILayout.IntField("Power", Items[i].Power);
                Items[i].Value = EditorGUILayout.FloatField("Value", Items[i].Value);
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Item"))
        {
            Items.Add(new Item("", ""));
        }

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Loads items from JSON
    /// </summary>
    private void LoadItems()
    {
        TextAsset data = Resources.Load<TextAsset>("Items");

        if (data == null)
        {
            Debug.LogError("Failed to load item data from Resources folder.");
            return;
        }

        Items = JsonUtility.FromJson<ItemWrapper>(data.text).Items;
    }

    /// <summary>
    /// Saves items to JSON
    /// </summary>
    private void SaveItems()
    {
        var wrapper = new ItemWrapper { Items = Items };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Path.Combine(FileManager.GetDocumentsPath(), "Items.json"), json);
        AssetDatabase.Refresh();

        Debug.Log("Items saved.");
    }

    [System.Serializable]
    private class ItemWrapper
    {
        public List<Item> Items;
    }
}

#endif