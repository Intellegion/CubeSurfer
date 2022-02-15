using System;
using UnityEngine;

// Serialized class to store slime pool positions on chunk
[SerializeField]
public class SlimePool
{
    [Range(5,25)]
    public int RelativeSpawnPositionZ;

    [Range(-1, 1)]
    public int RelativeSpawnPositionX;
}
