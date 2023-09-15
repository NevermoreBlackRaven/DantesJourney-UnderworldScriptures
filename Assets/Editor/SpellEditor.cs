#if UNITY_EDITOR


using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SpellEditor : EditorWindow
{
    private List<Spell> spells;
    private Dictionary<int, bool> foldoutStates = new Dictionary<int, bool>();

    [MenuItem("Text Adventure/Spell Editor")]
    public static void ShowWindow()
    {
        GetWindow<SpellEditor>("Spell Editor");
    }

    private void OnEnable()
    {
        LoadSpells();
    }

    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Spells", EditorStyles.boldLabel);
        if (GUILayout.Button("Save Spells"))
        {
            SaveSpells();
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < spells.Count; i++)
        {
            var spell = spells[i];

            if (!foldoutStates.ContainsKey(i))
            {
                foldoutStates.Add(i, false);
            }

            foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], spell.Name);
            if (foldoutStates[i])
            {
                EditorGUI.indentLevel++;
                spell.Name = EditorGUILayout.TextField("Name", spell.Name);
                spell.Description = EditorGUILayout.TextField("Description", spell.Description);
                spell.SpellType = (SpellType)EditorGUILayout.EnumPopup("Spell Type", spell.SpellType);
                spell.Target = (TargetType)EditorGUILayout.EnumPopup("Target", spell.Target);
                spell.Power = EditorGUILayout.FloatField("Power", spell.Power);
                spell.ManaCost = EditorGUILayout.FloatField("Mana Cost", spell.ManaCost);
                EditorGUI.indentLevel--;

                if (GUILayout.Button("Remove Spell"))
                {
                    spells.RemoveAt(i);
                }
            }
        }

        if (GUILayout.Button("Add Spell"))
        {
            spells.Add(new Spell());
        }
    }

    private void LoadSpells()
    {
        TextAsset data = Resources.Load<TextAsset>("Spells");

        if (data == null)
        {
            Debug.LogError("Failed to load item data from Resources folder.");
            return;
        }

        spells = JsonUtility.FromJson<SpellWrapper>(data.text).Spells;
    }

    private void SaveSpells()
    {
        var wrapper = new SpellWrapper { Spells = spells };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Path.Combine(FileManager.GetDocumentsPath(), "Spells.json"), json);
        AssetDatabase.Refresh();

        Debug.Log("Spells saved");
    }

    [System.Serializable]
    private class SpellWrapper
    {
        public List<Spell> Spells;
    }
}

#endif