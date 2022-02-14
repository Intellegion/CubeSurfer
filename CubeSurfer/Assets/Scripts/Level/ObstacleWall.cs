using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class ObstacleWall
{
    public int Type;

    [Range(5, 25)]
    public int SpawnPosition;
}
