using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static int LEVEL;

    public static int VELOCITY = 17;
    public static int SENSITIVITY = 7;

    public static bool CONTINUE = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
