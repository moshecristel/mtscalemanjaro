using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _volcanoTransform;
    [SerializeField] private Transform _playerTransform;

    [SerializeField] private float _abovePlayerOffset;
    [SerializeField] private float _awayFromVolcanoOffset;

    private float _smoothness = 0;
    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 playerHeightVolcanoPosition = new Vector3(_volcanoTransform.position.x, _playerTransform.position.y, _volcanoTransform.position.z);

        Vector3 normalizedPlayerDirection = (_playerTransform.position - playerHeightVolcanoPosition).normalized;
        Vector3 playerHeightCameraPosition =
            _playerTransform.position + (normalizedPlayerDirection * _awayFromVolcanoOffset);

        Vector3 cameraTarget = new Vector3(playerHeightCameraPosition.x, _playerTransform.transform.position.y + _abovePlayerOffset, playerHeightCameraPosition.z);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, cameraTarget, Time.deltaTime * 0.90f);
        
        _camera.transform.LookAt(_playerTransform);
    }
}
