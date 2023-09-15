using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// An exit UI button
/// </summary>
public class ExitButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    public string Direction;

    // Start is called before the first frame update
    void Start()
    {
        if (Direction != null) SetDirection(Direction);
    }

    /// <summary>
    /// Sets the button's direction
    /// </summary>
    /// <param name="direction">The direction</param>
    public void SetDirection(string direction)
    {
        label.text = direction;
        Direction = direction;
    }

    /// <summary>
    /// Click functionality
    /// </summary>
    public void Clicked()
    {
        UIManager.Instance.Log($">{Direction}");
        TextBasedGameWorld.Instance.MoveToRoom(Direction.ToLower());
    }
}
