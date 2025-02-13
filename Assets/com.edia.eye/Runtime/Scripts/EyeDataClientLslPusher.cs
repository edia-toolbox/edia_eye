using System.Collections.Generic;
using UnityEngine;

namespace Edia.Eye {
    
    public class EyeDataClientLslPusher : MonoBehaviour, IEyeDataClient {
        private List<EyeDataPackage> _receivedEyeDataSamples = new List<EyeDataPackage>();
        private float[] _sample;
        private ILslEyeOutlet _eyeOutlet;

        public Edia.Constants.EyeId Eye = Edia.Constants.EyeId.CENTER;

        // Start is called before the first frame update
        void Start() {
            _eyeOutlet = GetComponent<ILslEyeOutlet>();

            if (_eyeOutlet == null) {
                Debug.LogError("EyeDataClientLslPusher requires a EyeOutlet component (edia_lsl) on the same GameObject.");
                return;
            }

            if (_eyeOutlet.EyeId.ToString().ToLower() != Eye.ToString().ToLower()) {
                Debug.LogError("EyeDataClientLslPusher: EyeId mismatch between EyeOutlet and EyeDataClientLslPusher");
            }
        }

        public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
            _receivedEyeDataSamples.Clear();
            foreach (var sample in currentSamples) {
                if (sample.eye.ToLower() == Eye.ToString().ToLower())
                    _receivedEyeDataSamples.Add(sample);
            }
        }

        void LateUpdate() {
            PushCurrentSamples();
        }

        void PushCurrentSamples() {
            if (_receivedEyeDataSamples.Count == 0)
                return;

            while (_receivedEyeDataSamples.Count > 0) {
                float dirX = _receivedEyeDataSamples[0].direction_x_local;
                float dirY = _receivedEyeDataSamples[0].direction_y_local;
                float dirZ = _receivedEyeDataSamples[0].direction_z_local;

                float yaw = Mathf.Atan2(-1 * dirX, dirZ) * Mathf.Rad2Deg;
                float pitch = Mathf.Asin(dirY) * Mathf.Rad2Deg;

                float posX = _receivedEyeDataSamples[0].position_x_local;
                float posY = _receivedEyeDataSamples[0].position_y_local;
                float posZ = _receivedEyeDataSamples[0].position_z_local;

                float rotX = _receivedEyeDataSamples[0].rotation_x_local;
                float rotY = _receivedEyeDataSamples[0].rotation_y_local;
                float rotZ = _receivedEyeDataSamples[0].rotation_z_local;

                float pupilDia = _receivedEyeDataSamples[0].diameter;
                float pupilDiaX = _receivedEyeDataSamples[0].diameter_x;
                float pupilDiaY = _receivedEyeDataSamples[0].diameter_y;

                float openness = _receivedEyeDataSamples[0].openness;

                float confidence = _receivedEyeDataSamples[0].confidence;

                double etTime = _receivedEyeDataSamples[0].timestamp_et;

                double lslTime = _receivedEyeDataSamples[0].timestamp_lsl;

                _eyeOutlet.PushSample(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), pupilDia, openness, confidence, etTime, lslTime);

                _receivedEyeDataSamples.RemoveAt(0);
            }
        }
    }
}