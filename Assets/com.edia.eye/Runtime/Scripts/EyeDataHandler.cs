using System;
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
    [EdiaHeader("EDIA EYE", "Eye Data Handler","Manages incoming eyetracking data from SDK, conversion to EDIA, and forwarding data to listed dataclients.")]
    public sealed class EyeDataHandler : Singleton<EyeDataHandler> {
        [Header("Debug")]
            
        [Tooltip("If set to `true`, the `EyeDataHandler` starts processing and pushing samples immediately when the scene " +
                 "starts. If set to `false`, this must be initiated from code using `StartPushingSamples()`.")]
        public bool ProcessOnStart = false;

        [Space(10)]
        public bool IsDebug = false;

        // Lock for thread safety:
        /// <summary>
        /// A lock object used for thread safety when accessing the `_currentSamples` list.
        /// </summary>
        public readonly object Lock = new object();

        private List<IEyeDataClient> _dataClients = new();
        private List<EyeDataPackage> _currentSamples { get; } = new List<EyeDataPackage>();

        private float      _rayDistance = 50f;
        private int        _gazeLayer;
        private GameObject _recticle;

        private void Awake() {
            _gazeLayer = LayerMask.GetMask("GazeCollision");

            if (IsDebug)
                GenerateRecticle();
            ;
        }

        private void Start() {
            if (ProcessOnStart)
                StartPushingSamples();
        }

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
            // Debug.Log ("Started pushing");
            ResetCurrentSamplesCollection();
            StartCoroutine(PushSamplesThreadsafeAndReset());
        }

        private IEnumerator PushSamplesThreadsafeAndReset() {
            while (true) {
                yield return new WaitForEndOfFrame();

                if (_currentSamples.Count == 0)
                    continue;

                lock (Lock) {
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
            _currentSamples.Add(RegisterIntersection(latestSample));
        }

        /// <summary>
        /// Updates the given eye data package with intersection data resulting from a raycast operation.
        /// This method computes the world-space position and direction of the eye's gaze, performs a raycast
        /// to detect an intersection with colliders in the scene, and populates the eye data package with the intersection details if a hit occurs.
        /// </summary>
        /// <param name="latestSample">The eye data package containing the initial local-space gaze data.</param>
        /// <returns>The updated eye data package containing intersection details if a hit is detected, or the original data if no hit occurs.</returns>
        private EyeDataPackage RegisterIntersection(EyeDataPackage latestSample) {
            Vector3 Pos = transform.TransformPoint(new Vector3(latestSample.position_x_local, latestSample.position_y_local, latestSample.position_z_local));
            Vector3 dir = transform.TransformDirection(new Vector3(latestSample.direction_x_local, latestSample.direction_y_local,
                latestSample.direction_z_local));

            RaycastHit hit;

            if (Physics.Raycast(Pos, dir, out hit, 50, _gazeLayer)) {
                latestSample.target_id      = hit.collider.name;
                latestSample.intersection_x = hit.point.x;
                latestSample.intersection_y = hit.point.y;
                latestSample.intersection_z = hit.point.z;

                if (IsDebug) {
                    _recticle.transform.position = hit.point;
                    Debug.DrawRay(Pos, dir * hit.distance, Color.green);
                }
            }
            else {
                if (IsDebug) {
                    Debug.DrawRay(Pos, dir * _rayDistance, Color.red);
                    _recticle.transform.position = Vector3.zero;
                }
            }

            return latestSample;
        }

        private void GenerateRecticle() {
            _recticle                                             = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _recticle.transform.localScale                        = Vector3.one * 0.02f;
            _recticle.GetComponent<MeshRenderer>().material.color = new Color(0.17f, 1f, 0f);
        }
    }
}