using UnityEngine;

namespace Edia.Eye {
    /// <summary> Dummy class to fake a minimal data stream from an eye tracker (with 90Hz). </summary>
    public class DebugDataSender : MonoBehaviour {
        
        [Header("Refs")]
        public bool IsRunning = false;
        public static ILslTimeAccessible LslTimer;
        public bool UseLslTiming = true;
        [Range(0f, 1f)]
        public float ProportionInvalidSamples = 0.1f;

        // Locals
        private double _timestampLsl;
        private EyeDataPackage _ed = new();
        private double _randomWaitValue = 0.15f;
        private double _lastTime;

        /// <summary> Start the dummy data provider from script </summary>
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
        private void Update() {
            if (!IsRunning)
                return;

            if (Time.time > (_lastTime + _randomWaitValue)) { // Update fake eye data only after random interval
                _randomWaitValue = UnityEngine.Random.Range(0.01f, 0.5f);
                _lastTime = Time.time;
                _ed.eye = ((Constants.EyeId)(UnityEngine.Random.Range(0, 3))).ToString().ToLower();
                _ed.position_x_local = _ed.eye == "left" ? -0.03f : _ed.eye == "right" ? 0.03f : 0f;
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
                _ed.openness = UnityEngine.Random.Range(0f, 1f) < 0.05f ? 0 : UnityEngine.Random.Range(0.8f, 1.0f);
                _ed.isValid = UnityEngine.Random.Range(0f, 1f) < ProportionInvalidSamples ? false : true;  // sometimes send invalid sample
            }

            _ed.timestamp_et = Time.realtimeSinceStartup;

            _timestampLsl = LslTimer != null ? LslTimer.GetLslTime() : 0;
            _ed.timestamp_lsl = _timestampLsl;

            EyeDataHandler.Instance.AddEyeDataPackage(_ed);
        }
    }
}