using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scriptable object that stores level data
[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public Chunk[] Chunks;
}
