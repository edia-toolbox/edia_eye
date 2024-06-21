using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Edia;

namespace Edia.Eye {

	public class PanelEyeOptions : Edia.Controller.ExperimenterPanel {

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
		
		private void OnEvEnableEyeCalibrationTrigger(eParam obj)
		{
			// Debug.Log(name + "OnEvEnableEyeCalibrationTrigger: " + obj.GetBool());
			if (obj.GetBool()) {
				ShowPanel();
				btnEyeCalibration.interactable = obj.GetBool();
			} else HidePanel();
		}

		public void Reset() {
			btnEyeCalibration.interactable = true;
		}

	}

}