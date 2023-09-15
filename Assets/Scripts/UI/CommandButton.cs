using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A command UI button
/// </summary>
public class CommandButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    public string Command;

    // Start is called before the first frame update
    void Start()
    {
        if (Command != null) SetCommand(Command);
    }

    /// <summary>
    /// Sets the button's command
    /// </summary>
    /// <param name="command">The command</param>
    public void SetCommand(string command)
    {
        label.text = command;
        Command = command;
    }

    /// <summary>
    /// Click functionality
    /// </summary>
    public void Clicked()
    {
        CommandHandler.Instance.SetCommand(Command);
    }
}
