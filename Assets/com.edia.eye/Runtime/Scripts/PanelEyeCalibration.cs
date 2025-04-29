using UnityEngine;
using UnityEngine.UI;

namespace Edia.Eye {
	/// <summary>
	/// Handles the Controller UI panel for eye calibration during an experiment. 
	/// This class manages the eye calibration button.
	/// </summary>
	public class PanelEyeCalibration : Edia.Controller.ExperimenterPanel {

		/// <summary>
		/// Reference to the Eye Calibration button in the UI.
		/// </summary>
		[Header("Refs")]
		public Button btnEyeCalibration = null;

		void Start() {
			// Listen to core event for enabling calibration button
			EventManager.StartListening("EvEnableEyeCalibrationTrigger", OnEvEnableEyeCalibrationTrigger);
			
			// Fire EvEyeCalibrationRequested event when button is pressed
			btnEyeCalibration.onClick.AddListener( ()=> EventManager.TriggerEvent("EvEyeCalibrationRequested", null));
		}

		void OnDestroy() {
			EventManager.StopListening("EvEnableEyeCalibrationTrigger", OnEvEnableEyeCalibrationTrigger);
		}
		
		/// <summary>
		/// Callback function that handles the event when the eye calibration button's enable/disable state should be changed.
		/// Shows or hides the eye calibration panel based on the event parameter.
		/// </summary>
		/// <param name="obj">The event parameter containing the boolean that determines if the panel should be shown.</param>
		private void OnEvEnableEyeCalibrationTrigger(eParam obj) {
			// Debug.Log(name + "OnEvEnableEyeCalibrationTrigger: " + obj.GetBool());
			if (obj.GetBool()) {
				ShowPanel();
				btnEyeCalibration.interactable = obj.GetBool();
			} else HidePanel();
		}

		/// <summary>
		/// Resets the calibration button, ensuring that it is interactable again.
		/// </summary>
		public void Reset() {
			btnEyeCalibration.interactable = true;
		}

	}

}