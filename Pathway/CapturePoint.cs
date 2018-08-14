using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If enemy rise this point - player will be defeated.
/// </summary>
public class CapturePoint : MonoBehaviour
{
    private UiManager _uimanager;
    // Enemy already reached capture point
    private bool _objCaught;

    public bool ObjCaught
    {
        get { return _objCaught; } 
        set { _objCaught = value; }
    }

    public UiManager UiManager
    {
        get { return _uimanager; }
        set { _uimanager = value; }
    }

    /// <summary>
    /// Raises the trigger enter2d event.
    /// </summary>
    /// <param name="other">Other.</param>
    /// 
    void OnTriggerEnter2D(Collider2D other)
    {
        // If collision allowed for this scene
        if (LevelManager.IsCollisionValid(gameObject.tag, other.gameObject.tag) == true)
        {
            if (!ObjCaught)
            {
                ObjCaught = true;
                UiManager.Win = true;
                EventManager.TriggerEvent("AllDie", null, null);
                EventManager.TriggerEvent("DisableClick", null, null);
                //EventManager.TriggerEvent("Captured", other.gameObject, null);
            }
        }
    }
}
