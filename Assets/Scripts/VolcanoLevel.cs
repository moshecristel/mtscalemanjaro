using System.Collections.Generic;
using UnityEngine;

public class VolcanoLevel
{
    public int RadiusInBlocks { get; set; }
    public int Dims => RadiusInBlocks * 2;
    
    // Key is block's OriginNormalizedOffset, but with the Y at Ground Level
    public Dictionary<Vector3Int, VolcanoBlock> GroundLevelOffsetToBlock { get; set; } = new();
}
