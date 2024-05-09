using System.Collections.Generic;
using Edia;
using UnityEngine;
using UXF;

namespace Edia.Eye {

    /// <summary>
    /// Data client to the `EyeDataHandler` that receives the data and writes it to UXF logfiles.
    /// needs to be hooked to the list of Tracked Objects on the [ UXF_Rig ]. 
    /// TODO: tbc ...
    /// </summary>
    public class EyeDataClientUXFTracker : Tracker, IEyeDataClient {

        public Edia.Constants.EyeId Eye = Edia.Constants.EyeId.CENTER;

        #region DECLARATIONS 

        string[] Properties2Log = new string[] {
            "timestamp_et",
            "timestamp_lsl",
            "direction_x_local",
            "direction_y_local",
            "direction_z_local",
            "position_x_local",
			"position_y_local",
			"position_z_local",
			"diameter",
			"confidence"
    };

		private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage> ();

        #endregion // -------------------------------------------------------------------------------------------------------------------------------
        #region IEyeDataClient INTERFACE IMPLEMENTATION 

        /// <summary>
        /// Called from EyeDataHandler, supplies new eyedata frame(s)
        /// </summary>
        /// <param name="currentSamples">Sampled eyedata in this frame</param>
        public void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
            receivedEyeDataSamples.Clear();
            foreach (var sample in currentSamples) {
                if (sample.eye.ToLower() == Eye.ToString().ToLower())
                    receivedEyeDataSamples.Add(sample);
            }
        }

        #endregion // -------------------------------------------------------------------------------------------------------------------------------
        #region PROCESSING SAMPLES

        //As we're always recording "manually" with this tracker, we can overwrite LateUpdate
        void LateUpdate () {
			if (this.Recording) {
				RecordCurrentSamples ();
			}
		}

		void RecordCurrentSamples () {
			while (receivedEyeDataSamples.Count > 0) {
				RecordRow (); // Inherited from UXF tracker
				receivedEyeDataSamples.RemoveAt (0);
			}
		}

		UXFDataRow ParseEyeDataToUXFrow (EyeDataPackage eyeData) {
			UXFDataRow row = new UXFDataRow ();
			foreach (string prop2log in Properties2Log) {
				row.Add ((prop2log, GetFieldValue (eyeData, prop2log)));
			}
			return row;
		}

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region  TRACKER

		protected override void SetupDescriptorAndHeader () {
			measurementDescriptor = $"eye-tracking-{Eye.ToString().ToLower()}";
			customHeader = Properties2Log;
		}

		protected override UXFDataRow GetCurrentValues () {
			UXFDataRow row = new UXFDataRow ();
			if (receivedEyeDataSamples.Count > 0) {
				row = ParseEyeDataToUXFrow (receivedEyeDataSamples[0]);
			} else {
				Debug.Log ("ET queue is empty. Not a good sign.");
			}
			return row;
		}

#endregion // -------------------------------------------------------------------------------------------------------------------------------

		// TODO: This should go to the `Utils` of the framweork
		object GetFieldValue (object src, string propName) {
			return src.GetType ().GetField (propName).GetValue (src);
		}
	}
}