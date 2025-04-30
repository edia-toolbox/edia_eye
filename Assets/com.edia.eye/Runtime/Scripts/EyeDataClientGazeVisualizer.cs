using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Edia.Eye {
    public class EyeDataClientGazeVisualizer : EyeDataClient {
#region DECLARATIONS

        [Header("Which Eye?")]
        public Constants.EyeId Eye = Constants.EyeId.CENTER;

        [Header("Settings")]
        [Tooltip("Update the gaze ray only every Xth update.")]
        public int UpdateStep = 50;

        [Tooltip("Hides the ray after X seconds with no new sample.")]
        public float timeoutAfterSecondsWithNoNewSample = 4f;

        private LineRenderer _gazeRayRenderer;
        private int          _lengthOfRay       = 25;
        private float        _gazeOriginOffsetZ = 0.05f;

        private Vector3 _gazeDirection;
        private Vector3 _gazeOriginLocal;
        private int     _counter = 0;

        private Color _colorRay;
        private Color _colorLeft    = Color.green;
        private Color _colorRight   = Color.yellow;
        private Color _colorCenter  = Color.cyan;
        private Color _colorInvalid = Color.red;

        private float _timeLastSample = -1f;

        private List<EyeDataPackage> _receivedEyeDataSamples = new List<EyeDataPackage>();

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region INITS

        private protected override void Awake() {
            base.Awake();
            _gazeRayRenderer = GetComponent<LineRenderer>();
        }

        void Start() {
            this.transform.parent        = XRManager.Instance.XRCam;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;

            _colorRay                           = Eye == Constants.EyeId.CENTER ? _colorCenter : Eye == Constants.EyeId.LEFT ? _colorLeft : _colorRight;
            _gazeRayRenderer.materials[0].color = _colorInvalid;

            _counter = UpdateStep;
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region IEyeDataClient INTERFACE IMPLEMENTATION

        public override void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
            foreach (var sample in currentSamples) {
                if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
                    _receivedEyeDataSamples.Clear();
                    _receivedEyeDataSamples.Add(sample);
                    _timeLastSample = Time.time;
                }
            }
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region PROCESSING SAMPLES

        void Update() {
            _counter--;

            if (_counter < 0)
                UpdateGazeRays();
        }

        void UpdateGazeRays() {
            _counter = UpdateStep;

            if (_receivedEyeDataSamples.Count == 0 | (Time.time - _timeLastSample > timeoutAfterSecondsWithNoNewSample)) {
                UpdateRayPosition(_gazeOriginLocal + Vector3.zero, Vector3.zero);
                return;
            }

            EyeDataPackage validEyeDataPackage = _receivedEyeDataSamples.FirstOrDefault(x => x.isValid); // find first valid package

            // no valid samples, we skip:
            if (validEyeDataPackage == default) {
                _gazeRayRenderer.materials[0].color = Color.red;
                return;
            }

            _gazeOriginLocal = new Vector3(
                validEyeDataPackage.position_x_local,
                validEyeDataPackage.position_y_local,
                validEyeDataPackage.position_z_local
            );

            _gazeDirection = new Vector3(
                validEyeDataPackage.direction_x_local,
                validEyeDataPackage.direction_y_local,
                validEyeDataPackage.direction_z_local
            );

            _gazeRayRenderer.materials[0].color = _colorRay;
            UpdateRayPosition(_gazeOriginLocal + (Vector3.forward * _gazeOriginOffsetZ), _gazeOriginLocal + _gazeDirection * _lengthOfRay);
        }

        private void UpdateRayPosition(Vector3 startPosition, Vector3 endPosition) {
            _gazeRayRenderer.SetPosition(0, startPosition);
            _gazeRayRenderer.SetPosition(1, endPosition);
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
    }
}