using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an item
/// </summary>
[System.Serializable]
public class Item
{
    public string Name;
    public string Description;
    public string Slot;
    public int Power;
    public float Value;

    public Item(string name, string description, string slot = null, int power = 0, float value = 0)
    {
        Name = name;
        Description = description;
        Slot = slot;
        Power = power;
        Value = value;
    }
}