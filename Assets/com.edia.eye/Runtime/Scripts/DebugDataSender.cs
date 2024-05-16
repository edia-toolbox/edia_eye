using Edia.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edia.Eye
{
    /// <summary>
    /// Dummy class to fake a minimal data stream from an eye tracker (with 90Hz).
    /// </summary>
    public class DebugDataSender : MonoBehaviour
    {
        public bool IsRunning = false;
        public static ILslTimer LslTimer;
        public bool UseLslTiming = true;
        double timestampLsl;
        EyeDataPackage ed;
        double randomWaitValue = 0.3f;
        double lastTime;

        public void StartAddingDummyEyedata()
        {
            IsRunning = true;
        }

        // Sends random data to the eDIA `EyeDataHandler` 
        void Update()
        {
            if (!IsRunning)
                return;

            if (Time.time < (lastTime + randomWaitValue))
                return;

            ed = new();

            ed.eye = Edia.Constants.EyeId.CENTER.ToString().ToLower();
            ed.direction_x_local = UnityEngine.Random.Range(-0.2f, 0.2f);
            ed.direction_y_local = UnityEngine.Random.Range(-0.2f, 0.2f);
            ed.direction_z_local = UnityEngine.Random.Range(0.8f, 1f);
            ed.position_x_local = 0f;
            ed.position_y_local = 0f;
            ed.position_z_local = -0.02f; // Offset eyes to lens
            ed.diameter = UnityEngine.Random.Range(0.02f, 1.0f);
            ed.rotation_x_local = UnityEngine.Random.Range(-15f, 15f);
            ed.rotation_y_local = UnityEngine.Random.Range(-60f, 60f);
            ed.rotation_z_local = 0f;
            ed.timestamp_et = 123f;

            timestampLsl = LslTimer != null ? LslTimer.GetTime() : 0;
            ed.timestamp_lsl = timestampLsl;

            EyeDataHandler.Instance.AddEyeDataPackage(ed);

            randomWaitValue = UnityEngine.Random.Range(0.01f, 1.0f);
            lastTime = Time.time;
        }
    }
}