using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a monster
/// </summary>
[System.Serializable]
public class Monster
{
    public string Id { get; set; }
    public string Name;
    public string Description;
    public bool Aggressive;
    public bool ShopKeeper;
    public List<string> ItemNames;
    public List<string> EquippedItemNames;
    public bool Wanders = false;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float Mana = 100;
    public float MaxMana = 100;
    public bool Dead = false;
    public bool InCombat = false;
    public List<string> SpellNames;

    public Monster(string name, string description, bool aggressive = false, bool shopKeeper = false, bool wanders = false)
    {
        Id = System.Guid.NewGuid().ToString();
        Name = name;
        Description = description;
        Aggressive = aggressive;
        ShopKeeper = shopKeeper;
        ItemNames = new List<string>();
        EquippedItemNames = new List<string>();
        SpellNames = new List<string>();
        Wanders = wanders;
        Health = 100f;
        MaxHealth = 100f;
        Health = 100f;
        MaxHealth = 100f;
    }

    /// <summary>
    /// Retrieves the equipped item in the specified slot from the ItemDatabase.
    /// </summary>
    /// <param name="itemDatabase">The ItemDatabase instance to search in.</param>
    /// <param name="slot">The slot to search for an equipped item.</param>
    /// <returns>Returns an Item instance if found, otherwise null.</returns>
    public Item GetEquippedItemBySlot(ItemDatabase itemDatabase, string slot)
    {
        foreach (string itemName in EquippedItemNames)
        {
            Item equippedItem = itemDatabase.GetItemByName(itemName);

            if (equippedItem != null && equippedItem.Slot == slot)
            {
                return equippedItem;
            }
        }

        return null;
    }

    /// <summary>
    /// Applies damage
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Dead = true;
        }

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        if (Dead)
        {
            UIManager.Instance.Log($"{Name} is defeated!");

            // Drop monster's items to the room
            if (ItemNames.Count > 0)
            {
                UIManager.Instance.Log(Name + " dropped some items:");

                foreach (string itemName in ItemNames)
                {
                    UIManager.Instance.Log(itemName);
                    TextBasedGameWorld.Instance.CurrentRoom.ItemNames.Add(itemName);
                }
            }
        }
    }

    /// <summary>
    /// Calculates the amount of damage to mitigate by armour
    /// </summary>
    /// <returns>The mitigation amount</returns>
    public float GetDamageMitigation()
    {
        var validSlots = new List<string> { "chest", "head", "legs", "shield", "feet" };

        float totalMitigation = 0;

        foreach (var itemName in EquippedItemNames)
        {
            var item = TextBasedGameWorld.Instance.ItemDatabase.GetItemByName(itemName);

            if (validSlots.Contains(item.Slot))
            {
                totalMitigation += item.Power;
            }
        }

        return totalMitigation;
    }

    /// <summary>
    /// Casts the specified spell.
    /// </summary>
    /// <param name="spellName">The name of the spell to cast.</param>
    public void CastSpell(string spellName)
    {
        Spell spell = TextBasedGameWorld.Instance.SpellDatabase.GetSpellByName(spellName);

        if (Mana < spell.ManaCost)
        {
            UIManager.Instance.Log($"{Name} doesn't have enough mana to cast their spell.");
            return;
        }

        UIManager.Instance.Log($"{Name} casts {spellName}");

        Mana -= spell.ManaCost;
        Player player = Player.Instance;

        if (spell.SpellType == SpellType.Heal)
        {
            if (spell.Target == TargetType.Self)
            {
                TakeDamage(spell.Power * -1);
                UIManager.Instance.Log($"{Name} healed itself for {spell.Power} health.");
            }
            else if (spell.Target == TargetType.Other || spell.Target == TargetType.AOE)
            {
                player.TakeDamage(spell.Power * -1);
                UIManager.Instance.Log($"{Name} healed you for {spell.Power} health.");
            }
        }
        else if (spell.SpellType == SpellType.Nuke)
        {
            if (spell.Target == TargetType.Other || spell.Target == TargetType.AOE)
            {
                UIManager.Instance.Log($"{Name} dealt {spell.Power} damage to you.");
                player.TakeDamage(spell.Power);
            }
        }
    }


}