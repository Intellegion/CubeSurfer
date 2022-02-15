using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static variables to retain variable values across scene changes
public class Constants : MonoBehaviour
{
    public static int LEVEL = 1;

    public static int MAX_CHUNKS = 15;

    public static int VELOCITY = 17;
    public static float SENSITIVITY = 0.5f;

    public static bool CONTINUE = false;

    void Start()
    {
        // Do not destroy object on scene change
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
    }
}
