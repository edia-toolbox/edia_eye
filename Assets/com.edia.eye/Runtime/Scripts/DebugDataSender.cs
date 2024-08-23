using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;

namespace Edia.Eye {
    /// <summary>
    /// Dummy class to fake a minimal data stream from an eye tracker (with 90Hz).
    /// </summary>
    public class DebugDataSender : MonoBehaviour {
        public bool IsRunning = false;
        public static ILslTimeAccessible LslTimer;
        public bool UseLslTiming = true;
        [Range(0f, 1f)]
        public float proportionInvalidSamples = 0.1f;
        double _timestampLsl;
        EyeDataPackage _ed = new();
        double _randomWaitValue = 0.15f;
        double _lastTime;

        public void StartAddingDummyEyedata() {
            IsRunning = true;
        }

        private void Start() {

            if (UseLslTiming) {
                // check if the LslTiming component is available
                if (GetComponent<ILslTimeAccessible>() == null) {
                    Debug.LogError("To use LSL timing, the DebugDataSender requires a component on the same GameObject " +
                                   "which implements the ILslTimeAccessible interface (e.g., Edia.Lsl.LslTiming or " +
                                   "Edia.Lsl.EyeOutlet).");
                } else {
                    LslTimer = GetComponent<ILslTimeAccessible>();
                }
            }
        }

		// Sends random data to the eDIA `EyeDataHandler` 
		void Update() {
            if (!IsRunning)
                return;

            if (Time.time > (_lastTime + _randomWaitValue)) {
                // Update fake eye data only after random interval
                _randomWaitValue = UnityEngine.Random.Range(0.01f, 0.5f);
                _lastTime = Time.time;
                _ed.eye = ((Constants.EyeId)(UnityEngine.Random.Range(0, 3))).ToString().ToLower();
                _ed.position_x_local = 0f;
                _ed.position_y_local = 0f;
                _ed.position_z_local = 0f;
                _ed.diameter = UnityEngine.Random.Range(0.02f, 1.0f);
                _ed.rotation_x_local = UnityEngine.Random.Range(-15f, 15f);
                _ed.rotation_y_local = UnityEngine.Random.Range(-60f, 60f);
                _ed.rotation_z_local = 0f;
                Quaternion eyeRot = Quaternion.Euler(_ed.rotation_x_local, _ed.rotation_y_local, 0);
                Vector3 eyeFwd = eyeRot * Vector3.forward;
                _ed.direction_x_local = eyeFwd.x;
                _ed.direction_y_local = eyeFwd.y;
                _ed.direction_z_local = eyeFwd.z;
                _ed.isValid = UnityEngine.Random.Range(0f, 1f) < proportionInvalidSamples ? false : true;  // sometimes send invalid sample
            }

            _ed.timestamp_et = Time.realtimeSinceStartup;

            _timestampLsl = LslTimer != null ? LslTimer.GetLslTime() : 0;
            _ed.timestamp_lsl = _timestampLsl;

            EyeDataHandler.Instance.AddEyeDataPackage(_ed);
        }
    }
}