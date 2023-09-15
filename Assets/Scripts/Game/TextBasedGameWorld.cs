using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

/// <summary>
/// Represents the game world, a series of rooms
/// </summary>
[System.Serializable]
public class GameWorld
{
    public List<Room> Rooms;

    public GameWorld()
    {
        Rooms = new List<Room>();
    }
}

/// <summary>
/// Handles the logic for the game world
/// </summary>
public class TextBasedGameWorld : MonoBehaviour
{
    #region SINGLETON

    public static TextBasedGameWorld Instance;

    void Awake() => Instance = this;

    #endregion

    public GameWorld gameWorld;

    private Dictionary<string, Room> roomLookup;
    public Room CurrentRoom;
    private string lastDirection;

    public ItemDatabase ItemDatabase;
    public MonsterDatabase MonsterDatabase;
    public SpellDatabase SpellDatabase;

    private void Start()
    {
        LoadGameWorld();
        GenerateRoomLookup();

        // Set the starting room
        if (roomLookup.Count > 0)
        {
            CurrentRoom = gameWorld.Rooms[0];
            DisplayRoomInfo();
            UIManager.Instance.Log("");
        }
    }

    private void Update()
    {
        HandlePlayerInput();
    }

    /// <summary>
    /// Handles keyboard shortcuts
    /// </summary>
    private void HandlePlayerInput()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToRoom("north");
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToRoom("south");
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToRoom("west");
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToRoom("east");
            }
        }
    }

    /// <summary>
    /// Gets a room by its unique identifier.
    /// </summary>
    /// <param name="roomName">The unique identifier of the room.</param>
    /// <returns>The Room instance if found, otherwise null.</returns>
    public Room GetRoomById(string roomName)
    {
        return gameWorld.Rooms.FirstOrDefault(room => room.Name == roomName);
    }

    /// <summary>
    /// Gets a list of Monster instances currently in the room.
    /// </summary>
    /// <returns>A list of Monster instances.</returns>
    private List<Monster> GetCurrentRoomMonsters()
    {
        return CurrentRoom.MonsterNames.Select(name => MonsterDatabase.GetMonsterByName(name)).ToList();
    }

    /// <summary>
    /// Moves the player to the room in the specified direction.
    /// </summary>
    /// <param name="direction">The direction to move (e.g., "north", "south", "east", "west").</param>
    public void MoveToRoom(string direction)
    {
        if(Player.Instance.InCombat)
        {
            UIManager.Instance.Log("You are in combat.");
            return;
        }

        RoomExit exit = CurrentRoom.Exits.Find(e => e.Direction == direction);
        if (exit != null)
        {
            if (exit.Locked)
            {
                Item requiredItem = Player.Instance.Inventory.Find(item => item.Name.Contains(exit.RequiredItem));
                if (requiredItem != null)
                {
                    UIManager.Instance.Log("You used the " + requiredItem.Name + " to unlock the door.");
                }
                else
                {
                    UIManager.Instance.Log("The door is locked. You need the " + exit.RequiredItem + " to unlock it.");
                    return;
                }
            }

            if (roomLookup.ContainsKey(exit.RoomId))
            {
                lastDirection = direction;
                CurrentRoom = roomLookup[exit.RoomId];
                DisplayRoomInfo();
            }
        }
        else
        {
            UIManager.Instance.Log("There is no exit in that direction.");
        }
    }

    /// <summary>
    /// Allows the player to flee from combat in a random direction
    /// </summary>
    public void Flee()
    {
        if (Player.Instance.InCombat)
        {
            if (CurrentRoom.Exits.Count > 0)
            {
                if (UnityEngine.Random.Range(0, 4) == 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, CurrentRoom.Exits.Count);
                    RoomExit randomExit = CurrentRoom.Exits[randomIndex];

                    if (roomLookup.ContainsKey(randomExit.RoomId))
                    {
                        GameManager.Instance.CombatManager.EndCombat();
                        CurrentRoom = roomLookup[randomExit.RoomId];
                        UIManager.Instance.Log($"You flee {randomExit.Direction}.");
                        DisplayRoomInfo();
                    }
                }
                else
                {
                    UIManager.Instance.Log("You try to flee but fail!");
                }
            }
            else
            {
                UIManager.Instance.Log("There are no exits to flee through.");
            }
        }
        else
        {
            UIManager.Instance.Log("You are not in combat.");
        }
    }

    /// <summary>
    /// Allows the player to go back to the previously visited room
    /// </summary>
    public void Back()
    {
        if(lastDirection != null)
        {
            string oppositeDirection = "";

            switch(lastDirection)
            {
                case "north":
                    oppositeDirection = "south";
                    break;
                case "south":
                    oppositeDirection = "north";
                    break;
                case "east":
                    oppositeDirection = "west";
                    break;
                case "west":
                    oppositeDirection = "east";
                    break;
                case "in":
                    oppositeDirection = "out";
                    break;
                case "out":
                    oppositeDirection = "in";
                    break;
                case "up":
                    oppositeDirection = "down";
                    break;
                case "down":
                    oppositeDirection = "up";
                    break;
            }

            MoveToRoom(oppositeDirection);
        }
    }

    /// <summary>
    /// Outputs the current room's informationm
    /// </summary>
    public void DisplayRoomInfo()
    {
        UIManager.Instance.Log("");

        if (CurrentRoom.HasFlag("dark") && !Player.Instance.HasLightSource())
        {
            UIManager.Instance.Log("It is too dark to see anything.");
            UIManager.Instance.SetPicture("Dark");
        }
        else
        {
            UIManager.Instance.Log("<b>You are in " + CurrentRoom.Description + ".</b>");
            if(CurrentRoom.LongDescription != string.Empty) UIManager.Instance.Log(CurrentRoom.LongDescription);
            UIManager.Instance.Log("");

            // Display items
            if (CurrentRoom.ItemNames.Count > 0)
            {
                UIManager.Instance.Log("Items Here:");


                foreach (string itemName in CurrentRoom.ItemNames)
                {
                    Item item = ItemDatabase.GetItemByName(itemName);
                    if (item != null)
                    {
                        UIManager.Instance.Log($" - {item.Name}");
                    }
                }
            }

            // Display interactable objects
            if (CurrentRoom.InteractableObjects.Count > 0)
            {
                UIManager.Instance.Log("Objects Here:");

                foreach (InteractableObject obj in CurrentRoom.InteractableObjects)
                    UIManager.Instance.Log($" - {obj.Name}");
            }

            // Display monsters
            if (GetCurrentRoomMonsters().Count > 0)
            {
                UIManager.Instance.Log("Monsters here:");

                foreach (Monster monster in GetCurrentRoomMonsters())
                {
                    if (monster != null)
                        UIManager.Instance.Log(" - " + monster.Name);
                    else
                        Debug.LogError($"Cannot find monster in database.");
                }
            }

            // Display exits
            UIManager.Instance.Log("Exits:");

            foreach (RoomExit exit in CurrentRoom.Exits)
            {
                UIManager.Instance.Log(" - " + char.ToUpper(exit.Direction[0]) + exit.Direction.Substring(1));
            }

            UIManager.Instance.SetPicture(CurrentRoom.Picture);
        }
    }

    /// <summary>
    /// Attempts to take an item from the current room by matching a partial item name.
    /// </summary>
    /// <param name="partialItemName">A partial item name used to find a matching item in the room.</param>
    public void TakeItem(string partialItemName)
    {
        partialItemName = partialItemName.ToLower();

        string itemName = CurrentRoom.ItemNames.Where(name => ItemDatabase.GetItemByName(name) != null).FirstOrDefault(name => ItemDatabase.GetItemByName(name).Name.ToLower().Contains(partialItemName.ToLower()));

        if (itemName != null)
        {
            Item itemToTake = ItemDatabase.GetItemByName(itemName);
            Player.Instance.Inventory.Add(itemToTake);
            CurrentRoom.ItemNames.Remove(itemName);
            UIManager.Instance.Log($"You take the {itemToTake.Name}.");
        }
        else
        {
            UIManager.Instance.Log($"Could not find an item containing '{partialItemName}' in this room.");
        }

    }

    /// <summary>
    /// Takes every item in the current room
    /// </summary>
    public void TakeAllItems()
    {
        if (CurrentRoom.ItemNames.Count > 0)
        {
            foreach (string itemName in CurrentRoom.ItemNames)
            {
                Item item = ItemDatabase.GetItemByName(itemName);

                if (item != null)
                {
                    Player.Instance.Inventory.Add(item);
                    UIManager.Instance.Log("You have taken the " + item.Name + ".");
                }
            }
            CurrentRoom.ItemNames.Clear();
        }
        else
        {
            UIManager.Instance.Log("There are no items in this room.");
        }
    }

    /// <summary>
    /// Drops the specified item from the player's inventory into the current room.
    /// </summary>
    /// <param name="itemName">The name of the item to drop.</param>
    public void Drop(string itemName)
    {
        Item itemToDrop = Player.Instance.Inventory.Find(item => item.Name.ToLower().Contains(itemName.ToLower()));

        if (itemToDrop != null)
        {
            Player.Instance.Inventory.Remove(itemToDrop);
            CurrentRoom.ItemNames.Add(itemToDrop.Name);
            UIManager.Instance.Log("You dropped the " + itemToDrop.Name + ".");
        }
        else
        {
            UIManager.Instance.Log("You don't have this item in your inventory.");
        }
    }

    /// <summary>
    /// Interacts with an object in the current room.
    /// </summary>
    /// <param name="objectName">The name of the object to interact with.</param>
    public void Interact(string objectName)
    {
        InteractableObject interactableObject = CurrentRoom.InteractableObjects.Find(obj => obj.Name.ToLower().Contains(objectName.ToLower()));

        if (interactableObject != null)
        {
            UIManager.Instance.Log("You interact with the " + interactableObject.Name + ".");

            if(interactableObject.InteractionItemName != null)
            {
                CurrentRoom.ItemNames.Add(interactableObject.InteractionItemName);
                UIManager.Instance.Log("The " + interactableObject.Name + " revealed a hidden item: " + interactableObject.InteractionItemName + ".");
                interactableObject.InteractionItemName = null;
            }
        }
        else
        {
            UIManager.Instance.Log($"You don't see that");
        }
    }

    /// <summary>
    /// Examines a target (item or monster) in the current room.
    /// </summary>
    /// <param name="targetName">The name of the target to examine.</param>
    public void Examine(string targetName)
    {
        var itemsInRoom = CurrentRoom.ItemNames.Select(name => ItemDatabase.GetItemByName(name)).Where(item => item != null).ToList();
        Item itemInRoom = itemsInRoom.Find(item => item.Name.ToLower().Contains(targetName.ToLower()));
        Item itemInInventory = Player.Instance.Inventory.Find(item => item.Name.ToLower().Contains(targetName.ToLower()));
        Item itemEquipped = Player.Instance.Inventory.Find(item => item.Name.ToLower().Contains(targetName.ToLower()));
        InteractableObject interactableObject = CurrentRoom.InteractableObjects.Find(obj => obj.Name.ToLower().Contains(targetName.ToLower()));
        Monster monster = GetCurrentRoomMonsters().Find(monster => monster.Name.ToLower().Contains(targetName.ToLower()));


        if (itemInRoom != null)
            UIManager.Instance.Log(itemInRoom.Description);
        else if (itemInInventory != null)
            UIManager.Instance.Log(itemInInventory.Description);
        else if (itemEquipped != null)
            UIManager.Instance.Log(itemEquipped.Description);
        else if (interactableObject != null)
            UIManager.Instance.Log(interactableObject.Description);
        else if (monster != null)
            UIManager.Instance.Log(monster.Description);
        else
            UIManager.Instance.Log($"You don't see a {targetName}");
    }

    /// <summary>
    /// Determines the value of an item 
    /// </summary>
    /// <param name="targetName">The name of the item to determine the value of.</param>
    public void Value(string targetName)
    {
        float value = 0;
        string itemName = "";

        var itemsInRoom = CurrentRoom.ItemNames.Select(name => ItemDatabase.GetItemByName(name)).Where(item => item != null).ToList();
        Item itemInRoom = itemsInRoom.Find(item => item.Name.ToLower().Contains(targetName.ToLower()));
        Item itemInInventory = Player.Instance.Inventory.Find(item => item.Name.ToLower().Contains(targetName.ToLower()));
        Item itemEquipped = Player.Instance.Inventory.Find(item => item.Name.ToLower().Contains(targetName.ToLower()));

        if (itemInRoom != null)
        {
            value = itemInRoom.Value;
            itemName = itemInRoom.Name;
        }
        else if (itemInInventory != null)
        {
            value = itemInInventory.Value;
            itemName = itemInInventory.Name;
        }
        else if (itemEquipped != null)
        {
            value = itemEquipped.Value;
            itemName = itemEquipped.Name;
        }

        if (value > 0)
            UIManager.Instance.Log($"You value {itemName} at {value}");
        else
            UIManager.Instance.Log($"That has no monetary value");
    }

    /// <summary>
    /// Equips an item from the player's inventory.
    /// </summary>
    /// <param name="itemName">The name of the item to equip.</param>
    public void Equip(string itemName)
    {
        Item itemToEquip = Player.Instance.Inventory.Find(item => item.Name.ToLower().Contains(itemName.ToLower()));

        if (itemToEquip != null)
        {
            if (!string.IsNullOrEmpty(itemToEquip.Slot))
            {
                if (Player.Instance.EquippedItems.ContainsKey(itemToEquip.Slot))
                {
                    Player.Instance.Inventory.Add(Player.Instance.EquippedItems[itemToEquip.Slot]);
                    UIManager.Instance.Log("Unequipped " + Player.Instance.EquippedItems[itemToEquip.Slot].Name + " from the " + itemToEquip.Slot + " slot.");
                }

                Player.Instance.EquippedItems[itemToEquip.Slot] = itemToEquip;
                Player.Instance.Inventory.Remove(itemToEquip);
                UIManager.Instance.Log("Equipped " + itemToEquip.Name + " in the " + itemToEquip.Slot + " slot.");
            }
            else
            {
                UIManager.Instance.Log("This item cannot be equipped.");
            }
        }
        else
        {
            UIManager.Instance.Log("You don't have this item in your inventory.");
        }
    }

    /// <summary>
    /// Kills the specified monster in the current room.
    /// </summary>
    /// <param name="monsterName">The name of the monster to kill.</param>
    public void Kill(string monsterName)
    {
        Monster monsterToKill = GetCurrentRoomMonsters().Find(monster => monster.Name.ToLower().Contains(monsterName.ToLower()));

        if (monsterToKill != null)
        {
            CurrentRoom.MonsterNames.Remove(monsterToKill.Name);
            UIManager.Instance.Log("You killed the " + monsterToKill.Name + ".");

            // Drop monster's items to the room
            if (monsterToKill.ItemNames.Count > 0)
            {
                UIManager.Instance.Log(monsterToKill.Name + " dropped some items:");

                foreach (string itemName in monsterToKill.ItemNames)
                {
                    UIManager.Instance.Log(itemName);
                    CurrentRoom.ItemNames.Add(itemName);
                }
            }
        }
        else
        {
            UIManager.Instance.Log("There's no such monster in the room.");
        }
    }

    /// <summary>
    /// Considers the aggression of a monster in the current room.
    /// </summary>
    /// <param name="monsterName">The name of the monster to consider.</param>
    public void Consider(string monsterName)
    {
        Monster mob = GetCurrentRoomMonsters().Find(monster => monster.Name.ToLower().Contains(monsterName.ToLower()));

        UIManager.Instance.Log($"{mob.Health} hp.");


        if (mob != null)
        {
            if(mob.Aggressive)
            {
                UIManager.Instance.Log($"{mob.Name} scowls at you, ready to attack.");

            }
            else
            {
                UIManager.Instance.Log($"{mob.Name} regards you indifferently.");
            }
        }
        else
        {
            UIManager.Instance.Log("I don't see them.");
        }
    }

    /// <summary>
    /// Lists all the shops in the current room
    /// </summary>
    public void ShopAll()
    {
        bool foundShopkeeper = false;

        foreach (string monsterName in CurrentRoom.MonsterNames)
        {
            Monster monster = MonsterDatabase.GetMonsterByName(monsterName);

            if (monster != null && monster.ShopKeeper)
            {
                Shop(monsterName);
                foundShopkeeper = true;
            }
        }

        if (!foundShopkeeper)
        {
            UIManager.Instance.Log("There are no shopkeepers in this room.");
        }
    }

    /// <summary>
    /// Opens the shop interface for a monster in the current room.
    /// </summary>
    /// <param name="monsterName">The name of the monster that runs the shop.</param>
    public void Shop(string monsterName)
    {
        Monster shopKeeper = GetCurrentRoomMonsters().Find(monster => monster.ShopKeeper && monster.Name.ToLower().Contains(monsterName.ToLower()));

        if (shopKeeper != null)
        {
            UIManager.Instance.Log("Items available for purchase from " + shopKeeper.Name + ":");

            foreach(string itemName in shopKeeper.ItemNames)
            {
                Item item = ItemDatabase.GetItemByName(itemName);

                if(item != null)
                    UIManager.Instance.Log($"{item.Name} - {item.Value} gold");
            }
        }
        else
        {
            UIManager.Instance.Log("There's no shopkeeper with that name in this room.");
        }
    }

    /// <summary>
    /// Buys an item from a shop.
    /// </summary>
    /// <param name="itemName">The name of the item to buy.</param>
    public void Buy(string itemName)
    {
        itemName = itemName.ToLower();
        bool itemBought = false;

        foreach (string monsterName in CurrentRoom.MonsterNames)
        {
            Monster shopMonster = MonsterDatabase.GetMonsterByName(monsterName);

            if (shopMonster != null && shopMonster.ShopKeeper)
            {
                string foundItemName = shopMonster.ItemNames.Find(item => ItemDatabase.GetItemByName(item) != null && ItemDatabase.GetItemByName(item).Name.ToLower().Contains(itemName));

                if (foundItemName != null)
                {
                    Item itemToBuy = ItemDatabase.GetItemByName(foundItemName);
                    float price = itemToBuy.Value;

                    if (Player.Instance.Gold >= price)
                    {
                        Player.Instance.Inventory.Add(itemToBuy);
                        Player.Instance.Gold -= price;
                        shopMonster.ItemNames.Remove(foundItemName);
                        UIManager.Instance.Log($"You bought the {itemToBuy.Name} for {price} gold.");
                        itemBought = true;
                    }
                    else
                    {
                        UIManager.Instance.Log($"You do not have enough gold to buy the {itemToBuy.Name}.");
                    }
                }
            }
        }

        if (!itemBought)
        {
            UIManager.Instance.Log($"Could not find an item containing '{itemName}' in any shopkeepers' inventory.");
        }
    }

    /// <summary>
    /// Sells an item to a shopkeeper.
    /// </summary>
    /// <param name="itemName">The name of the item to sell.</param>
    /// <param name="monsterName">The name of the monster that runs the shop.</param>
    public void Sell(string itemName, string monsterName)
    {
        // Find the shopkeeper monster in the room
        Monster shopMonster = CurrentRoom.MonsterNames
            .Select(monsterName => MonsterDatabase.GetMonsterByName(monsterName))
            .FirstOrDefault(monster => monster.Name.ToLower().Contains(monsterName.ToLower()) && monster.ShopKeeper);

        if (shopMonster == null)
        {
            UIManager.Instance.Log("There is no shopkeeper named " + monsterName + " here.");
            return;
        }

        // Find the item in the player's inventory
        Item itemToSell = Player.Instance.Inventory.FirstOrDefault(item => item.Name.ToLower().Contains(itemName.ToLower()));

        if (itemToSell == null)
        {
            UIManager.Instance.Log("You don't have a " + itemName + " in your inventory.");
            return;
        }

        // Remove the item from the player's inventory
        Player.Instance.Inventory.Remove(itemToSell);

        // Add the item to the shopkeeper's inventory
        shopMonster.ItemNames.Add(itemToSell.Name);

        // Update the player's gold amount
        Player.Instance.Gold += itemToSell.Value;

        UIManager.Instance.Log("You sold " + itemToSell.Name + " to " + shopMonster.Name + " for " + itemToSell.Value + " gold.");
    }

    /// <summary>
    /// Gives an item to a monster in the current room.
    /// </summary>
    /// <param name="itemName">The name of the item to give.</param>
    /// <param name="monsterName">The name of the monster to receive the item.</param>
    public void Give(string itemName, string monsterName)
    {
        // Find the shopkeeper monster in the room
        Monster mob = CurrentRoom.MonsterNames
            .Select(monsterName => MonsterDatabase.GetMonsterByName(monsterName))
            .FirstOrDefault(monster => monster.Name.ToLower().Contains(monsterName.ToLower()));

        if (mob == null)
        {
            UIManager.Instance.Log("There is nobody named " + monsterName + " here.");
            return;
        }

        // Find the item in the player's inventory
        Item itemToSell = Player.Instance.Inventory.FirstOrDefault(item => item.Name.ToLower().Contains(itemName.ToLower()));

        if (itemToSell == null)
        {
            UIManager.Instance.Log("You don't have a " + itemName + " in your inventory.");
            return;
        }

        // Remove the item from the player's inventory
        Player.Instance.Inventory.Remove(itemToSell);

        // Add the item to the mob's inventory
        mob.ItemNames.Add(itemToSell.Name);

        UIManager.Instance.Log("You gave " + itemToSell.Name + " to " + mob.Name);
    }

    /// <summary>
    /// Initiates combat by attacking a monster in the current room.
    /// </summary>
    /// <param name="monsterName">The name of the monster to attack.</param>
    public void AttackMonster(string monsterName)
    {
        Monster targetMonster = CurrentRoom.MonsterNames
            .Select(monsterName => MonsterDatabase.GetMonsterByName(monsterName))
            .FirstOrDefault(monster => monster.Name.ToLower().Contains(monsterName.ToLower()));

        if (targetMonster != null)
        {
            GameManager.Instance.CombatManager.StartCombat(targetMonster, CombatManager.Turn.Player);
            UIManager.Instance.Log($"You start combat with {targetMonster.Name}!");
        }
        else
        {
            UIManager.Instance.Log("There is no monster with that name in the room.");
        }
    }

    #region FILE MANAGEMENT

    /// <summary>
    /// Loads the game world from JSON
    /// </summary>
    private void LoadGameWorld()
    {
        ItemDatabase = new ItemDatabase();
        ItemDatabase.LoadItemDatabase();
        MonsterDatabase = new MonsterDatabase();
        MonsterDatabase.LoadMonsters();
        SpellDatabase = new SpellDatabase();
        SpellDatabase.LoadSpellDatabase();

        TextAsset gameWorldData = Resources.Load<TextAsset>("World");

        if (gameWorldData == null)
        {
            Debug.LogError("Failed to load game world data from Resources folder.");
            return;
        }

        gameWorld = JsonUtility.FromJson<GameWorld>(gameWorldData.text);
        GenerateRoomLookup();
        CurrentRoom = gameWorld.Rooms[0];
    }

    /// <summary>
    /// Generates the room lookup table
    /// </summary>
    private void GenerateRoomLookup()
    {
        roomLookup = new Dictionary<string, Room>();
        foreach (Room room in gameWorld.Rooms)
        {
            roomLookup.Add(room.Name, room);
        }
    }

    #endregion
}