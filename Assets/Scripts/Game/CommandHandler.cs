using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the input of commands
/// </summary>
public class CommandHandler : MonoBehaviour
{
    #region SINGLETON

    public static CommandHandler Instance;

    private void Awake() => Instance = this;

    #endregion

    [SerializeField] private TMP_InputField inputField;

    public object EventSystemManager { get; private set; }
    private string lastCommand;

    void Start()
    {
        inputField.onFocusSelectAll = false;
        inputField.Select();
        inputField.ActivateInputField();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Submit();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            inputField.text = lastCommand;
    }

    /// <summary>
    /// Sumits the current typed command
    /// </summary>
    public void Submit()
    {
        ProcessCommand(inputField.text.Trim().ToLower());
    }

    /// <summary>
    /// Processes the given command string and performs the corresponding action.
    /// </summary>
    /// <param name="command">The command string to process.</param>
    public void ProcessCommand(string command)
    {
        if(command == null || command == string.Empty)
        {
            if(lastCommand != null) ProcessCommand(lastCommand);
            return;
        }

        if (Player.Instance.Dead)
        {
            UIManager.Instance.Log("You are in dead.");
            return;
        }

        lastCommand = command;
        inputField.text = "";

        UIManager.Instance.Log($">{command}");

        //Movement
        if (command.StartsWith("move "))
        {
            string direction = command.Substring(5);
            TextBasedGameWorld.Instance.MoveToRoom(direction);
        }
        else if (command.StartsWith("go "))
        {
            string direction = command.Substring(3);
            TextBasedGameWorld.Instance.MoveToRoom(direction);
        }
        else if(command.Equals("back"))
        {
            TextBasedGameWorld.Instance.Back();
        }
        else if (command.Equals("b"))
        {
            TextBasedGameWorld.Instance.Back();
        }
        else if (command.Equals("north"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("north");
        }
        else if (command.Equals("south"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("south");
        }
        else if (command.Equals("east"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("east");
        }
        else if (command.Equals("west"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("west");
        }
        else if (command.Equals("up"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("up");
        }
        else if (command.Equals("down"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("down");
        }
        else if (command.Equals("in"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("in");
        }
        else if (command.Equals("out"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("out");
        }
        else if (command.Equals("n"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("north");
        }
        else if (command.Equals("s"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("south");
        }
        else if (command.Equals("e"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("east");
        }
        else if (command.Equals("w"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("west");
        }
        else if (command.Equals("u"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("up");
        }
        else if (command.Equals("d"))
        {
            TextBasedGameWorld.Instance.MoveToRoom("down");
        }
        else if (command.Equals("flee"))
        {
            TextBasedGameWorld.Instance.Flee();
        }

        //Look
        else if (command.Equals("look"))
        {
            TextBasedGameWorld.Instance.DisplayRoomInfo();
        }
        else if (command.Equals("l"))
        {
            TextBasedGameWorld.Instance.DisplayRoomInfo();
        }
        else if (command.StartsWith("look at "))
        {
            string targetName = command.Substring(8);
            TextBasedGameWorld.Instance.Examine(targetName);
        }
        else if (command.StartsWith("look "))
        {
            string targetName = command.Substring(5);
            TextBasedGameWorld.Instance.Examine(targetName);
        }
        else if (command.StartsWith("examine "))
        {
            string targetName = command.Substring(8);
            TextBasedGameWorld.Instance.Examine(targetName);
        }
        else if (command.StartsWith("ex "))
        {
            string targetName = command.Substring(3);
            TextBasedGameWorld.Instance.Examine(targetName);
        }

        //Items
        else if (command.Equals("take all"))
        {
            TextBasedGameWorld.Instance.TakeAllItems();
        }
        else if (command.Equals("t all"))
        {
            TextBasedGameWorld.Instance.TakeAllItems();
        }
        else if (command.Equals("get all") || command.Equals("get"))
        {
            TextBasedGameWorld.Instance.TakeAllItems();
        }
        else if (command.Equals("g all"))
        {
            TextBasedGameWorld.Instance.TakeAllItems();
        }
        else if (command.StartsWith("take "))
        {
            string itemName = command.Substring(5);
            TextBasedGameWorld.Instance.TakeItem(itemName);
        }
        else if (command.StartsWith("t "))
        {
            string itemName = command.Substring(2);
            TextBasedGameWorld.Instance.TakeItem(itemName);
        }
        else if (command.StartsWith("get "))
        {
            string itemName = command.Substring(4);
            TextBasedGameWorld.Instance.TakeItem(itemName);
        }
        else if (command.StartsWith("g "))
        {
            string itemName = command.Substring(2);
            TextBasedGameWorld.Instance.TakeItem(itemName);
        }
        else if (command.StartsWith("drop "))
        {
            string itemName = command.Substring(5);
            TextBasedGameWorld.Instance.Drop(itemName);
        }
        else if (command.StartsWith("dr "))
        {
            string itemName = command.Substring(3);
            TextBasedGameWorld.Instance.Drop(itemName);
        }
        else if (command.StartsWith("eq "))
        {
            string itemName = command.Substring(3);
            TextBasedGameWorld.Instance.Equip(itemName);
        }
        else if (command.StartsWith("equip "))
        {
            string itemName = command.Substring(6);
            TextBasedGameWorld.Instance.Equip(itemName);
        }
        else if (command.StartsWith("wear "))
        {
            string itemName = command.Substring(5);
            TextBasedGameWorld.Instance.Equip(itemName);
        }
        else if (command.StartsWith("wield "))
        {
            string itemName = command.Substring(6);
            TextBasedGameWorld.Instance.Equip(itemName);
        }
        else if (command.Equals("equip"))
        {
            TextBasedGameWorld.Instance.Equip("");
        }
        else if (command.StartsWith("value "))
        {
            string itemName = command.Substring(6);
            TextBasedGameWorld.Instance.Value(itemName);
        }
        else if (command.StartsWith("val "))
        {
            string itemName = command.Substring(4);
            TextBasedGameWorld.Instance.Value(itemName);
        }
        else if (command.StartsWith("appraise "))
        {
            string itemName = command.Substring(9);
            TextBasedGameWorld.Instance.Value(itemName);
        }
        else if (command.Equals("equipment"))
        {
            Player.Instance.ListEquipment();
        }
        else if (command.Equals("eq"))
        {
            Player.Instance.ListEquipment();
        }
        else if (command.Equals("inventory"))
        {
            Player.Instance.ListItems();
        }
        else if (command.Equals("inv"))
        {
            Player.Instance.ListItems();
        }
        else if (command.Equals("i"))
        {
            Player.Instance.ListItems();
        }

        //Objects
        else if (command.StartsWith("interact "))
        {
            string objectName = command.Substring(9);
            TextBasedGameWorld.Instance.Interact(objectName);
        }
        else if (command.StartsWith("use "))
        {
            string objectName = command.Substring(4);
            TextBasedGameWorld.Instance.Interact(objectName);
        }
        else if (command.StartsWith("open "))
        {
            string objectName = command.Substring(5);
            TextBasedGameWorld.Instance.Interact(objectName);
        }
        else if (command.Equals("use"))
        {
            TextBasedGameWorld.Instance.Interact("");
        }

        //Mobs
        else if (command.StartsWith("kill "))
        {
            string monsterName = command.Substring(5);
            TextBasedGameWorld.Instance.Kill(monsterName);
        }
        else if (command.StartsWith("k "))
        {
            string monsterName = command.Substring(2);
            TextBasedGameWorld.Instance.Kill(monsterName);
        }

        else if (command.StartsWith("attack "))
        {
            string monsterName = command.Substring(7);
            TextBasedGameWorld.Instance.AttackMonster(monsterName);
        }
        else if (command.StartsWith("att "))
        {
            string monsterName = command.Substring(4);
            TextBasedGameWorld.Instance.AttackMonster(monsterName);
        }
        else if (command.StartsWith("hit "))
        {
            string monsterName = command.Substring(4);
            TextBasedGameWorld.Instance.AttackMonster(monsterName);
        }
        else if (command.Equals("attack"))
        {
            TextBasedGameWorld.Instance.AttackMonster("");
        }

        else if (command.StartsWith("consider "))
        {
            string monsterName = command.Substring(9);
            TextBasedGameWorld.Instance.Consider(monsterName);
        }
        else if (command.StartsWith("con "))
        {
            string monsterName = command.Substring(4);
            TextBasedGameWorld.Instance.Consider(monsterName);
        }
        else if (command.StartsWith("c "))
        {
            string monsterName = command.Substring(2);
            TextBasedGameWorld.Instance.Consider(monsterName);
        }

        //Shop
        else if (command.Equals("shop"))
        {
            TextBasedGameWorld.Instance.ShopAll();
        }
        else if (command.Equals("shop all"))
        {
            TextBasedGameWorld.Instance.ShopAll();
        }
        else if (command.Equals("list"))
        {
            TextBasedGameWorld.Instance.ShopAll();
        }
        else if (command.Equals("barter"))
        {
            TextBasedGameWorld.Instance.ShopAll();
        }
        else if (command.StartsWith("shop "))
        {
            string monsterName = command.Substring(5);
            TextBasedGameWorld.Instance.Shop(monsterName);
        }
        else if (command.StartsWith("buy "))
        {
            string itemName = command.Substring(4);
            TextBasedGameWorld.Instance.Buy(itemName);
        }
        else if (command.StartsWith("purchase "))
        {
            string itemName = command.Substring(9);
            TextBasedGameWorld.Instance.Buy(itemName);
        }
        else if (command.Equals("gold") || command.Equals("gp") || command.Equals("money") || command.Equals("cash"))
        {
            Player.Instance.ShowGold();
        }
        else if (command.Equals("stats") || command.Equals("stat") || command.Equals("hp") || command.Equals("mp"))
        {
            Player.Instance.ShowStats();
        }
        else if (command.StartsWith("sell "))
        {
            string[] commandWords = command.ToLower().Split(' ');

            if (commandWords.Length >= 3 && commandWords[0] == "sell")
            {
                string sellItemName = commandWords[1];
                int shopkeeperNameStartIndex = commandWords[2] == "to" ? 3 : 2;
                string monsterName = string.Join(" ", commandWords, shopkeeperNameStartIndex, commandWords.Length - shopkeeperNameStartIndex);

                TextBasedGameWorld.Instance.Sell(sellItemName, monsterName);
            }
            else if(commandWords.Length == 2)
            {
                string sellItemName = commandWords[1];
                TextBasedGameWorld.Instance.Sell(sellItemName, "");
            }
        }
        else if (command.StartsWith("give "))
        {
            string[] commandWords = command.ToLower().Split(' ');

            if (commandWords.Length >= 3 && commandWords[0] == "give")
            {
                string giveItemName = commandWords[1];
                int mobNameStartIndex = commandWords[2] == "to" ? 3 : 2;
                string monsterName = string.Join(" ", commandWords, mobNameStartIndex, commandWords.Length - mobNameStartIndex);

                TextBasedGameWorld.Instance.Give(giveItemName, monsterName);
            }
        }

        //Spells
        else if (command.StartsWith("cast "))
        {
            string[] commandWords = command.Split(' ');

            if (commandWords.Length >= 3)
            {
                string spellName = commandWords[1];
                string targetName = commandWords[2];

                Player.Instance.CastSpell(spellName, targetName);
            }
            else if(commandWords.Length == 2)
            {
                string spellName = commandWords[1];
                Player.Instance.CastSpell(spellName, "");
            }
        }
        else if (command.Equals("cast"))
        {
            UIManager.Instance.Log("Cast which spell?");
        }
        else if(command.Equals("spells"))
        {
            Player.Instance.ListSpells();
        }


        else
        {
            UIManager.Instance.Log($"You don't know the word \"{command}\"");
        }

        UIManager.Instance.Log("");

        inputField.Select();
        inputField.ActivateInputField();
    }

    /// <summary>
    /// Sets a command to the input directly
    /// </summary>
    /// <param name="command">The command</param>
    public void SetCommand(string command)
    { 
        inputField.text = command + " ";
        inputField.Select();
        inputField.ActivateInputField();
        inputField.MoveToEndOfLine(false,false);
    }
}
