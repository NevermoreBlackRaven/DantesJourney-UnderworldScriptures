using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// A monster database
/// </summary>
public class MonsterDatabase
{
    public List<Monster> Monsters { get; set; }

    /// <summary>
    /// Loads monsters from JSON
    /// </summary>
    public void LoadMonsters()
    {
        TextAsset mobData = Resources.Load<TextAsset>("Monsters");

        if (mobData == null)
        {
            Debug.LogError("Failed to load mob data from Resources folder.");
            return;
        }

        Monsters = JsonUtility.FromJson<MonsterListWrapper>(mobData.text).Monsters;
    }

    /// <summary>
    /// Retrieves a Monster instance by its name.
    /// </summary>
    /// <param name="name">The name of the monster to retrieve.</param>
    /// <returns>Returns a Monster instance if found, otherwise null.</returns>
    public Monster GetMonsterByName(string name)
    {
        return Monsters.FirstOrDefault(monster => monster.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Wrapper class for loading monsters from JSON
    /// </summary>
    [System.Serializable]
    private class MonsterListWrapper
    {
        public List<Monster> Monsters;
    }
}
