using System;
using UnityEngine;

public enum ChunkType
{
    Basic, // Plain path with only collectibles
    Obstacle, // Path with obstacle
    Slime // Path with slime pools
}

public enum Direction
{
    Straight,
    Left,
    Right
}

// Each chunk is a block in the level with obstacles or slime pools or collectibles based on the type
[Serializable]
public class Chunk 
{
    public ChunkType Type;

    public ObstacleWall Obstacle;

    [Range(0, 3)]
    public SlimePool[] SlimePools;

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
        chunk.Type = ChunkType.Slime;
        return chunk;
    }
}
