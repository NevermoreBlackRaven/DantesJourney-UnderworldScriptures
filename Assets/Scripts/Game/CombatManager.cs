using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles turn based combat
/// </summary>
public class CombatManager
{
    public bool CombatActive { get; private set; }
    public Player Player => Player.Instance;
    public List<Monster> EngagedMonsters { get; private set; }
    public int PlayerTargetIndex { get; private set; }

    public enum Turn
    {
        Player,
        Monster
    }

    public Turn CurrentTurn { get; private set; }

    public CombatManager()
    {
        CombatActive = false;
        CurrentTurn = Turn.Player;
        EngagedMonsters = new List<Monster>();
        PlayerTargetIndex = 0;
    }

    /// <summary>
    /// Starts combat with the given monster.
    /// </summary>
    /// <param name="monster">The Monster instance to start combat with.</param>
    /// <param name="turn">The initial turn of the combat (player or monster).</param>
    public void StartCombat(Monster monster, Turn turn)
    {
        CombatActive = true;
        EngagedMonsters.Add(monster);
        monster.InCombat = true;
        Player.InCombat = true;
        CurrentTurn = turn;
    }

    /// <summary>
    /// Ends combat
    /// </summary>
    public void EndCombat()
    {
        CombatActive = false;
        EngagedMonsters.Clear();
        Player.InCombat = false;
    }

    /// <summary>
    /// Changes the turn
    /// </summary>
    public void NextTurn()
    {
        CurrentTurn = CurrentTurn == Turn.Player ? Turn.Monster : Turn.Player;
    }

    /// <summary>
    /// Changes the player's target during combat.
    /// </summary>
    /// <param name="index">The index of the new target monster in the EngagedMonsters list.</param>
    public void ChangeTarget(int index)
    {
        if (index >= 0 && index < EngagedMonsters.Count)
        {
            PlayerTargetIndex = index;
        }
    }
}