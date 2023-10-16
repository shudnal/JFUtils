using TMPro;
using UnityEngine.UI;

namespace JFUtils;

public static class RectExtension
{
    public static bool IsOverlapsingOther(this RectTransform a, RectTransform b) { return a.WorldRect().Overlaps(b.WorldRect()); }

    public static bool IsOverlapsingOther(this RectTransform a, RectTransform b, bool allowInverse) => a.WorldRect().Overlaps(b.WorldRect(), allowInverse);

    /// <summary>
    ///     Calculate the world rect of a RectTransform
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <returns></returns>
    public static Rect WorldRect(this RectTransform rectTransform)
    {
        var sizeDelta = rectTransform.sizeDelta;
        var rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        var rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        var position = rectTransform.position;
        return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth,
            rectTransformHeight);
    }
    
    public static GameObject SetToTextHeight(this GameObject go)
    {
        var preferredHeight = go.GetComponentInChildren<TextMeshProUGUI>()?.preferredHeight
                              ?? go.GetComponent<Text>()?.preferredHeight ?? 0;
        go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + 3f);
        return go;
    }

    public static GameObject SetUpperLeft(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(0, 1);
        rect.anchorMin = new(0, 1);
        rect.pivot = new(0, 1);
        rect.anchoredPosition = new(0, 0);
        return go;
    }

    public static GameObject SetMiddleLeft(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(0, 0.5f);
        rect.anchorMin = new(0, 0.5f);
        rect.pivot = new(0, 0.5f);
        rect.anchoredPosition = new(0, 0f);
        return go;
    }

    public static GameObject SetBottomLeft(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(0, 0);
        rect.anchorMin = new(0, 0);
        rect.pivot = new(0, 0);
        rect.anchoredPosition = new(0, 0f);
        return go;
    }

    public static GameObject SetUpperRight(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(1, 1);
        rect.anchorMin = new(1, 1);
        rect.pivot = new(1, 1);
        rect.anchoredPosition = new(0, 0);
        return go;
    }

    public static GameObject SetMiddleRight(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(1, 0.5f);
        rect.anchorMin = new(1, 0.5f);
        rect.pivot = new(1, 0.5f);
        rect.anchoredPosition = new(0, 0f);
        return go;
    }

    public static GameObject SetBottomRight(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(1, 0);
        rect.anchorMin = new(1, 0);
        rect.pivot = new(1f, 0f);
        rect.anchoredPosition = new(0, 0);
        return go;
    }

    public static GameObject SetUpperCenter(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(0.5f, 1f);
        rect.anchorMin = new(0.5f, 1f);
        rect.pivot = new(0.5f, 1f);
        rect.anchoredPosition = new(0, 0);
        return go;
    }

    public static GameObject SetMiddleCenter(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(0.5f, 0.5f);
        rect.anchorMin = new(0.5f, 0.5f);
        rect.pivot = new(0.5f, 0.5f);
        rect.anchoredPosition = new(0, 0);
        return go;
    }

    public static GameObject SetBottomCenter(this GameObject go)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMax = new(0.5f, 0);
        rect.anchorMin = new(0.5f, 0);
        rect.pivot = new(0.5f, 0f);
        rect.anchoredPosition = new(0, 0);
        return go;
    }
}