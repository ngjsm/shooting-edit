using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunplugin2 : MonoBehaviour
{
    [Header("Hand Tracking (직접 연결)")]
    public Transform rightPalm;
    public Transform rightIndexTip;
    public Transform rightThumbTip;

    [Header("Gun Settings")]
    public Transform gunTransform;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float rotationSmooth = 5f;
    public float shootCooldown = 0.5f;

    [Header("Translation-to-Yaw Settings")]
    [Tooltip("손이 수평으로 1m 이동했을 때 얼마나 회전할지 (°)")]
    public float yawSensitivity = 100f;
    [Tooltip("최대 인식할 손 이동 거리 (m)")]
    public float maxPalmMovement = 0.2f;

    [Header("Pinch Settings")]
    public float pinchThreshold = 0.025f;

    private bool calibrated = false;
    private Vector3 initialPalmPos;
    private float initialYaw;
    private float lastShotTime;
    private bool hasPinched = false;

    void Update()
    {
        if (rightPalm == null || rightIndexTip == null || rightThumbTip == null)
        {
            Debug.LogWarning("Palm, IndexTip 또는 ThumbTip이 연결되지 않았습니다.");
            return;
        }

        Vector3 palmPos = rightPalm.position;

        if (!calibrated)
        {
            initialPalmPos = palmPos;
            initialYaw = gunTransform.rotation.eulerAngles.y;
            calibrated = true;
            return;
        }

        // 총구 회전 처리
        float dx = Mathf.Clamp(palmPos.x - initialPalmPos.x, -maxPalmMovement, maxPalmMovement);
        float targetYaw = initialYaw + dx * yawSensitivity;
        Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, targetRot, rotationSmooth * Time.deltaTime);

        float pinchDistance = Vector3.Distance(rightIndexTip.position, rightThumbTip.position);
        Debug.Log($"[Pinch Check] 거리: {pinchDistance:F3}");

        // 핀치 거리 조건을 만족하면 발사
        if (pinchDistance < pinchThreshold && !hasPinched && Time.time - lastShotTime > shootCooldown)
        {
            Fire();
            hasPinched = true; // 발사된 상태
        }

        // 손가락이 떨어지면 다음 발사 가능
        if (pinchDistance >= pinchThreshold)
        {
            hasPinched = false;
        }
    }

    void Fire()
    {
        Debug.Log("▶ 핀치 인식 → 발사!");
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        lastShotTime = Time.time;
    }
}
