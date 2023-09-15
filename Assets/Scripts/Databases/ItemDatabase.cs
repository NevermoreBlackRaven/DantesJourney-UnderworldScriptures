using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// An item database
/// </summary>
public class ItemDatabase
{
    public List<Item> Items { get; set; }

    /// <summary>
    /// Retrieves an Item instance by its exact name.
    /// </summary>
    /// <param name="itemName">The exact name of the item to retrieve.</param>
    /// <returns>Returns an Item instance if found, otherwise null.</returns>
    public Item GetItemByName(string itemName)
    {
        return Items.Find(item => item.Name == itemName);
    }

    /// <summary>
    /// Retrieves an Item instance by a partial name.
    /// </summary>
    /// <param name="itemName">A partial name of the item to retrieve.</param>
    /// <returns>Returns an Item instance if found, otherwise null.</returns>
    public Item GetItemByPartialName(string itemName)
    {
        return Items.Find(item => item.Name.Contains(itemName));
    }


    /// <summary>
    /// Loads the item database from JSON
    /// </summary>
    public void LoadItemDatabase()
    {
        TextAsset gameWorldData = Resources.Load<TextAsset>("Items");

        if (gameWorldData == null)
        {
            Debug.LogError("Failed to load item data from Resources folder.");
            return;
        }

        Items = JsonUtility.FromJson<ItemListWrapper>(gameWorldData.text).Items;
    }

    /// <summary>
    /// A wrapper class for loading items from JSON
    /// </summary>
    [System.Serializable]
    private class ItemListWrapper
    {
        public List<Item> Items;
    }
}