using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static int LEVEL = 1;

    public static int MAX_CHUNKS = 15;

    public static int VELOCITY = 17;
    public static float SENSITIVITY = 0.5f;

    public static bool CONTINUE = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
    }
}
