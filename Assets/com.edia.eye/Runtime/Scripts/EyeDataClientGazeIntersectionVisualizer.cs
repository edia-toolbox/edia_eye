using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

namespace Edia.Eye {
    [EdiaHeader("EDIA EYE", "EyeData Debug helper", "Trail showing gaze intersection on objects on layer 'GazeCollison'")]
    public class EyeDataClientGazeIntersectionVisualizer : EyeDataClient {
#region DECLARATIONS

        [Header("Which Eye?")]
        public Constants.EyeId Eye = Constants.EyeId.CENTER;

        [Header("Settings")]
        [Tooltip("Update only every Xth update.")]
        public int UpdateStep = 2;

        [Header("Callback")]
        public UnityEvent<Vector3> OnValidGazeIntersection;

        // locals
        private int                  _counter                = 0;
        private float                _timeLastSample         = -1f;
        private List<EyeDataPackage> _receivedEyeDataSamples = new();

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region INITS

        void Start() {
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
                ValidateSample();
        }

        void ValidateSample() {
            _counter = UpdateStep;

            EyeDataPackage validEyeDataPackage = _receivedEyeDataSamples.FirstOrDefault(x => x.isValid); // find first valid package

            if (validEyeDataPackage == default)
                return;

            OnValidGazeIntersection?.Invoke(new Vector3(
                validEyeDataPackage.intersection_x,
                validEyeDataPackage.intersection_y,
                validEyeDataPackage.intersection_z)
            );
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
    }
}