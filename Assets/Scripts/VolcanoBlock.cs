using System;
using UnityEngine;
using static VolcanoBlockType;

// Assumes a local transform position that is 0 on the y axis
public class VolcanoBlock : MonoBehaviour
{
    private VolcanoBlockType _type;
    public VolcanoBlockType Type
    {
        get => _type;
        set
        {
            _type = value;
            UpdateVisuals();
        }
    }
    
    // The offset to the base origin of this block (normalized = in blocks, not units)
    // Y == LevelIndex
    public Vector3Int OriginNormalizedOffset { get; set; }
    
    // Zero-based Y position starting at ground level (normalized = in blocks, not units)
    public int LevelIndex { get; set; }

    [SerializeField] private MeshRenderer _meshRenderer;

    private void UpdateVisuals()
    {
        // .sharedMaterial effects all materials and this effects only the current instance
        _meshRenderer.material.color = _type switch
        {
            Base => Color.gray,
            Top => Color.green,
            Subtract => Color.red
        };
    }
}
