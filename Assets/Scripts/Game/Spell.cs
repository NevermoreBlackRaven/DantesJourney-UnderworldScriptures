using System;

/// <summary>
/// The type of spell
/// </summary>
[Serializable]
public enum SpellType
{
    Heal,
    Nuke
}

/// <summary>
/// A spell's target
/// </summary>
[Serializable]
public enum TargetType
{
    Self,
    Other,
    AOE
}

/// <summary>
/// Represents a spell
/// </summary>
[Serializable]
public class Spell
{
    public string Name;
    public string Description;
    public SpellType SpellType;
    public TargetType Target;
    public float Power;
    public float ManaCost;

    public Spell()
    {

    }
}