using System;
using UnityEngine;

// TODO temp?
public class GameManager : MonoBehaviour
{
    [SerializeField] private Volcano _volcano;
    
    [SerializeField] private Vector3 _groundCenter;
    [SerializeField] private int _baseRadiusInBlocks;
    [SerializeField] private int _mouthRadiusInBlocks;
    [SerializeField] private int _heightInBlocks;
    
    private void Start()
    {
        _volcano.Generate(_groundCenter, _baseRadiusInBlocks, _mouthRadiusInBlocks, _heightInBlocks);

        for (int i = 0; i < 300; i++)
        {
            _volcano.ApplyNoise();
        }
    }
}
