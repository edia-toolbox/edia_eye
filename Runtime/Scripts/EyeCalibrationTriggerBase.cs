using UnityEngine;
using Edia;

namespace Edia.Eye {

	public class EyeCalibrationTriggerBase: MonoBehaviour {

		void Start () {
			EventManager.StartListening("EvEyeCalibrationRequested", OnEvEyeCalibrationRequested);
		}

		void OnDestroy() {
			EventManager.StopListening("EvEyeCalibrationRequested", OnEvEyeCalibrationRequested);
		}

		public virtual void OnEvEyeCalibrationRequested (eParam e) {
			// Intentionally empty
		}
	}
}