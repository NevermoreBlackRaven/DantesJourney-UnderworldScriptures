using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the game loop
/// </summary>
public class GameManager : MonoBehaviour
{
    #region SINGLETON

    public static GameManager Instance;

    private void Awake() => Instance = this;

    #endregion

    [SerializeField] private float tickInterval = 1f;
    public CombatManager CombatManager { get; private set; }

    private void Start()
    {
        CombatManager = new CombatManager();
        StartCoroutine(TickRoutine());
    }

    /// <summary>
    /// Coroutine for handling game ticks.
    /// </summary>
    /// <returns>Returns an IEnumerator for the coroutine.</returns>
    private IEnumerator TickRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            Tick();
        }
    }

    /// <summary>
    /// Processes events for a single tick of game time
    /// </summary>
    private void Tick()
    {
        //Mobs
        MoveWanderingMonsters();

        //Combat
        HandleAggroMobs();
        CombatRound();
    }

    #region COMBAT

    /// <summary>
    /// Has aggressive monsters attack the player
    /// </summary>
    public void HandleAggroMobs()
    {
        var aggressiveMonsters = TextBasedGameWorld.Instance.CurrentRoom.MonsterNames
          .Select(name => TextBasedGameWorld.Instance.MonsterDatabase.GetMonsterByName(name))
          .Where(monster => monster.Aggressive && !monster.Dead && !monster.InCombat)
          .ToList();

        // If there are aggressive monsters, have them attack the player
        if (aggressiveMonsters.Count > 0)
        {
            foreach (var monster in aggressiveMonsters)
            {
                CombatManager.StartCombat(monster, CombatManager.Turn.Monster);
            }
        }
    }

    /// <summary>
    /// Handles a single round of turn-based combat
    /// </summary>
    public void CombatRound()
    {
        if (CombatManager.CombatActive)
        {
            if (CombatManager.CurrentTurn == CombatManager.Turn.Player)
            {
                Monster targetMonster = CombatManager.EngagedMonsters[CombatManager.PlayerTargetIndex];
                if (targetMonster.Dead) return;

                float playerDamage = Player.Instance.GetPlayerDamage() - targetMonster.GetDamageMitigation();
                if (playerDamage < 0) playerDamage = 1;
                UIManager.Instance.Log($"Player dealt {playerDamage} damage to {targetMonster.Name}!");

                targetMonster.TakeDamage(playerDamage);
              

                if (targetMonster.Dead)
                {
                    

                    CombatManager.EngagedMonsters.Remove(targetMonster);

                    if (CombatManager.EngagedMonsters.Count == 0)
                    {
                        CombatManager.EndCombat();
                    }
                    else
                    {
                        CombatManager.ChangeTarget(0);
                    }
                }
                else
                {
                    CombatManager.NextTurn();
                }
            }
            else if (CombatManager.CurrentTurn == CombatManager.Turn.Monster)
            {
                foreach (Monster monster in CombatManager.EngagedMonsters)
                {
                    if (monster.Dead) continue;

                    if (monster.SpellNames.Count > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, monster.SpellNames.Count);
                        string spellName = monster.SpellNames[randomIndex];
                        Spell spell = TextBasedGameWorld.Instance.SpellDatabase.GetSpellByName(spellName);

                        if (spell != null && monster.Mana >= spell.ManaCost)
                        {
                            monster.CastSpell(spellName);
                        }
                        else
                        {
                            float monsterDamage = GetMonsterDamage(monster) - Player.Instance.GetDamageMitigation();
                            if (monsterDamage < 0) monsterDamage = 1;
                            CombatManager.Player.TakeDamage(monsterDamage);
                            UIManager.Instance.Log($"{monster.Name} dealt {monsterDamage} damage to Player!");
                        }
                    }
                    else
                    {
                        float monsterDamage = GetMonsterDamage(monster) - Player.Instance.GetDamageMitigation();
                        if (monsterDamage < 0) monsterDamage = 1;
                        CombatManager.Player.TakeDamage(monsterDamage);
                        UIManager.Instance.Log($"{monster.Name} dealt {monsterDamage} damage to Player!");
                    }

                    if (CombatManager.Player.Dead)
                    {
                        UIManager.Instance.Log("Player is defeated!");
                        CombatManager.EndCombat();
                        break;
                    }
                }

                CombatManager.NextTurn();
            }
        }
    }

    /// <summary>
    /// Calculates the damage dealt by the given monster.
    /// </summary>
    /// <param name="monster">The Monster instance to calculate damage for.</param>
    /// <returns>Returns the calculated damage as a float.</returns>
    public float GetMonsterDamage(Monster monster)
    {
        float baseDamage = 10f;

        ItemDatabase itemDatabase = TextBasedGameWorld.Instance.ItemDatabase;
        Item equippedWeapon = monster.GetEquippedItemBySlot(itemDatabase, "weapon");

        if (equippedWeapon != null)
        {
            baseDamage += equippedWeapon.Power;
        }

        return baseDamage;
    }

    #endregion

    #region MOBS

    /// <summary>
    /// Moves monsters that are set to be able to wander to adjacent rooms
    /// </summary>
    private void MoveWanderingMonsters()
    {
        List<Monster> wanderingMonsters = TextBasedGameWorld.Instance.MonsterDatabase.Monsters.FindAll(monster => monster.Wanders && !monster.Dead && !monster.InCombat);

        foreach (Monster wanderingMonster in wanderingMonsters)
        {
            if (UnityEngine.Random.Range(0, 3) == 0)
            {
                Room currentRoom = TextBasedGameWorld.Instance.gameWorld.Rooms.Find(room => room.MonsterNames.Contains(wanderingMonster.Name));
                MoveMonsterToRandomAdjoiningRoom(wanderingMonster, currentRoom);
            }
        }
    }

    /// <summary>
    /// Moves the given monster to a random adjoining room.
    /// </summary>
    /// <param name="wanderingMonster">The Monster instance to move.</param>
    /// <param name="currentRoom">The current Room instance of the monster.</param>
    private void MoveMonsterToRandomAdjoiningRoom(Monster wanderingMonster, Room currentRoom)
    {
        List<RoomExit> availableExits = currentRoom.Exits.FindAll(exit => !string.IsNullOrEmpty(exit.RoomId));
        if (availableExits.Count > 0)
        {
            RoomExit randomExit = availableExits[UnityEngine.Random.Range(0, availableExits.Count)];

            // Check if the exit is locked and if the monster has the required item
            if (randomExit.Locked)
            {
                string requiredItemName = randomExit.RequiredItem;
                if (!wanderingMonster.ItemNames.Contains(requiredItemName))
                {
                    if (TextBasedGameWorld.Instance.CurrentRoom.Name.Equals(currentRoom.Name)) UIManager.Instance.Log($"{wanderingMonster.Name} tries to leave {randomExit.Direction.ToLower()} but it's locked.");
                    return; // Exit early if the monster doesn't have the required item
                }
            }

            Room targetRoom = TextBasedGameWorld.Instance.gameWorld.Rooms.Find(room => room.Name.Equals(randomExit.RoomId));

            currentRoom.MonsterNames.Remove(wanderingMonster.Name);
            targetRoom.MonsterNames.Add(wanderingMonster.Name);

            if (TextBasedGameWorld.Instance.CurrentRoom.Name.Equals(currentRoom.Name))
            {
                UIManager.Instance.Log($"{wanderingMonster.Name} leaves {randomExit.Direction.ToLower()}.");
            }
            if (TextBasedGameWorld.Instance.CurrentRoom.Name.Equals(targetRoom.Name))
            {
                UIManager.Instance.Log($"{wanderingMonster.Name} enters from {GetOppositeDirection(randomExit.Direction).ToLower()}.");
            }
        }
    }

    /// <summary>
    /// Retrieves the opposite direction of the given direction string.
    /// </summary>
    /// <param name="direction">The direction string to get the opposite of.</param>
    /// <returns>Returns the opposite direction as a string.</returns>
    private string GetOppositeDirection(string direction)
    {
        switch (direction.ToLower())
        {
            case "north":
                return "south";
            case "south":
                return "north";
            case "east":
                return "west";
            case "west":
                return "east";
            case "up":
                return "down";
            case "down":
                return "up";
            default:
                return null;
        }
    }

    #endregion

}