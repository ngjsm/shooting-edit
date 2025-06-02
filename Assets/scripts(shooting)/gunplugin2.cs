using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunplugin2 : MonoBehaviour
{
    [Header("Hand Tracking (���� ����)")]
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
    [Tooltip("���� �������� 1m �̵����� �� �󸶳� ȸ������ (��)")]
    public float yawSensitivity = 100f;
    [Tooltip("�ִ� �ν��� �� �̵� �Ÿ� (m)")]
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
            Debug.LogWarning("Palm, IndexTip �Ǵ� ThumbTip�� ������� �ʾҽ��ϴ�.");
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

        // �ѱ� ȸ�� ó��
        float dx = Mathf.Clamp(palmPos.x - initialPalmPos.x, -maxPalmMovement, maxPalmMovement);
        float targetYaw = initialYaw + dx * yawSensitivity;
        Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, targetRot, rotationSmooth * Time.deltaTime);

        float pinchDistance = Vector3.Distance(rightIndexTip.position, rightThumbTip.position);
        Debug.Log($"[Pinch Check] �Ÿ�: {pinchDistance:F3}");

        // ��ġ �Ÿ� ������ �����ϸ� �߻�
        if (pinchDistance < pinchThreshold && !hasPinched && Time.time - lastShotTime > shootCooldown)
        {
            Fire();
            hasPinched = true; // �߻�� ����
        }

        // �հ����� �������� ���� �߻� ����
        if (pinchDistance >= pinchThreshold)
        {
            hasPinched = false;
        }
    }

    void Fire()
    {
        Debug.Log("�� ��ġ �ν� �� �߻�!");
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        lastShotTime = Time.time;
    }
}
