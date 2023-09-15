using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps the picture scaled to the correct size
/// </summary>
public class PictureScaler : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();  
    }

    private void Update()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentRectTransform.rect.height);
        float height = parentRectTransform.rect.height;
        rectTransform.sizeDelta = new Vector2(height, height);
    }

}
