using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region SINGLETON

    public static UIManager Instance;

    private void Awake() => Instance = this;

    #endregion

    [SerializeField] private TextMeshProUGUI labelOutput;
    [SerializeField] private ScrollRect scrollOutput;
    [SerializeField] private Image picture;

    public void Log(string line)
    {
        labelOutput.text += line + "\n";
        Canvas.ForceUpdateCanvases();
        UpdateContentSize();
        scrollOutput.verticalNormalizedPosition = 0;
    }

    private void UpdateContentSize()
    {
        float preferredHeight = labelOutput.preferredHeight;
        RectTransform contentRectTransform = labelOutput.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, preferredHeight);
    }

    public void SetPicture(string pictureName)
    {
        if (picture == null) return;
        Sprite pic = Resources.Load<Sprite>("Pictures/" + pictureName);
        picture.sprite = pic;
    }
}
