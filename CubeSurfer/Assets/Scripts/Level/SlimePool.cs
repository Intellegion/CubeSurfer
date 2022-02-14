using System;
using UnityEngine;

[SerializeField]
public class SlimePool
{
    public GameObject SlimeObject;

    [Range(5,25)]
    public int RelativeSpawnPositionZ;

    [Range(-1, 1)]
    public int RelativeSpawnPositionX;
}
