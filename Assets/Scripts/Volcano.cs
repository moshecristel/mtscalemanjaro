using System;
using System.Collections.Generic;
using UnityEngine;

public class Volcano : MonoBehaviour
{
    [SerializeField] public VolcanoBlock _volcanoBlockPrefab;

    private List<int> levels = new();
    private Vector3 center;
    private float outerRadius;
    private float innerRadius;
    
    void Awake()
    {
        levels = new List<int>() { 14,13,12,11,11,10,10,9,9,9,8,8,8,8,7,7,7,7,7,6,6,6,6,6,6,5,5,5,5,5,4,4,4,4,4,4,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3 };
        
    }

    public void Generate(Vector3 center)
    {
        this.center = center;
        for (int i = 0; i < levels.Count; i++)
            CreateLayer(Vector3.up * i, levels[i], i);
    }

    // zRadius is 
    public void CreateLayer(Vector3 center, int zRadiusInBlocks, int level)
    {
        float apothem = 0.866f;
        float sideLength = apothem * 2f;
        
        // z-axis step is the length of the center point of a hex to a point on the hex and then 1/2 an apothem after that
        // ie. the unit distance between rows of hex blocks on the z-axis
        float zStepLength = 1f + (apothem / 2f);

        // x-axis step is the length of the center to the middle of a side and then to the other middle again
        float xStepLength = 2 * apothem;
        
        // zStepLength = 1.866
        // xStepLength = 1.732
        
        int xRadiusInBlocks = Mathf.CeilToInt((zStepLength / xStepLength) * zRadiusInBlocks + 1);

        outerRadius = zRadiusInBlocks * zStepLength;
        innerRadius = outerRadius - (2f * zStepLength);
        
        // Start at min z -> max z, then min x -> max x
        // Place block and then remove if they intersect a sphere that is far enough within
        // the main circle to allow an outer layer of hex blocks (optimization)

        for (int zBlockIndex = -zRadiusInBlocks; zBlockIndex <= zRadiusInBlocks; zBlockIndex++)
        {
            // Go radius steps in both x directions and place blocks
            for (int xBlockIndex = -xRadiusInBlocks; xBlockIndex <= xRadiusInBlocks; xBlockIndex++)
            {
                float z = center.z + (zBlockIndex * zStepLength);
                float x = center.x + (xBlockIndex * (2 * apothem));

                if (zBlockIndex % 2 != 0)
                {
                    // stagger every odd index
                    x -= apothem;
                }
                
                VolcanoBlock block = PlaceBlock(new Vector3(x, center.y, z), level);
                block.name = "Block_" + zBlockIndex + "_" + xBlockIndex;

                if (!IsOnEdgeOfSphere(block, center, outerRadius, innerRadius))
                {
                    Destroy(block.gameObject);
                }
            }
        }
    }

    public VolcanoBlock PlaceBlock(Vector3 position, int level)
    {
        VolcanoBlock block = Instantiate(_volcanoBlockPrefab, transform);
        block.Type = VolcanoBlockType.Base;
        bool isTop = level == levels.Count - 1 || levels[level + 1] != levels[level];
        if (isTop)
        {
            block.Type = level <= (levels.Count / 2f) ? VolcanoBlockType.TopGrass : VolcanoBlockType.TopBevel;
        }
        block.transform.position = position;
        return block;
    }

    public bool IsOnEdgeOfSphere(VolcanoBlock block, Vector3 center, float outerRadius, float innerRadius)
    {
        return IsWithinDistance(block, center, outerRadius + 0.5f) &&
               !IsWithinDistance(block, center, innerRadius - 0.5f);
    }

    public bool IsWithinDistance(VolcanoBlock block, Vector3 center, float distance)
    {
        return Vector3.Distance(block.transform.position, center) <= distance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, outerRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(center, innerRadius);
    }
}
