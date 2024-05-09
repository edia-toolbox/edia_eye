using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edia.Eye
{
    public class EyeDataClientGazeVisualizer : MonoBehaviour, IEyeDataClient
    {

        #region DECLARATIONS 

		[Header("Which Eye?")]
		public Edia.Constants.EyeId Eye = Edia.Constants.EyeId.CENTER;

		[Header ("Ray")]
		public LineRenderer GazeRayRenderer;
		public int LengthOfRay = 25;
		public float gazeOriginOffsetZ = 0.05f;

        [Header("Settings")]
        public int updateDelay = 50;

        Vector3 GazeDirectionCombined;
        Vector3 GazeOriginCombinedLocal;
        int counter = 0;



        private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();

        #endregion // -------------------------------------------------------------------------------------------------------------------------------
        #region INITS	
        private void OnEnable()
        {
            this.transform.parent = XRManager.Instance.XRCam;
        }

        void Start()
        {
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

            //Debug.Log($"EyeDataClientGazeRecorder: {receivedEyeDataSamples.Count} samples received");

            if (receivedEyeDataSamples.Count == 0)
                return;

            // Substract the vector from 0,0,0 to have the center between two eyes offset from the lenses
            GazeOriginCombinedLocal = new Vector3(receivedEyeDataSamples[0].position_x_local, receivedEyeDataSamples[0].position_y_local, receivedEyeDataSamples[0].position_z_local);

            // The gaze starts from GazeOriginCombinedLocal in the given direction
            GazeDirectionCombined = new Vector3(receivedEyeDataSamples[0].direction_x_local, receivedEyeDataSamples[0].direction_y_local, receivedEyeDataSamples[0].direction_z_local);

            GazeRayRenderer.SetPosition(0, new Vector3(0f, 0f, gazeOriginOffsetZ));
            GazeRayRenderer.SetPosition(1, GazeOriginCombinedLocal + GazeDirectionCombined * LengthOfRay);
        }

        #endregion // -------------------------------------------------------------------------------------------------------------------------------
    }
}