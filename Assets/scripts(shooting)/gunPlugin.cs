using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemsImplementation;

public class gunPlugin : MonoBehaviour
{
    XRHandSubsystem handSubsystem;

    [Header("Gun Settings")]
    public Transform gunTransform;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float rotationSmooth = 5f;
    public float shootCooldown = 0.5f;
    [Tooltip("�� cm �̳��� ����-���� ���� ���� �߻�����(���� ����)")]
    public float pinchDistance = 0.02f;

    [Header("Translation-to-Yaw Settings")]
    [Tooltip("���� �������� 1m �̵����� �� �󸶳� ȸ������ (��)")]
    public float yawSensitivity = 100f;
    [Tooltip("�ִ� �ν��� �� �̵� �Ÿ� (m)")]
    public float maxPalmMovement = 0.2f;

    bool calibrated = false;
    Vector3 initialPalmPos;
    float initialYaw;

    float lastShotTime;

    void Start()
    {
        var subs = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(subs);
        if (subs.Count > 0) handSubsystem = subs[0];
        else Debug.LogError("�� XRHandSubsystem�� ã�� ���߽��ϴ�!");
    }

    void Update()
    {
        if (handSubsystem == null) return;

        XRHand right = handSubsystem.rightHand;
        if (!right.isTracked) return;

        Pose palmPose;

        if (!calibrated)
        {
            if (right.GetJoint(XRHandJointID.Palm).TryGetPose(out palmPose))
            {
                initialPalmPos = palmPose.position;
                initialYaw = gunTransform.rotation.eulerAngles.y;
                calibrated = true;
            }

            else if (right.GetJoint(XRHandJointID.Palm).TryGetPose(out palmPose))
            {
                float dx = Mathf.Clamp(palmPose.position.x - initialPalmPos.x,
                                       -maxPalmMovement,
                                        maxPalmMovement);
                float targetYaw = initialYaw + dx * yawSensitivity;
                Quaternion targetRot = Quaternion.Euler(0f, targetYaw, 0f);

                gunTransform.rotation = Quaternion.Slerp(
                    gunTransform.rotation,
                    targetRot,
                    rotationSmooth * Time.deltaTime
                );
            }

            if (right.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumb) &&
                 right.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose idx))
            {
                float dist = Vector3.Distance(thumb.position, idx.position);
                Debug.Log($"PinchDist = {dist:F3} m");

                if (dist <= pinchDistance && Time.time - lastShotTime > shootCooldown)
                {
                    Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                    lastShotTime = Time.time;
                }
            }
        }
    }
}