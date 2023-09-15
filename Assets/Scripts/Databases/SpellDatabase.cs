using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// A spell database
/// </summary>
public class SpellDatabase
{
    public List<Spell> Spells { get; private set; }

    public SpellDatabase()
    {

    }

    /// <summary>
    /// Loads spells from JSON
    /// </summary>
    public void LoadSpellDatabase()
    {
        TextAsset data = Resources.Load<TextAsset>("Spells");

        if (data == null)
        {
            Debug.LogError("Failed to load spell data from Resources folder.");
            return;
        }

        Spells = JsonUtility.FromJson<SpellWrapper>(data.text).Spells;
    }

    /// <summary>
    /// Retrieves a Spell instance by its name.
    /// </summary>
    /// <param name="spellName">The name of the spell to retrieve.</param>
    /// <returns>Returns a Spell instance if found, otherwise null.</returns>
    public Spell GetSpellByName(string spellName)
    {
        return Spells.FirstOrDefault(spell => spell.Name.ToLower() == spellName.ToLower());
    }

    /// <summary>
    /// Wrapper for loading spells from JSON
    /// </summary>
    [System.Serializable]
    private class SpellWrapper
    {
        public List<Spell> Spells;
    }
}