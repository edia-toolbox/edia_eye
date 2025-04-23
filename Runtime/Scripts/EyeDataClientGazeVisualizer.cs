using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;
using System.Linq;

namespace Edia.Eye {
	public class EyeDataClientGazeVisualizer : MonoBehaviour, IEyeDataClient {
		#region DECLARATIONS 

		[Header("Which Eye?")]
		public Constants.EyeId Eye = Constants.EyeId.CENTER;

		LineRenderer gazeRayRenderer;
		int lengthOfRay = 25;
		float gazeOriginOffsetZ = 0.05f;

		[Header("Settings")]
		[Tooltip("Update the gaze ray only every Xth update.")]
		public int UpdateStep = 50;
		[Tooltip("Hides the ray after X seconds with no new sample.")]
        public float timeoutAfterSecondsWithNoNewSample = 4f;

        Vector3 gazeDirection;
		Vector3 gazeOriginLocal;
		int counter = 0;

		Color colorRay;
		Color colorLeft = Color.green;
		Color colorRight = Color.yellow;
		Color colorCenter = Color.cyan;
		Color colorInvalid = Color.red;

		float timeLastSample = -1f;
		

		private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();

		#endregion // -------------------------------------------------------------------------------------------------------------------------------
		#region INITS	
		private void Awake() {
			gazeRayRenderer = GetComponent<LineRenderer>();
		}

		void Start() {
			this.transform.parent = XRManager.Instance.XRCam;
			this.transform.localPosition = Vector3.zero;
			this.transform.localRotation = Quaternion.identity;

			colorRay = Eye == Constants.EyeId.CENTER ? colorCenter : Eye == Constants.EyeId.LEFT ? colorLeft : colorRight;
			gazeRayRenderer.materials[0].color = colorInvalid;

			counter = UpdateStep;
		}

		#endregion // -------------------------------------------------------------------------------------------------------------------------------
		#region IEyeDataClient INTERFACE IMPLEMENTATION 

		public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
			foreach (var sample in currentSamples) {
				if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
                    receivedEyeDataSamples.Clear();
                    receivedEyeDataSamples.Add(sample);
					timeLastSample = Time.time;
				}
			}
		}

		#endregion // -------------------------------------------------------------------------------------------------------------------------------
		#region PROCESSING SAMPLES

		void Update() {
			counter--;

			if (counter < 0)
				UpdateGazeRays();
		}

		void UpdateGazeRays() {
			counter = UpdateStep;

			if (receivedEyeDataSamples.Count == 0 | (Time.time - timeLastSample > timeoutAfterSecondsWithNoNewSample)) {
				UpdateRayPosition(gazeOriginLocal + Vector3.zero, Vector3.zero);
				return;
			}

			EyeDataPackage validEyeDataPackage = receivedEyeDataSamples.FirstOrDefault(x => x.isValid); // find first valid package

            // no valid samples, we skip:
            if (validEyeDataPackage == default) {
				gazeRayRenderer.materials[0].color = Color.red;
				return;
			}

			gazeOriginLocal = new Vector3(
				validEyeDataPackage.position_x_local,
				validEyeDataPackage.position_y_local,
				validEyeDataPackage.position_z_local
			);

			gazeDirection = new Vector3(
				validEyeDataPackage.direction_x_local,
				validEyeDataPackage.direction_y_local,
				validEyeDataPackage.direction_z_local
			);

			gazeRayRenderer.materials[0].color = colorRay;
			UpdateRayPosition(gazeOriginLocal + (Vector3.forward * gazeOriginOffsetZ), gazeOriginLocal + gazeDirection * lengthOfRay);
        }

		private void UpdateRayPosition(Vector3 startPosition, Vector3 endPosition) {
			gazeRayRenderer.SetPosition(0, startPosition);
			gazeRayRenderer.SetPosition(1, endPosition);
		}

		#endregion // -------------------------------------------------------------------------------------------------------------------------------
	}
}