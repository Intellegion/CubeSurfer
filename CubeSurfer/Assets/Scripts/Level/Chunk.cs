using System;
using UnityEngine;

public enum ChunkType
{
    Basic,
    Obstacle,
    Slime
}

public enum Direction
{
    Straight,
    Left,
    Right
}

[Serializable]
public class Chunk 
{
    public ChunkType Type;

    [Range(0, 5)]
    public int ObstacleCount;

    [Range(0, 3)]
    public int SlimeCount;

    public bool ShouldTurn;

    public Direction TurnDirection;

    public static Chunk GetBasicChunk(Chunk chunk)
    {
        chunk.Type = ChunkType.Basic;
        return chunk;
    }

    public static Chunk GetObstacleChunk(Chunk chunk)
    {
        chunk.Type = ChunkType.Obstacle;
        return chunk;
    }

    public static Chunk GetSlimeChunk(Chunk chunk)
    {
        chunk.Type = ChunkType.Obstacle;
        return chunk;
    }
}
