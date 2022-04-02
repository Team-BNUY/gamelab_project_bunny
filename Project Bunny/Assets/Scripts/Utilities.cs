using UnityEngine;
using UnityEngine.InputSystem;

public static class Utilities
{
    /// <summary>
    /// Utility function that uses mouse position to return angle between player and on-screen mouse pointer
    /// </summary>
    /// <param name="baseTransform" />
    /// <param name="baseCamera"></param>
    /// <returns></returns>
    public static float MousePosToRotationInput(Transform baseTransform, Camera baseCamera)
    {
        var mousePos = Mouse.current.position.ReadValue();
        if (baseCamera is { })
        {
            var objectPos =  baseCamera.WorldToScreenPoint(baseTransform.position);

            mousePos.x -= objectPos.x;
            mousePos.y -= objectPos.y;
        }

        var angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        return 90 - angle;
    } 
    
    /// <summary>
    /// Set RectTransform Left
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="left"></param>
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }
 
    /// <summary>
    /// Set RectTransform Right
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="right"></param>
    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
 
    /// <summary>
    /// SetRectTransform Top
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="top"></param>
    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }
 
    /// <summary>
    /// SetRectTransform Bottom
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="bottom"></param>
    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
