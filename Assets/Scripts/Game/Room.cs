using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a room in the game world
/// </summary>
[System.Serializable]
public class Room
{
    public string Name;
    public string Description;
    public string LongDescription;
    public string Picture;
    public List<RoomExit> Exits;
    public List<Item> Items;
    public List<InteractableObject> InteractableObjects;
    public List<string> ItemNames;
    public List<string> MonsterNames;
    public List<string> Flags;

    public Room()
    {
        Exits = new List<RoomExit>();
        Items = new List<Item>();
        InteractableObjects = new List<InteractableObject>();
        ItemNames = new List<string>();
        MonsterNames = new List<string>();
    }

    /// <summary>
    /// Sets an exit in the specified direction, with the associated roomId.
    /// </summary>
    /// <param name="direction">The direction of the exit (e.g., "north", "south", "east", "west").</param>
    /// <param name="roomId">The unique identifier of the room that the exit leads to.</param>
    public void SetExit(string direction, string roomId)
    {
        RoomExit existingExit = Exits.Find(exit => exit.Direction == direction);
        if (existingExit != null)
        {
            existingExit.RoomId = roomId;
        }
        else
        {
            Exits.Add(new RoomExit(direction, roomId));
        }
    }

    /// <summary>
    /// Checks if the room has a specific flag.
    /// </summary>
    /// <param name="flag">The flag to check for (e.g., "dark", "locked").</param>
    /// <returns>True if the room has the specified flag, otherwise false.</returns>
    public bool HasFlag(string flag)
    {
        return Flags.Any(f => f.ToLower() == flag.ToLower());
    }

    /// <summary>
    /// Gets the exit direction that leads to the target room, if available.
    /// </summary>
    /// <param name="targetRoomName">The name of the target room.</param>
    /// <returns>The exit direction if the target room is connected, otherwise an empty string.</returns>
    public string GetExitDirection(string targetRoomName)
    {
        RoomExit roomExit = Exits.Find(exit => exit.RoomId == targetRoomName);
        return roomExit != null ? roomExit.Direction : null;
    }

}

/// <summary>
/// Represents an exit to a room
/// </summary>
[System.Serializable]
public class RoomExit
{
    public string Direction;
    public string RoomId;
    public bool Locked;
    public string RequiredItem;

    public RoomExit(string direction, string roomId, bool locked = false, string requiredItem = null)
    {
        Direction = direction;
        RoomId = roomId;
        Locked = locked;
        RequiredItem = requiredItem;
    }
}