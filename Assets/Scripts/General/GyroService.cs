//Attach this script to a GameObject in your Scene.

using System;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

public class GyroService : MonoBehaviour
{
    Gyroscope m_Gyro;
    private Queue<Vector3> smoothingKernel = new Queue<Vector3>();
    public UltEvent<float, float> deviceDidRotate; 

    void Start()
    {
        //Set up and enable the gyroscope (check your device has one)
        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;
    }


    private float pastResultTwist;
    private float pastResultVTwist;

    private void Update()
    {
        #if UNITY_EDITOR
        // Virtual twist
        var vTwist = RoundToNearest(Input.mousePosition.x / (float)Screen.width * 720f, 5);
        if (pastResultVTwist != null && pastResultVTwist != vTwist)
        {
            var delta = pastResultVTwist - vTwist;
            deviceDidRotate?.Invoke(vTwist, delta);
        }
        pastResultVTwist = vTwist;
        #endif
        if (pastResultTwist == null) return;
        var twist = RoundToNearest(m_Gyro.attitude.eulerAngles.z, 5);
        if (pastResultTwist != twist)
        {
            var delta = pastResultTwist - twist;
            deviceDidRotate?.Invoke(twist, delta);
        }
        pastResultTwist = twist;
    }

    int RoundToNearest(float n, int rounder)
    {
        return (int) (Mathf.Floor(n / rounder) * rounder);
    }

    // private void DeriveSmoothedEulers()
    // {
    //     var newSmoothedResult = Vector3.zero;
    //     smoothingKernel.Enqueue(m_Gyro.attitude.eulerAngles);
    //     if (smoothingKernel.Count > 5)
    //     {
    //         smoothingKernel.Dequeue();
    //         foreach (var pastEuler in smoothingKernel)
    //         {
    //             newSmoothedResult += pastEuler;
    //         }
    //
    //         newSmoothedResult /= 5f;
    //         newSmoothedResult = new Vector3(RoundToNearest(newSmoothedResult.x, 5),
    //             RoundToNearest(newSmoothedResult.y, 5), RoundToNearest(newSmoothedResult.z, 5));
    //     }
    //
    //     smoothedResult = newSmoothedResult;
    // }


    //This is a legacy function, check out the UI section for other ways to create your UI
    // void OnGUI()
    // {
    //     //Output the rotation rate, attitude and the enabled state of the gyroscope as a Label
    //     // GUI.Label(new Rect(0, 00, 200, 40), "Gyro rotation rate " + m_Gyro.rotationRate);
    //     GUI.Label(new Rect(0, 50, 200, 40), "Gyro attitude" + smoothedResult);
    //     // GUI.Label(new Rect(0, 150, 200, 40), "Gyro enabled : " + m_Gyro.enabled);
    // }
}