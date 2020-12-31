using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    public static float CheckDelay = 0.5f;        // How long to wait until we check again.

    static Orientation orientation;        // Current Device Orientation
    static bool isAlive = true;                    // Keep this script running?

    void Start()
    {
        orientation = GetOrientation();
        Debug.Log("Start: " + orientation);
        StartCoroutine(CheckForChange());
    }

    IEnumerator CheckForChange()
    {

        while (isAlive)
        {
            Debug.Log("Cecking: " + orientation);
            if (orientation != GetOrientation())
            {
                orientation = GetOrientation();
                Debug.Log("Changed: " + orientation);
            }

            yield return new WaitForSeconds(CheckDelay);
        }
    }

    Orientation GetOrientation()
    {
        if (Screen.width > Screen.height)
        {
            return Orientation.Landscape;
        }
        return Orientation.Potrait;
    }

    void OnDestroy()
    {
        isAlive = false;
    }

    public enum Orientation
    {
        Potrait,
        Landscape
    }
}
