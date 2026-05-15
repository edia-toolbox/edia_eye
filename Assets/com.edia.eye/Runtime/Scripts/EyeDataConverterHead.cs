using UnityEngine;

namespace Edia.Eye {
    /// <summary> Dummy class to fake a minimal data stream from an eye tracker (with 90Hz). </summary>
    [EdiaHeader("EDIA EYE", "Proxy center eye gaze", "Generates center eye data based on headset orientation (NO REAL EYE TRACKING DATA!).")]
    public class EyeDataConverterHead : MonoBehaviour {

        [Header("Refs")]
        public bool IsRunning = true;

        public static ILslTimeAccessible LslTimer;
        public        bool               UseLslTiming = false;

        // Locals
        private double         _timestampLsl;
        private EyeDataPackage _ed = new();
        private double         _lastTime;

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
                }
                else {
                    LslTimer = GetComponent<ILslTimeAccessible>();
                }
            }
        }

        // Sends "looking straight ahead" (relative to head) data to the `EyeDataHandler` 
        private void Update() {
            if (!IsRunning)
                return;

            _ed.eye              = nameof(Constants.EyeId.CENTER).ToLower();
            _ed.position_x_local = 0f;
            _ed.position_y_local = 0f;
            _ed.position_z_local = 0f;
            _ed.diameter         = 0f;
            _ed.rotation_x_local = 0f;
            _ed.rotation_y_local = 0f;
            _ed.rotation_z_local = 0f;
            _ed.direction_x_local = 0f;
            _ed.direction_y_local = 0f;
            _ed.direction_z_local = 1f;
            _ed.openness          = 0f;
            _ed.isValid           = true;

            _ed.timestamp_et = Time.realtimeSinceStartup;

            _timestampLsl     = LslTimer != null ? LslTimer.GetLslTime() : 0;
            _ed.timestamp_lsl = _timestampLsl;

            EyeDataHandler.Instance.AddEyeDataPackage(_ed);
        }
    }
}