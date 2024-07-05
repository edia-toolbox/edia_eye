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

		LineRenderer GazeRayRenderer;
		int LengthOfRay = 25;
		float gazeOriginOffsetZ = 0.05f;

        [Header("Settings")]
        public int updateDelay = 50;

        Vector3 GazeDirection;
        Vector3 GazeOriginLocal;
        int counter = 0;

        private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region INITS	
		private void Awake() {
	        GazeRayRenderer	= GetComponent<LineRenderer>();
		}

		void Start()
        {
            this.transform.parent = XRManager.Instance.XRCam;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;

			GazeRayRenderer.materials[0].color = Eye == Constants.EyeId.CENTER ? Color.cyan : Eye == Constants.EyeId.LEFT? Color.green: Color.yellow;

            counter = updateDelay;
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region IEyeDataClient INTERFACE IMPLEMENTATION 

		public void ProcessCurrentSamples (List<EyeDataPackage> currentSamples) {
			receivedEyeDataSamples.Clear ();
			foreach (var sample in currentSamples) {
				if (sample.eye.ToLower() == Eye.ToString().ToLower())
					receivedEyeDataSamples.Add (sample);
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
            counter = updateDelay;

            if (receivedEyeDataSamples.Count == 0)
                return;

            GazeOriginLocal = new Vector3(
                receivedEyeDataSamples[0].position_x_local, 
                receivedEyeDataSamples[0].position_y_local, 
                receivedEyeDataSamples[0].position_z_local
            );

			GazeDirection = new Vector3(
                receivedEyeDataSamples[0].direction_x_local, 
                receivedEyeDataSamples[0].direction_y_local, 
                receivedEyeDataSamples[0].direction_z_local
            );

            GazeRayRenderer.SetPosition(0, new Vector3(0f, 0f, gazeOriginOffsetZ));
            GazeRayRenderer.SetPosition(1, GazeOriginLocal + GazeDirection * LengthOfRay);
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
    }
}