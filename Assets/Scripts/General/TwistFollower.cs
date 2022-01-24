using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class TwistFollower : MonoBehaviour
{
    // Config
    [SerializeField] private bool rotateWithTwist = false;

    [SerializeField] private Transform arcCenter;
    [SerializeField] private float startAngle = 0;

    [SerializeField] private bool useAngleLimits = false;

    [ShowIf("useAngleLimits")] [SerializeField]
    private float minAngle = 0;

    [ShowIf("useAngleLimits")] [SerializeField]
    private float maxAngle = 0;

    [SerializeField] private float verticalRadius = 10;
    [SerializeField] private float horizontalRadius = 10;

    [SerializeField] private float twistScale = 1f;


    private float currentAngle = 0f;
    private float vpWidth => Camera.main.orthographicSize * 2;
    private float vpHeight => Camera.main.orthographicSize * 2 / Camera.main.aspect;

    private void OnEnable()
    {
        currentAngle = startAngle;
        FindObjectOfType<GyroService>().deviceDidRotate += OnDeviceRotate;
    }

    private float Rads(float degrees)
    {
        return degrees / 360f * 2 * Mathf.PI;
    }

    private void OnDeviceRotate(float position, float delta)
    {
        currentAngle += twistScale * delta;
        if (useAngleLimits)
        {
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
        }

        transform.localPosition = PointAtAngle(currentAngle);
        if (rotateWithTwist)
        {
            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    public Vector3 PointAtAngle(float angle)
    {
        return new Vector3(arcCenter.position.x + Mathf.Cos(Rads(angle)) * horizontalRadius,
            arcCenter.position.y + Mathf.Sin(Rads(angle)) * verticalRadius);
    }

    public Vector3 RandomPointOnArc()
    {
        return PointAtAngle(Random.Range(0f, 360f));
    }

    private void OnValidate()
    {
        currentAngle = startAngle;
        OnDeviceRotate(0, 0);
    }
}