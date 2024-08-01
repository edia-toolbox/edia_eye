using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edia;

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
			receivedEyeDataSamples.Clear();
			foreach (var sample in currentSamples) {
				if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
					receivedEyeDataSamples.Add(sample);
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

			if (receivedEyeDataSamples.Count == 0) {
				Debug.Log($"No dataframes");
				gazeRayRenderer.materials[0].color = colorInvalid;
				UpdateRayPosition(gazeOriginLocal + Vector3.zero, Vector3.forward);
				return;
			}

			if (receivedEyeDataSamples[0].isValid) {

				gazeOriginLocal = new Vector3(
					receivedEyeDataSamples[0].position_x_local,
					receivedEyeDataSamples[0].position_y_local,
					receivedEyeDataSamples[0].position_z_local
				);

				gazeDirection = new Vector3(
					receivedEyeDataSamples[0].direction_x_local,
					receivedEyeDataSamples[0].direction_y_local,
					receivedEyeDataSamples[0].direction_z_local
				);

				gazeRayRenderer.materials[0].color = colorRay;
				UpdateRayPosition(gazeOriginLocal + (Vector3.forward * gazeOriginOffsetZ), gazeOriginLocal + gazeDirection * lengthOfRay);
			}
			else {
				gazeRayRenderer.materials[0].color = colorInvalid;
				UpdateRayPosition(gazeOriginLocal + Vector3.zero, Vector3.forward);
			}
		}

		private void UpdateRayPosition(Vector3 startPosition, Vector3 endPosition) {
			gazeRayRenderer.SetPosition(0, startPosition);
			gazeRayRenderer.SetPosition(1, endPosition);
		}

		#endregion // -------------------------------------------------------------------------------------------------------------------------------
	}
}