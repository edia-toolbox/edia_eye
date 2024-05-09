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

        public void StartAddingDummyEyedata() {
            isStarted = true;
        }

        // Sends random data to the eDIA `EyeDataHandler` with 90Hz. 
        void Update()
        {
            if (!isStarted)
                return;

            EyeDataPackage ed = new EyeDataPackage();
            ed.eye = "combined";
            ed.timestamp_et = 123f;
            ed.position_x_local = 0f;
            ed.position_y_local = 0f;
            ed.position_z_local = -0.02f; // Offset eyes to lens
            ed.direction_x_local = Random.Range(-0.2f, 0.2f);
            ed.direction_y_local = Random.Range(-0.2f, 0.2f);
            ed.direction_z_local = Random.Range(0.8f, 1f);

            EyeDataHandler.Instance.AddEyeDataPackage(ed);
        }
    }
}