using System.Collections.Generic;
using UnityEngine;
using Edia;

namespace Edia.Eye {

    public class EyeDataClientLslPusher : MonoBehaviour, IEyeDataClient {

        private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();
        private float[] _sample;
        private ILslEyeOutlet _eyeOutlet;

        public Edia.Constants.EyeId Eye = Edia.Constants.EyeId.CENTER;

        // Start is called before the first frame update
        void Start() {

            _eyeOutlet = GetComponent<ILslEyeOutlet>();

            if (_eyeOutlet == null) {
                Debug.LogError("EyeDataClientLslPusher requires a EyeOutlet component (edia_lsl) on the same GameObject.");
            }

            if (_eyeOutlet.EyeId.ToString().ToLower() != Eye.ToString().ToLower()) {
                Debug.LogError("EyeDataClientLslPusher: EyeId mismatch between EyeOutlet and EyeDataClientLslPusher");
            }
            
        }

        public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
            receivedEyeDataSamples.Clear();
            foreach (var sample in currentSamples) {
                if (sample.eye.ToLower() == Eye.ToString().ToLower())
                    receivedEyeDataSamples.Add(sample);
            }
        }

        void LateUpdate() {
            PushCurrentSamples();
        }

        void PushCurrentSamples() {
            if (receivedEyeDataSamples.Count == 0)
                return;

            while (receivedEyeDataSamples.Count > 0) {

                float dirX = receivedEyeDataSamples[0].direction_x_local;
                float dirY = receivedEyeDataSamples[0].direction_y_local;
                float dirZ = receivedEyeDataSamples[0].direction_z_local;

                float yaw = Mathf.Atan2(-1 * dirX, dirZ) * Mathf.Rad2Deg;
                float pitch = Mathf.Asin(dirY) * Mathf.Rad2Deg;

                float posX = receivedEyeDataSamples[0].position_x_local;
                float posY = receivedEyeDataSamples[0].position_y_local;
                float posZ = receivedEyeDataSamples[0].position_z_local;

                float rotX = receivedEyeDataSamples[0].rotation_x_local;
                float rotY = receivedEyeDataSamples[0].rotation_y_local;
                float rotZ = receivedEyeDataSamples[0].rotation_z_local;

                float pupilDia = receivedEyeDataSamples[0].diameter;
                float pupilDiaX = receivedEyeDataSamples[0].diameter_x;
                float pupilDiaY = receivedEyeDataSamples[0].diameter_y;

                float confidence = receivedEyeDataSamples[0].confidence;

                double etTime = receivedEyeDataSamples[0].timestamp_et;

                double lslTime = receivedEyeDataSamples[0].timestamp_lsl;

                _eyeOutlet.PushSample(new Vector3(posX, posY, posZ), new Vector3(rotX, rotY, rotZ), pupilDia, confidence, etTime, lslTime);

                receivedEyeDataSamples.RemoveAt(0);
            }
        }
    }
}
