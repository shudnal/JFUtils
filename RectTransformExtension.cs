using UnityEngine;

namespace Extensions;

public static class RectTransformExtension
{
    /// <summary>
    ///     Test if a RectTransform overlaps another RectTransform in world space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Overlaps(this RectTransform a, RectTransform b) { return a.WorldRect().Overlaps(b.WorldRect()); }

    /// <summary>
    ///     Test if a RectTransform overlaps another RectTransform in world space
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="allowInverse"></param>
    /// <returns></returns>
    public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
    {
        return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
    }

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
}