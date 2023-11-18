using System;
using UnityEngine;

// TODO temp?
public class GameManager : MonoBehaviour
{
    [SerializeField] private Volcano _volcano;

    private void Start()
    {
        _volcano.Generate(Vector3.zero);
    }
}
