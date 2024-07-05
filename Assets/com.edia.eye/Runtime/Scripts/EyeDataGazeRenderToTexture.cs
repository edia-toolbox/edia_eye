using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edia.Eye
{
    public class EyeDataGazeRenderToTexture : MonoBehaviour, IEyeDataClient
    {
#region DECLARATIONS 

		[Header("Which Eye?")]
		public Edia.Constants.EyeId Eye = Edia.Constants.EyeId.CENTER;

		[Header ("Ray")]
		public LineRenderer GazeRayRenderer;
		public int LengthOfRay = 25;
		public float GazeOriginOffsetZ = 0.05f;

        [Header("Settings")]
        [Tooltip("Update the gaze ray only every Xth update.")]
        public int UpdateStep = 50;

        Vector3 GazeDirection;
        Vector3 GazeOriginLocal;
        int counter = 0;

        private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region INITS	
        void Start()
        {
            SetLayer(gameObject, 9);
            
            this.transform.parent = XRManager.Instance.XRCam;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            counter = UpdateStep;
        }

		void SetLayer(GameObject obj, int newLayer) {
		
            obj.layer = newLayer;
			foreach (Transform child in obj.transform) {
				SetLayer(child.gameObject, newLayer);
			}
		}

		void OnValidate() {
#if UNITY_EDITOR
            if (LayerMask.LayerToName(9) != "EyeTrackingViz") {
                Debug.LogError($"Layer 9 'EyeTrackingViz' not existing. Run Menu>Edia>Configurator to generate needed layers");
            }
#endif
		}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region IEyeDataClient INTERFACE IMPLEMENTATION 

		public void ProcessCurrentSamples (List<EyeDataPackage> currentSamples) {
			receivedEyeDataSamples.Clear ();
			foreach (var sample in currentSamples) {
				if (sample.eye.ToLower() == "center")
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
            counter = UpdateStep;

            //Debug.Log($"EyeDataClientGazeRecorder: {receivedEyeDataSamples.Count} samples received");

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

			GazeRayRenderer.SetPosition(0, GazeOriginLocal + (Vector3.forward * GazeOriginOffsetZ));
			GazeRayRenderer.SetPosition(1, GazeOriginLocal + GazeDirection * LengthOfRay);
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
    }
}