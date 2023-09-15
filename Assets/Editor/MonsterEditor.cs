#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MonsterEditor : EditorWindow
{
    private string jsonPath;
    public List<Monster> Monsters { get; set; }
    private Vector2 scrollPosition;
    private Dictionary<int, bool> monsterFoldoutStates = new Dictionary<int, bool>();

    [MenuItem("Text Adventure/Monster Editor")]
    public static void ShowWindow()
    {
        GetWindow<MonsterEditor>("Monster Editor");
    }

    private void OnEnable()
    {
        jsonPath = Path.Combine(Application.dataPath, "Resources/Monsters.JSON");
        LoadMonsters();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Monsters", EditorStyles.boldLabel);
        if (GUILayout.Button("Save Monsters"))
        {
            SaveMonsters();
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < Monsters.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            // Store the foldout state for each monster in a dictionary using their index as key
            if (!monsterFoldoutStates.ContainsKey(i))
            {
                monsterFoldoutStates.Add(i, false);
            }

            // Create the foldout for each monster
            monsterFoldoutStates[i] = EditorGUILayout.Foldout(monsterFoldoutStates[i], Monsters[i].Name);

            if (monsterFoldoutStates[i])
            {
                Monsters[i].Name = EditorGUILayout.TextField("Name", Monsters[i].Name);
                Monsters[i].Description = EditorGUILayout.TextField("Description", Monsters[i].Description);
                Monsters[i].Aggressive = EditorGUILayout.Toggle("Aggressive", Monsters[i].Aggressive);
                Monsters[i].ShopKeeper = EditorGUILayout.Toggle("ShopKeeper", Monsters[i].ShopKeeper);
                Monsters[i].Wanders = EditorGUILayout.Toggle("Wanders", Monsters[i].Wanders);
                Monsters[i].Health = EditorGUILayout.FloatField("Health", Monsters[i].Health);
                Monsters[i].MaxHealth = EditorGUILayout.FloatField("MaxHealth", Monsters[i].MaxHealth);
                Monsters[i].Mana = EditorGUILayout.FloatField("Mana", Monsters[i].Mana);
                Monsters[i].MaxMana = EditorGUILayout.FloatField("MaxMana", Monsters[i].MaxMana);

                EditMonsterInventory(Monsters[i]);
                EditMonsterEquippedItems(Monsters[i]);
                EditMonsterSpells(Monsters[i]);
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Monster"))
        {
            Monsters.Add(new Monster("", ""));
        }

        EditorGUILayout.EndScrollView();
    }

    private void EditMonsterInventory(Monster monster)
    {
        EditorGUILayout.LabelField("Item Names");
        for (int j = 0; j < monster.ItemNames.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();
            monster.ItemNames[j] = EditorGUILayout.TextField(monster.ItemNames[j]);
            if (GUILayout.Button("Remove"))
            {
                monster.ItemNames.RemoveAt(j);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Item"))
        {
            monster.ItemNames.Add("New Item");
        }
    }


    private void EditMonsterEquippedItems(Monster monster)
    {
        EditorGUILayout.LabelField("Equipped Items", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        for (int i = 0; i < monster.EquippedItemNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            monster.EquippedItemNames[i] = EditorGUILayout.TextField("Item Name:", monster.EquippedItemNames[i]);

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                monster.EquippedItemNames.RemoveAt(i);
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Equipped Item"))
        {
            monster.EquippedItemNames.Add("New Item");
        }

        EditorGUI.indentLevel--;
    }

    private void EditMonsterSpells(Monster monster)
    {
        EditorGUILayout.LabelField("Spells", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        for (int i = 0; i < monster.SpellNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            monster.SpellNames[i] = EditorGUILayout.TextField("Spell Name:", monster.SpellNames[i]);

            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                monster.SpellNames.RemoveAt(i);
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Spell"))
        {
            monster.SpellNames.Add("New Spell");
        }

        EditorGUI.indentLevel--;
    }

    #region FILE MANAGEMENT

    private void LoadMonsters()
    {
        TextAsset mobData = Resources.Load<TextAsset>("Monsters");

        if (mobData == null)
        {
            Debug.LogError("Failed to load mob data from Resources folder.");
            return;
        }

        Monsters = JsonUtility.FromJson<MonsterListWrapper>(mobData.text).Monsters;
    }

    private void SaveMonsters()
    {
        var wrapper = new MonsterListWrapper { Monsters = Monsters };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Path.Combine(FileManager.GetDocumentsPath(), "Monsters.json"), json);
        AssetDatabase.Refresh();

        Debug.Log("Monsters saved");
    }

    #endregion
}

[System.Serializable]
public class MonsterListWrapper
{
    public List<Monster> Monsters;
}

#endif