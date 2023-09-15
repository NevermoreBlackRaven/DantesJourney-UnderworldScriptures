#if UNITY_EDITOR


using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class LevelEditor : EditorWindow
{
    private GameWorld gameWorld;
    private Vector2 scrollPos;
    private Dictionary<string, Room> roomLookup;
    private bool[] roomFoldouts;
    private bool[] itemsFoldouts;
    private bool[] interactablesFoldouts;

    private Dictionary<int, bool> roomFoldoutStates = new Dictionary<int, bool>();

    private Vector2 scrollPosition;
    private readonly string[] directions = { "north", "south", "east", "west", "up", "down" };

    private string gameWorldFilePath;

    [MenuItem("Text Adventure/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnEnable()
    {
        gameWorldFilePath = Path.Combine(FileManager.GetDocumentsPath(), "World.json");
        gameWorld = new GameWorld();
        LoadGameWorld();
        roomFoldouts = new bool[gameWorld.Rooms.Count];
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 30));

        if (gameWorld != null)
        {
            for (int i = 0; i < gameWorld.Rooms.Count; i++)
            {
                Room room = gameWorld.Rooms[i];

                if (!roomFoldoutStates.ContainsKey(i))
                {
                    roomFoldoutStates[i] = false;
                }

                roomFoldoutStates[i] = EditorGUILayout.Foldout(roomFoldoutStates[i], room.Name);

                if (roomFoldoutStates[i])
                {
                    EditorGUI.indentLevel++;
                    EditRoom(room);
                    EditorGUI.indentLevel--;
                }
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.Space();
        if (GUILayout.Button("Add Room"))
        {
            AddRoom();
        }

        if (GUILayout.Button("Save"))
        {
            SaveGameWorld();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// Edits the properties of a given Room instance.
    /// </summary>
    /// <param name="room">The Room instance to edit.</param>
    private void EditRoom(Room room)
    {
        room.Name = EditorGUILayout.TextField("Name", room.Name);
        room.Description = EditorGUILayout.TextField("Description", room.Description);
        room.LongDescription = EditorGUILayout.TextField("Long", room.LongDescription);
        room.Picture = EditorGUILayout.TextField("Picture", room.Picture);
        room.Flags = EditStringList("Flags", room.Flags);

        EditorGUILayout.LabelField("Exits", EditorStyles.boldLabel);

        for (int i = 0; i < directions.Length; i++)
        {
            string direction = directions[i];
            RoomExit exit = room.Exits.Find(e => e.Direction == direction);
            string exitRoomId = exit != null ? exit.RoomId : "";

            EditorGUI.indentLevel++;
            string newExitRoomId = EditorGUILayout.TextField($"{direction.Substring(0, 1).ToUpper() + direction.Substring(1)}", exitRoomId);
            EditorGUI.indentLevel--;

            if (newExitRoomId != exitRoomId)
            {
                if (exit != null)
                {
                    room.Exits.Remove(exit);
                }

                if (!string.IsNullOrEmpty(newExitRoomId))
                {
                    room.Exits.Add(new RoomExit(direction, newExitRoomId));
                }
            }
        }

        EditRoomItems(room);
        EditRoomMonsters(room);
        EditRoomInteractableObjects(room);
    }

    /// <summary>
    /// Edits a string list with the given list name.
    /// </summary>
    /// <param name="listName">The name of the list to edit.</param>
    /// <param name="list">The list of strings to edit.</param>
    /// <returns>Returns the edited list of strings.</returns>
    private List<string> EditStringList(string listName, List<string> list)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField(listName);

        int removeIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            list[i] = EditorGUILayout.TextField(list[i]);

            if (GUILayout.Button("Remove"))
            {
                removeIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
        {
            list.RemoveAt(removeIndex);
        }

        if (GUILayout.Button("Add " + listName))
        {
            list.Add("");
        }

        EditorGUILayout.EndVertical();

        return list;
    }

    /// <summary>
    /// Edits the items of a given Room instance.
    /// </summary>
    /// <param name="room">The Room instance to edit.</param>
    private void EditRoomItems(Room room)
    {
        EditorGUILayout.LabelField("Items:");
        EditorGUI.indentLevel++;

        for (int i = 0; i < room.ItemNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            room.ItemNames[i] = EditorGUILayout.TextField(room.ItemNames[i]);
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                room.ItemNames.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Item"))
        {
            room.ItemNames.Add("New Item");
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Edits the monsters of a given Room instance.
    /// </summary>
    /// <param name="room">The Room instance to edit.</param>
    private void EditRoomMonsters(Room room)
    {
        EditorGUILayout.LabelField("Monsters:");
        EditorGUI.indentLevel++;

        for (int i = 0; i < room.MonsterNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            room.MonsterNames[i] = EditorGUILayout.TextField(room.MonsterNames[i]);
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                room.MonsterNames.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Monster"))
        {
            room.MonsterNames.Add("New Monster");
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Edits the objects of a given Room instance.
    /// </summary>
    /// <param name="room">The Room instance to edit.</param>
    private void EditRoomInteractableObjects(Room room)
    {
        EditorGUILayout.LabelField("Interactable Objects:");
        EditorGUI.indentLevel++;

        for (int i = 0; i < room.InteractableObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            room.InteractableObjects[i].Name = EditorGUILayout.TextField("Name", room.InteractableObjects[i].Name);
            room.InteractableObjects[i].Description = EditorGUILayout.TextField("Description", room.InteractableObjects[i].Description);
            room.InteractableObjects[i].InteractionItemName = EditorGUILayout.TextField("Item", room.InteractableObjects[i].InteractionItemName);
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                room.InteractableObjects.RemoveAt(i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Interactable Object"))
        {
            //room.InteractableObjects.Add(new InteractableObject { Name = "New Object", Description = "A new interactable object.", InteractionItemName = "New Item" });
            room.InteractableObjects.Add(new InteractableObject("", ""));
        }

        EditorGUI.indentLevel--;
    }

    /// <summary>
    /// Adds a room
    /// </summary>
    private void AddRoom()
    {
        Room newRoom = new Room
        {
            //RoomId = Guid.NewGuid().ToString(),
            Name = "New Room",
            Description = "A new room.",
            Exits = new List<RoomExit>()
        };
        gameWorld.Rooms.Add(newRoom);
    }


    #region FILE MANAGEMENT

    /// <summary>
    /// Loads the world from JSON
    /// </summary>
    private void LoadGameWorld()
    {
        TextAsset gameWorldData = Resources.Load<TextAsset>("World");

        if (gameWorldData == null)
        {
            Debug.LogError("Failed to load game world data from Resources folder.");
            return;
        }

        gameWorld = JsonUtility.FromJson<GameWorld>(gameWorldData.text);
        GenerateRoomLookup();
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

    /// <summary>
    /// Saves the world to JSON
    /// </summary>
    private void SaveGameWorld()
    {
        string gameWorldData = JsonUtility.ToJson(gameWorld, true);
        File.WriteAllText(gameWorldFilePath, gameWorldData);
        AssetDatabase.Refresh();

        Debug.Log("World saved");
    }


    #endregion
}

#endif