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

        public bool isStarted = false;

        public static ILslTimer LslTimer;
        public bool UseLslTiming = true;

        public void StartAddingDummyEyedata()
        {
            isStarted = true;
        }

        // Sends random data to the eDIA `EyeDataHandler` with 90Hz. 
        void Update()
        {
            if (!isStarted)
                return;

            double timestampLsl;
            timestampLsl = LslTimer != null ? LslTimer.GetTime() : 0;

            EyeDataPackage ed = new EyeDataPackage();
            ed.eye = "combined";
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
            ed.timestamp_lsl = 0f;

            //        direction_x_local = 0f,
            //        direction_y_local = 0f,
            //        direction_z_local = 0f,
            //        position_x_local = 0f,
            //        position_y_local = 0f,
            //        position_z_local = 0f,
            //        diameter = 0f,
            //        rotation_x_local = 0f,
            //        rotation_y_local = 0f,
            //        rotation_z_local = 0f,
            //        openness = 0f,
            //        timestamp_et = DateTime.UtcNow.Second,
            //        timestamp_lsl = timestampLsl

            //EyeDataHandler.Instance.AddEyeDataPackage(ed);

            //    foreach (string eye in new string[] { "left", "right", "center" })
            //    {

            //        SingleEyeData tmpData;
            //        switch (eye)
            //        {
            //            //case "left":
            //            //    tmpData = eyeData.verbose_data.left;
            //            //    break;
            //            //case "right":
            //            //    tmpData = eyeData.verbose_data.right;
            //            //    break;
            //            case "center":
            //                tmpData = eyeData.verbose_data.combined.eye_data;
            //                break;
            //            default:
            //                tmpData = eyeData.verbose_data.combined.eye_data;
            //                break;
            //        }

            //        double timestampLsl;

            //    if (LslTimer != null)
            //    {
            //        timestampLsl = LslTimer.GetTime();
            //    }
            //    else
            //    {
            //        timestampLsl = 0;
            //    };

            //    ed = new()
            //    {
            //        eye = eye,
            //        direction_x_local = 0f,
            //        direction_y_local = 0f,
            //        direction_z_local = 0f,
            //        position_x_local = 0f,
            //        position_y_local = 0f,
            //        position_z_local = 0f,
            //        diameter = 0f,
            //        rotation_x_local = 0f,
            //        rotation_y_local = 0f,
            //        rotation_z_local = 0f,
            //        openness = 0f,
            //        timestamp_et = DateTime.UtcNow.Second,
            //        timestamp_lsl = timestampLsl
            //    };
            //}
        }
    }
}