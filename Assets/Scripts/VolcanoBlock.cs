using System;
using UnityEngine;

public class VolcanoBlock : MonoBehaviour
{
    [SerializeField] private GameObject _topGrassMesh;
    [SerializeField] private GameObject _topBevelMesh;
    [SerializeField] private GameObject _baseMesh;
    
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

    private void UpdateVisuals()
    {
        _topGrassMesh.gameObject.SetActive(false);
        _topBevelMesh.gameObject.SetActive(false);
        _baseMesh.gameObject.SetActive(false);
        
        GameObject meshObj = _type switch
        {
            VolcanoBlockType.Base => _baseMesh,
            VolcanoBlockType.TopBevel => _topBevelMesh,
            VolcanoBlockType.TopGrass => _topGrassMesh,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        meshObj.gameObject.SetActive(true);
    }
}
