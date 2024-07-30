using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float orthographicSize;
    private float targetOrthographicSize;
    [SerializeField] private Slider zoomSlider;

    private void Start()
    {
        orthographicSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        targetOrthographicSize = orthographicSize;
    }

    private void Update()
    {
        HandleMovement();
        // HandleZoom();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        float moveSpeed = 30f;
        Vector3 moveDir = new Vector3(x, y).normalized;

        transform.position += moveDir * (moveSpeed * Time.deltaTime);
    }

    public void HandleZoom(int zoomAmount)
    {
        float zoomMultiplier = 5f; // previously 2f

        // targetOrthographicSize -= Input.mouseScrollDelta.y * zoomAmount;
        // targetOrthographicSize -= zoomSlider.value * zoomMultiplier;
        targetOrthographicSize -= zoomAmount * zoomMultiplier;

        float minOrthographicSize = 10;
        float maxOrthographicSize = 30;

        targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);

        float zoomSpeed = 1f;

        // orthographicSize = Mathf.Lerp(orthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);

        DOTween.To(() => cinemachineVirtualCamera.m_Lens.OrthographicSize, x => cinemachineVirtualCamera.m_Lens.OrthographicSize = x, targetOrthographicSize, zoomSpeed);
        // orthographicSize = Mathf.Lerp(orthographicSize, targetOrthographicSize, 2);
        // cinemachineVirtualCamera.m_Lens.OrthographicSize = orthographicSize;
    }
}