using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;

namespace Edia.Eye
{
    public class EyeDataClientGazeVisualizer : MonoBehaviour, IEyeDataClient
    {
#region DECLARATIONS 

		[Header("Which Eye?")]
		public Constants.EyeId Eye = Constants.EyeId.CENTER;

		LineRenderer gazeRayRenderer;
		int lengthOfRay = 25;
		float gazeOriginOffsetZ = 0.05f;

        [Header("Settings")]
        [Tooltip("Update the gaze ray only every Xth update.")]
        public int UpdateStep = 50;

        Vector3 gazeDirection;
        Vector3 gazeOriginLocal;
        int counter = 0;

        Color colorRay;
        Color colorLeft = Color.green;
        Color colorRight = Color.yellow;
        Color colorCenter = Color.cyan;
        Color colorInvalid = Color.red;

        private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region INITS	
		private void Awake() {
			gazeRayRenderer = GetComponent<LineRenderer>();
		}

		void Start()
        {
            this.transform.parent = XRManager.Instance.XRCam;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;

            colorRay = Eye == Constants.EyeId.CENTER ? colorCenter : Eye == Constants.EyeId.LEFT ? colorLeft : colorRight;
            gazeRayRenderer.materials[0].color = colorRay;

            counter = UpdateStep;
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region IEyeDataClient INTERFACE IMPLEMENTATION 

		public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
			receivedEyeDataSamples.Clear();
            foreach (var sample in currentSamples) {
                if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
                    receivedEyeDataSamples.Add(sample);
                }
            }
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region PROCESSING SAMPLES

        void Update()
        {
            counter--;

            if (counter < 0)
                UpdateGazeRays();
        }

        void UpdateGazeRays()
        {
            counter = UpdateStep;

            if (receivedEyeDataSamples.Count == 0)
                return;

            // Grab the first valid sample since last ray update:
            int sampleIdx = -1;
            for (int i = 0; i < receivedEyeDataSamples.Count; i++) {
                if (receivedEyeDataSamples[i].isValid) {
                    sampleIdx = i;
                    break;
                }
            }

            // for invalid samples, we skip:
            if (sampleIdx < 0) {
                gazeRayRenderer.materials[0].color = Color.red;
                return;
            } else {
                gazeRayRenderer.materials[0].color = colorRay;
            }

			gazeOriginLocal = new Vector3(
                receivedEyeDataSamples[sampleIdx].position_x_local, 
                receivedEyeDataSamples[sampleIdx].position_y_local, 
                receivedEyeDataSamples[sampleIdx].position_z_local
            );

			gazeDirection = new Vector3(
                receivedEyeDataSamples[sampleIdx].direction_x_local, 
                receivedEyeDataSamples[sampleIdx].direction_y_local, 
                receivedEyeDataSamples[sampleIdx].direction_z_local
            );

            gazeRayRenderer.SetPosition(0, gazeOriginLocal + (Vector3.forward * gazeOriginOffsetZ));
            gazeRayRenderer.SetPosition(1, gazeOriginLocal + gazeDirection * lengthOfRay);

		}

		#endregion // -------------------------------------------------------------------------------------------------------------------------------
	}
}