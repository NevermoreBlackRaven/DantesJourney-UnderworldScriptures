using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an interactable object
/// </summary>
[System.Serializable]
public class InteractableObject
{
    public string Name;
    public string Description;
    public string InteractionItemName;

    public InteractableObject(string name, string description)
    {
        Name = name;
        Description = description;
    }
}
