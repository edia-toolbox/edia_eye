using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edia.Eye {

    /// <summary>
    /// The `EyeDataHandler` class is responsible for receiving, managing, and distributing eye-tracking data to registered data clients.
    ///
    /// - Operates as a singleton, ensuring that there is only one instance of it in the scene.
    /// - Receives data from an EyeTracker (or something that pretends to be) via its public AddEyeDataPackage() method.
    /// - Puts these samples into an internal list `_currentSamples`
    /// - Manages a list of registered `DataClients` (objects implementing the `IEyeDataClient` interface).
    /// - At the end (!) of each frame, the class:
    ///     - Locks access to the `_currentSamples` list for thread safety.
    ///     - Sends the collected samples to all registered `DataClients` using their `ProcessCurrentSamples()` method.
    ///     - Empties the `_currentSamples` list
    ///		- Releases the lock to allow further data collection.
    /// </summary>
    [EdiaHeader("EDIA EYE", "Eye Data Handler","Converts SDK ET data to EDIA and pushes it to registered dataclients.")]
    public sealed class EyeDataHandler : Singleton<EyeDataHandler> {
            
        [Tooltip("Starts processing and pushing samples immediately when the scene ")]
        public bool ProcessOnStart = false;

        [Space(10)]
        public bool ShowDebugRay = false;

        // Lock for thread safety:
        /// <summary>
        /// A lock object used for thread safety when accessing the `_currentSamples` list.
        /// </summary>
        public readonly object Lock = new object();

        private List<IEyeDataClient> _dataClients = new();
        private List<EyeDataPackage> _currentSamples { get; } = new List<EyeDataPackage>();

        private float      _rayDistance = 50f;
        private int        _gazeLayer;

        private void Awake() {
            _gazeLayer = LayerMask.GetMask("GazeCollision");
        }

        private void Start() {
            if (ProcessOnStart)
                StartPushingSamples();
        }

        /// <summary>
        /// Adds a new eye-tracking data client to the list of data clients,
        /// if it is not already present in the list.
        /// </summary>
        /// <param name="dataClient">The eye-tracking data client to be added.</param>
        public void AddDataClient(IEyeDataClient dataClient) {
            if (!_dataClients.Contains(dataClient)) {
                _dataClients.Add(dataClient);
            }
        }

        /// <summary>
        /// Starts the coroutine that pushes eye-tracking data to registered data clients.
        /// This should be called when processing of samples should begin.
        /// </summary>
        public void StartPushingSamples() {
            ResetCurrentSamplesCollection();
            StartCoroutine(PushSamplesThreadsafeAndReset());
        }

        /// <summary>
        /// Continuously pushes the current collection of eye-tracking samples to all registered data clients
        /// in a thread-safe manner and resets the sample collection afterwards.
        /// </summary>
        /// <returns>An enumerator to be used with a coroutine, enabling frame-by-frame execution.</returns>
        private IEnumerator PushSamplesThreadsafeAndReset() {
            while (true) {
                yield return new WaitForEndOfFrame();

                if (_currentSamples.Count == 0)
                    continue;
                
                lock (Lock) {
                    // To detect intersections we need the current pose. Note that this assumes the same pose (the one valid
                    // at the end of the frame) for all ET samples collected during the frame. In practice, this should not
                    // make a relevant difference for hit detection. 
                    // Note that we CANNOT use `foreach` (which makes a copy of the list) here, bc we are updating the
                    // samples in-place.
                    for (int i = 0; i < _currentSamples.Count; i++) {
                        RegisterIntersection(_currentSamples[i]);
                    }
                    
                    // Push listed samples to all clients
                    foreach (IEyeDataClient dataClient in _dataClients) {
                        dataClient.ProcessCurrentSamples(_currentSamples);
                    }

                    ResetCurrentSamplesCollection(); // Reset Queue
                }
            }
        }

        private void ResetCurrentSamplesCollection() {
            _currentSamples.Clear();
        }

        /// <summary>
        /// Adds a new eye-tracking data package to the list of current samples.
        /// This method is called by the eye data converter to feed new data into the handler.
        /// </summary>
        /// <param name="latestSample">The latest eye-tracking sample to be added to the list.</param>
        public void AddEyeDataPackage(EyeDataPackage latestSample) {
            _currentSamples.Add(latestSample);
        }

        /// <summary>
        /// Updates (in-place!) the given eye data package with intersection data resulting from a raycast operation.
        /// This method computes the world-space position and direction of the eye's gaze, performs a raycast
        /// to detect an intersection with colliders in the scene, and populates the eye data package with the intersection details if a hit occurs.
        /// </summary>
        /// <param name="latestSample">The eye data package containing the initial local-space gaze data.</param>
        private void RegisterIntersection(EyeDataPackage latestSample) {
            
            Vector3 pos = transform.TransformPoint(new Vector3(
                    latestSample.position_x_local, 
                    latestSample.position_y_local, 
                    latestSample.position_z_local)
            );
            
            Vector3 dir = transform.TransformDirection(new Vector3(
                latestSample.direction_x_local, 
                latestSample.direction_y_local,
                latestSample.direction_z_local)
            );

            RaycastHit hit;

            if (Physics.Raycast(pos, dir, out hit, 50, _gazeLayer)) {
                latestSample.target_id      = hit.collider.name;
                latestSample.intersection_x = hit.point.x;
                latestSample.intersection_y = hit.point.y;
                latestSample.intersection_z = hit.point.z;

                if (ShowDebugRay) { Debug.DrawRay(pos, dir * hit.distance, Color.green); }
            }
            else {
                if (ShowDebugRay) { Debug.DrawRay(pos, dir * _rayDistance, Color.red); }
            }
        }
    }
}