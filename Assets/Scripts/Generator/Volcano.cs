using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Volcano : MonoBehaviour
{
    [SerializeField] private VolcanoBlock _volcanoBlockPrefab;

    public VolcanoLevel[] Levels { get; set; }

    private Vector3 _groundCenter;
    private int _baseRadiusInBlocks;
    private int _mouthRadiusInBlocks;
    private int _heightInBlocks;
    private bool _isGenerated;

    private float _blockDimension;
    
    void Awake()
    {
        BoxCollider blockCollider = _volcanoBlockPrefab.GetComponent<BoxCollider>();
        _blockDimension = blockCollider.size.x;
    }

    public void Generate(Vector3 groundCenter, int baseRadiusInBlocks, int mouthRadiusInBlocks, int heightInBlocks)
    {
        _groundCenter = groundCenter;
        _baseRadiusInBlocks = baseRadiusInBlocks;
        _mouthRadiusInBlocks = mouthRadiusInBlocks;
        _heightInBlocks = heightInBlocks;

        // Calculate the contraction in blocks from base to mouth
        // Distribute the theoretical contraction to various levels (which effect that level and all higher levels) - prefer levels that haven't gotten contractions yet
        // Create a "perfect" volcano given the contraction levels
        // Do a round of adding and subtracting from every level (starting on bottom) given these constraints:
        //  - Blocks must be supported by another block 

        int contractionInBlocks = baseRadiusInBlocks - mouthRadiusInBlocks;

        int[] contractions = new int[heightInBlocks];
        int contractionsRemaining = contractionInBlocks;
        while (contractionsRemaining > 0)
        {
            // Randomly pick a level to add a contraction to (not first) 
            contractions[Random.Range(1, heightInBlocks)]++;
            contractionsRemaining--;
        }
        
        int currRadius = baseRadiusInBlocks;
        Levels = new VolcanoLevel[heightInBlocks];
        for (int i = 0; i < Levels.Length; i++)
        {
            currRadius -= contractions[i];
            VolcanoLevel currLevel = new VolcanoLevel();
            currLevel.RadiusInBlocks = currRadius;
            
            // Iterate over square where dimension == currRadius and create blocks for this level
            for (int xOffsetNormalized = -currRadius; xOffsetNormalized <= currRadius; xOffsetNormalized++)
            {
                for (int zOffsetNormalized = -currRadius; zOffsetNormalized <= currRadius; zOffsetNormalized++)
                {
                    float xOffset = xOffsetNormalized * _blockDimension;
                    float zOffset = zOffsetNormalized * _blockDimension;

                    float normalizedDistance = Vector3.Distance(groundCenter, new Vector3(xOffset, groundCenter.y, zOffset));
                    if (normalizedDistance <= currRadius)
                    {
                        // Effectively excludes the corners of the square dimension and makes a rough/quantized "circle" of about currRadius
                        VolcanoBlock block = Instantiate(_volcanoBlockPrefab, transform);
                        block.transform.position = new Vector3(xOffset, i * _blockDimension, zOffset);
                        block.LevelIndex = i;
                        block.OriginNormalizedOffset = new Vector3Int(xOffsetNormalized, i, zOffsetNormalized);
                        currLevel.GroundLevelOffsetToBlock[
                            new Vector3Int(xOffsetNormalized, (int)groundCenter.y, zOffsetNormalized)] = block;
                    }
                }
            }

            Levels[i] = currLevel;
            _isGenerated = true;
        }
    }

    public void ApplyNoise(bool isSubtract = true)
    {
        // Weighted random selection (don't want to be removing disproportionately from the top)
        int randomBlockCutoff = Random.Range(0, GetBlockCount());
        int blocksSoFar = 0;
        int randomLevelIndex = 0;
        for(int i = 0; i < Levels.Length - 1; i++)
        {
            VolcanoLevel currLevel = Levels[i];
            blocksSoFar += currLevel.GroundLevelOffsetToBlock.Count;
            if (blocksSoFar > randomBlockCutoff)
            {
                randomLevelIndex = i;
                break;
            }
        }

        VolcanoLevel levelToApplyNoiseTo = Levels[randomLevelIndex];
        VolcanoLevel levelAbove = Levels[randomLevelIndex + 1];

        List<VolcanoBlock> candidateBlocks = new();
        
        foreach (VolcanoBlock block in levelToApplyNoiseTo.GroundLevelOffsetToBlock.Values)
        {
            Vector3Int aboveKey = new Vector3Int(block.OriginNormalizedOffset.x, 0, block.OriginNormalizedOffset.z);
            bool noBlockAbove = !levelAbove.GroundLevelOffsetToBlock.ContainsKey(aboveKey);

            bool allSidesHaveBlocks =
                levelToApplyNoiseTo.GroundLevelOffsetToBlock.ContainsKey(aboveKey + Vector3Int.back) &&
                levelToApplyNoiseTo.GroundLevelOffsetToBlock.ContainsKey(aboveKey + Vector3Int.forward) &&
                levelToApplyNoiseTo.GroundLevelOffsetToBlock.ContainsKey(aboveKey + Vector3Int.left) &&
                levelToApplyNoiseTo.GroundLevelOffsetToBlock.ContainsKey(aboveKey + Vector3Int.right);

            if(noBlockAbove && !allSidesHaveBlocks){
                // No block above
                candidateBlocks.Add(block);
            }   
        }

        if (candidateBlocks.Count == 0) return;
        
        VolcanoBlock selectedBlock = candidateBlocks[Random.Range(0, candidateBlocks.Count)];
        if (isSubtract)
        {
            levelToApplyNoiseTo.GroundLevelOffsetToBlock.Remove(new Vector3Int(selectedBlock.OriginNormalizedOffset.x,
                0, selectedBlock.OriginNormalizedOffset.z));
            Destroy(selectedBlock.gameObject);
            // selectedBlock.Type = VolcanoBlockType.Subtract;
        }
    }

    private int GetBlockCount()
    {
        int blockCount = 0;
        foreach (VolcanoLevel level in Levels)
        {
            blockCount += level.GroundLevelOffsetToBlock.Values.Count;
        }

        return blockCount;
    }


    private void OnDrawGizmos()
    {
        if(Application.isPlaying && _isGenerated)
        {
            // Draw green base circle
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_groundCenter, _baseRadiusInBlocks);

            // Draw red mouth circle
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCenter + (Vector3.up * _heightInBlocks * _blockDimension), _mouthRadiusInBlocks);

            // Draw black vertical line
            Gizmos.color = Color.black;
            Vector3 topPoint = _groundCenter + new Vector3(0, _heightInBlocks * _blockDimension, 0);
            Gizmos.DrawLine(_groundCenter, topPoint);
        }
    }
}
