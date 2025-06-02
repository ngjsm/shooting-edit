using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewgunPlugin : MonoBehaviour
{
    [Header("Hand Tracking (직접 연결)")]
    public Transform rightPalm;
    public Transform rightIndexTip;

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

    public float fistThreshold = 0.03f;
    public float requiredHoldDuration = 0.1f;

    private bool calibrated = false;
    private Vector3 initialPalmPos;
    private float initialYaw;
    private float lastShotTime;

    private bool isFist = false;
    private float fistHoldTime = 0f;

    void Update()
    {
        if (rightPalm == null || rightIndexTip == null)
        {
            Debug.LogWarning("Palm 또는 IndexTip이 연결되지 않았습니다.");
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

        // 총구 회전
        float dx = Mathf.Clamp(palmPos.x - initialPalmPos.x, -maxPalmMovement, maxPalmMovement);
        float targetYaw = initialYaw + dx * yawSensitivity;
        Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, targetRot, rotationSmooth * Time.deltaTime);

        // 주먹 상태 판정
        isFist = IsFist();

        if (isFist)
        {
            fistHoldTime += Time.deltaTime;

            if (fistHoldTime >= requiredHoldDuration && Time.time - lastShotTime > shootCooldown)
            {
                Debug.Log("▶ 일정 시간 주먹 유지 → 발사!");
                Fire();
                fistHoldTime = 0f; // 다음 발사를 위해 초기화
            }
        }
        else
        {
            fistHoldTime = 0f; // 손을 펴면 시간 초기화
        }
    }

    void Fire()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        lastShotTime = Time.time;
    }

    bool IsFist()
    {
        float indexDist = Vector3.Distance(rightIndexTip.position, rightPalm.position);
        Debug.Log($"[Fist Check] indexDist = {indexDist:F3}");
        return indexDist < fistThreshold;
    }
}