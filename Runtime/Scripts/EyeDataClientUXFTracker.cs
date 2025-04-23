using System;
using System.Collections.Generic;
using Edia;
using UnityEngine;
using UXF;

namespace Edia.Eye {

    /// <summary>
    /// Data client to the `EyeDataHandler` that receives the data and writes it to UXF logfiles.
    /// 1. needs to be hooked to the list of Tracked Objects on the [ UXF_Rig ]. 
    /// 2. also needs to be hooked to the EyeDataHandler
    /// </summary>
    public class EyeDataClientUXFTracker : Tracker, IEyeDataClient {
        public Edia.Constants.EyeId Eye = Edia.Constants.EyeId.CENTER;
        
        #region DECLARATIONS 
        // When properties are added to this list in script -> call component RESET on inspector to force updating the component.
        
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
            "confidence",
            "openness",
            "eye"
    };

        private List<EyeDataPackage> receivedEyeDataSamples = new List<EyeDataPackage>();

        #endregion // -------------------------------------------------------------------------------------------------------------------------------
        #region IEyeDataClient INTERFACE IMPLEMENTATION 

        void Awake() {
            objectName = Eye.ToString().ToLower();
            gameObject.name = ($"Eye-{objectName}-UxfTracker");
        }

        /// <summary> Auto add myself to UXF trackers in start, as the Session singleton does not exist earlier </summary>
        private void Start() {
            RegisterWithUXF();
        }

        private void RegisterWithUXF() {
            if (UXF.Session.instance != null)
                UXF.Session.instance.trackedObjects.Add(this);
            else {
                Debug.LogError("Session not yet initialized. You probably need to add the <b>Edia-Executer</b> prefab to the scene.");
            }
        }

        /// <summary> Called from EyeDataHandler, supplies new eyedata frame(s) </summary>
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
        void LateUpdate() {
            if (this.Recording) {
                RecordCurrentSamples();
            }
        }

        void RecordCurrentSamples() {
            //Debug.Log($"receivedEyeDataSamples {receivedEyeDataSamples.Count}");
            while (receivedEyeDataSamples.Count > 0) {
                RecordRow(); // Inherited from UXF tracker
                receivedEyeDataSamples.RemoveAt(0);
            }
        }

        UXFDataRow ParseEyeDataToUXFrow(EyeDataPackage eyeData) {
            UXFDataRow row = new UXFDataRow();
            foreach (string prop2log in Properties2Log) {
                row.Add((prop2log, GetFieldValue(eyeData, prop2log)));
            }
            return row;
        }

        #endregion // -------------------------------------------------------------------------------------------------------------------------------
        #region  TRACKER

        public override string MeasurementDescriptor => $"-eye-tracking";
        public override IEnumerable<string> CustomHeader => Properties2Log;

        protected override UXFDataRow GetCurrentValues() {
            UXFDataRow row = new UXFDataRow();
            if (receivedEyeDataSamples.Count > 0) {
                row = ParseEyeDataToUXFrow(receivedEyeDataSamples[0]);
            } else {
                Debug.Log("ET queue is empty. Not a good sign.");
            }
            return row;
        }

        #endregion // -------------------------------------------------------------------------------------------------------------------------------

        // TODO: This should go to the `Utils` of the framweork
        object GetFieldValue(object src, string propName) {
            return src.GetType().GetField(propName).GetValue(src);
        }
    }
}