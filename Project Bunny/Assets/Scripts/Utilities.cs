using UnityEngine;
using UnityEngine.InputSystem;

public class Utilities : MonoBehaviour
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
}
