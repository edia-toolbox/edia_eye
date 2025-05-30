using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace Edia.Eye {

    /// <summary>
    /// Tracks and records raycast hit data using the UXF (Unity Experiment Framework) Tracker framework.
    /// This class integrates with EyeGazeHitRegistration and processes hit position as well as UV coordinate data.
    /// </summary>
    /// <remarks>
    /// The <c>EyeGazeHitUXFTracker</c> adds raycast hit sample data to the UXF tracker system. It retrieves hitpoints
    /// (local positions) and UV coordinates, stores them for frame-by-frame processing, and logs them in the UXF data management system.
    /// Ensure this component is linked with an <c>EyeGazeHitRegistration</c> for correct operation,
    /// as it depends on real-time data streams provided by the registration system.
    /// </remarks>
    [EdiaHeader("EDIA EYE", "Eye Gaze Hit UXF Tracker", "Tracks and saves Raycast Hit data using UXF Tracker")]
    [RequireComponent(typeof(EyeGazeHitRegistration))]
    [AddComponentMenu("EDIA/Eye/Eye Gaze Hit UXF Tracker")]
    public class EyeGazeHitUXFTracker : Tracker {
#region DECLARATIONS
        
        [Header("Settings")]
        [InspectorHelpBox("Processes hitpoint + uv data sample from EyeGazeHitRegistration")]
        public bool AutoRegisterToHitRegistration = true;
        
        public override string              MeasurementDescriptor => $"eye-gazehits";
        public override IEnumerable<string> CustomHeader          => Properties2Log;

        /// <summary>
        /// Represents the list of property names to be included in the tracker log.
        /// </summary>
        /// <remarks>
        /// This array defines the headers for tracked data, including the local hit positions (`hit_x_local`, `hit_y_local`, `hit_z_local`)
        /// and UV coordinates (`uv_x`, `uv_y`) derived from raycast operations. These properties will be appended to the UXF tracker system
        /// for logging and analysis.
        /// </remarks>
        string[] Properties2Log = new string[] {
            "hit_x_local",
            "hit_y_local",
            "hit_z_local",
            "uv_x",
            "uv_y",
        };

        private List<float[]>          receivedGazeHitSamples = new();
        private EyeGazeHitRegistration _eyeGazeHitRegistration;

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region IMPLEMENTATION

        void Awake() {
            gameObject.name         = ($"Eye-{objectName}-GazeHit-UxfTracker");
            _eyeGazeHitRegistration = GetComponent<EyeGazeHitRegistration>();
        }

        /// <summary> Auto add myself to UXF trackers in start, as the Session singleton does not exist earlier </summary>
        private void Start() {
            if (AutoRegisterToHitRegistration) 
                RegisterWithHitRegistration();
            RegisterWithUXF();
        }

        private void OnDestroy() {
            _eyeGazeHitRegistration.NewHit.RemoveListener(AddSample);
        }

        private void RegisterWithHitRegistration() {
            _eyeGazeHitRegistration.NewHit.AddListener(AddSample);
        }

        private void RegisterWithUXF() {
            if (UXF.Session.instance != null)
                UXF.Session.instance.trackedObjects.Add(this);
            else {
                Debug.LogError("Session not yet initialized. You probably need to add the <b>Edia-Executer</b> prefab to the scene.");
            }
        }

        /// <summary>
        /// Adds a new sample to the list of received gaze hit samples for tracking.
        /// </summary>
        /// <param name="currentSample">An array of floats representing the current sample data to be added.</param>
        public void AddSample(float[] currentSample) {
            receivedGazeHitSamples.Add(currentSample);
        }
        
#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region TRACKER

        void LateUpdate() {
            if (this.Recording) {
                RecordRow(); // Inherited from UXF tracker
            }
        }

        protected override UXFDataRow GetCurrentValues() {
            UXFDataRow row = new UXFDataRow();
            if (receivedGazeHitSamples.Count > 0) {
                row = ParseSampleToUXFrow(receivedGazeHitSamples[0]);
                receivedGazeHitSamples.RemoveAt(0);
            }
            else {
                Debug.Log("Queue is empty.");
            }

            return row;
        }

        UXFDataRow ParseSampleToUXFrow(float[] sample) {
            UXFDataRow row = new UXFDataRow();
            for (int i = 0; i < sample.Length; i++) {
                row.Add((Properties2Log[i], sample[i]));
            }

            return row;
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------

    }
}