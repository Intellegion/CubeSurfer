using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static int LEVEL = 1;

    public static int MAX_CHUNKS = 15;

    public static int VELOCITY = 17;
    public static int SENSITIVITY = 7;

    public static bool CONTINUE = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this.gameObject);
    }
}
