using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

/// <summary>
/// Represents the player
/// </summary>
public class Player : MonoBehaviour
{
    #region SINGLETON

    public static Player Instance;

    private void Awake() => Instance = this;

    #endregion

    public List<Item> Inventory = new List<Item>();
    public List<string> Items = new List<string>();
    public Dictionary<string, Item> EquippedItems = new Dictionary<string, Item>();
    public float Gold = 0;
    public bool InCombat = false;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public bool Dead = false;
    public float Mana = 100;
    public float MaxMana = 100;
    public List<Spell> Spells = new List<Spell>();


    void Start()
    {
        Gold = 1000;
        MemorizeSpell(TextBasedGameWorld.Instance.SpellDatabase.GetSpellByName("Fireball"));
        MemorizeSpell(TextBasedGameWorld.Instance.SpellDatabase.GetSpellByName("Heal"));
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Checks if the player has an item that acts as a light source
    /// </summary>
    /// <returns>If the player has a lightsource</returns>
    public bool HasLightSource()
    {
        List<string> lightSourceItems = new List<string> { "torch", "lantern", "glowing", "burning", "flaming", "illuminated" };
        return Inventory.Exists(item => lightSourceItems.Any(lightSource => item.Name.ToLower().Contains(lightSource.ToLower())));
    }

    /// <summary>
    /// Outputs the player's equipment
    /// </summary>
    public void ListEquipment()
    {
        if (EquippedItems.Count == 0)
        {
            UIManager.Instance.Log("You have no items equipped.");
            return;
        }

        StringBuilder equipmentList = new StringBuilder("Equipped Items:\n");

        foreach (KeyValuePair<string, Item> entry in EquippedItems)
        {
            equipmentList.AppendLine(entry.Key + " - " + entry.Value.Name);
        }

        UIManager.Instance.Log(equipmentList.ToString());
    }

    /// <summary>
    /// Outputs the player's inventory
    /// </summary>
    public void ListItems()
    {
        if (Inventory.Count == 0)
        {
            UIManager.Instance.Log("You have no items in your inventory.");
            return;
        }

        StringBuilder itemList = new StringBuilder("Inventory:\n");

        foreach (Item item in Inventory)
        {
            itemList.AppendLine($"{item.Name}"); 
        }

        UIManager.Instance.Log(itemList.ToString());
    }

    /// <summary>
    /// Outputs the player's gold amount
    /// </summary>
    public void ShowGold()
    {
        UIManager.Instance.Log($"You have {Gold} gold.");
    }

    /// <summary>
    /// Outputs the player's statistics
    /// </summary>
    public void ShowStats()
    {
        UIManager.Instance.Log($"HP: {Health}, MP: {Mana}");
    }

    /// <summary>
    /// Retrieves the equipped item in the specified slot.
    /// </summary>
    /// <param name="slot">The slot to search for an equipped item.</param>
    /// Returns an Item instance if found, otherwise null.</returns>
    public Item GetEquippedItemBySlot(string slot)
    {
        if (EquippedItems.ContainsKey(slot))
        {
            return EquippedItems[slot];
        }
        else
        {
            return null;
        }
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

        if(Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    /// <summary>
    /// Calculates the player's damage output.
    /// </summary>
    /// <returns>Returns the calculated damage as a float.</returns>
    public float GetPlayerDamage()
    {
        float baseDamage = 10f;
        Player player = Player.Instance;

        ItemDatabase itemDatabase = TextBasedGameWorld.Instance.ItemDatabase;
        Item equippedWeapon = player.GetEquippedItemBySlot("weapon");

        if (equippedWeapon != null)
        {
            baseDamage += equippedWeapon.Power;
        }

        return baseDamage;
    }

    /// <summary>
    /// Gets the amount of mitigation from the player's equipment
    /// </summary>
    /// <returns></returns>
    public float GetDamageMitigation()
    {
        var validSlots = new List<string> { "chest", "head", "legs", "shield", "feet" };

        float totalMitigation = 0;

        foreach (var item in EquippedItems.Values)
        {
            if (validSlots.Contains(item.Slot))
            {
                totalMitigation += item.Power;
            }
        }

        return totalMitigation;
    }

    /// <summary>
    /// Memorizes the specified spell.
    /// </summary>
    /// <param name="spell">The Spell instance to memorize.</param>
    public void MemorizeSpell(Spell spell)
    {
        Spells.Add(spell);
    }

    /// <summary>
    /// Checks if the specified spell is memorized.
    /// </summary>
    /// <param name="spell">The Spell instance to check.</param>
    /// <returns>Returns true if the spell is memorized, otherwise false.</returns>
    public bool IsSpellMemorized(Spell spell)
    {
        return Spells.Contains(spell);
    }

    /// <summary>
    /// Casts the specified spell on a target.
    /// </summary>
    /// <param name="spellName">The name of the spell to cast.</param>
    /// <param name="target">The name of the target to cast the spell on.</param>
    public void CastSpell(string spellName, string target)
    {
        Spell spell = Spells.FirstOrDefault(s => s.Name.ToLower() == spellName.ToLower());

        if (spell == null)
        {
            UIManager.Instance.Log("You haven't memorized this spell.");
            return;
        }

        if (Mana < spell.ManaCost)
        {
            UIManager.Instance.Log("You don't have enough mana to cast this spell.");
            return;
        }

        UIManager.Instance.Log($"You cast {spellName}");


        Mana -= spell.ManaCost;

        if (spell.SpellType == SpellType.Heal)
        {
            if (spell.Target == TargetType.Self)
            {
                TakeDamage(spell.Power * -1);
                UIManager.Instance.Log($"You healed yourself for {spell.Power} health.");
            }
            else if (spell.Target == TargetType.Other || spell.Target == TargetType.AOE)
            {
                string monsterNameInRoom = TextBasedGameWorld.Instance.CurrentRoom.MonsterNames.FirstOrDefault(name => name.ToLower().Contains(target.ToLower()));

                if (monsterNameInRoom != null)
                {
                    Monster targetMonster = TextBasedGameWorld.Instance.MonsterDatabase.GetMonsterByName(monsterNameInRoom);
                    targetMonster.TakeDamage(spell.Power * -1);
                    UIManager.Instance.Log($"You healed {targetMonster.Name} for {spell.Power} health.");
                }
                else
                {
                    UIManager.Instance.Log("There's no such monster in the room.");
                }
            }
        }
        else if (spell.SpellType == SpellType.Nuke)
        {
            if (spell.Target == TargetType.Other)
            {
                string monsterNameInRoom = TextBasedGameWorld.Instance.CurrentRoom.MonsterNames.FirstOrDefault(name => name.ToLower().Contains(target.ToLower()));

                if (monsterNameInRoom != null)
                {
                    Monster targetMonster = TextBasedGameWorld.Instance.MonsterDatabase.GetMonsterByName(monsterNameInRoom);
                    UIManager.Instance.Log($"You dealt {spell.Power} damage to {targetMonster.Name}.");
                    targetMonster.TakeDamage(spell.Power);
                    if (!targetMonster.InCombat) GameManager.Instance.CombatManager.StartCombat(targetMonster, CombatManager.Turn.Monster);
                    
                }
                else
                {
                    UIManager.Instance.Log("There's no such monster in the room.");
                }
            }
            else if (spell.Target == TargetType.AOE)
            {
                foreach (string monsterName in TextBasedGameWorld.Instance.CurrentRoom.MonsterNames)
                {
                    Monster targetMonster = TextBasedGameWorld.Instance.MonsterDatabase.GetMonsterByName(monsterName);
                    UIManager.Instance.Log($"You dealt {spell.Power} damage to {targetMonster.Name}.");
                    targetMonster.TakeDamage(spell.Power);
                    if (!targetMonster.InCombat) GameManager.Instance.CombatManager.StartCombat(targetMonster, CombatManager.Turn.Monster);
                    
                }
            }
        }
    }

    /// <summary>
    /// Outputs the player's spells
    /// </summary>
    public void ListSpells()
    {
        if (Spells.Count == 0)
        {
            UIManager.Instance.Log("You have no spells memorized.");
            return;
        }

        StringBuilder spellList = new StringBuilder("Known spells:\n");

        foreach (Spell spell in Spells)
        {
            spellList.AppendLine($"{spell.Name} - {spell.Description}");
        }

        UIManager.Instance.Log(spellList.ToString());
    }
}
