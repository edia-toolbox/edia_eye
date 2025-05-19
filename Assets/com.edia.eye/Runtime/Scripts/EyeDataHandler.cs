using System.Collections;
using System.Collections.Generic;
using Edia;
using UnityEngine;
using UXF;

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

    public sealed class EyeDataHandler : MonoBehaviour

	{
		public static EyeDataHandler Instance;
		[Header("Settings")]
		[InspectorHeader("EDIA EYE", "Eye Data Handler", "Manages incoming eyetracking data from SDK, conversion to EDIA, and forwarding data to listed dataclients.")]
		
		[Header("Debug")]
		[Tooltip("If set to `true`, the `EyeDataHandler` starts processing and pushing samples immediately when the scene " +
		         "starts. If set to `false`, this must be initiated from code using `StartPushingSamples()`.")]
		public bool ProcessOnStart = false;

        // Lock for thread safety:
        /// <summary>
        /// A lock object used for thread safety when accessing the `_currentSamples` list.
        /// </summary>
        public readonly object Lock = new object();

        private List<IEyeDataClient> _dataClients = new(); 
        private List<EyeDataPackage>  _currentSamples { get; } = new List<EyeDataPackage> ();

		// Pseudo-Singleton pattern to make sure we have only one DataHandler in the scene:
		private void Awake () {
			if (Instance == null) {
				Instance = this;
			} else if (Instance != this) {
				Destroy (this.gameObject);
			}
		}

		private void Start() {
			if(ProcessOnStart)
				StartPushingSamples();
		}

		public void AddDataClient(IEyeDataClient dataClient) {
			if (!_dataClients.Contains(dataClient))
			{
				_dataClients.Add(dataClient);
			}
		}

		/// <summary>
		/// Starts the coroutine that pushes eye-tracking data to registered data clients.
		/// This should be called when processing of samples should begin.
		/// </summary>
		public void StartPushingSamples () {
			// Debug.Log ("Started pushing");
			ResetCurrentSamplesCollection ();
			StartCoroutine (PushSamplesThreadsafeAndReset ());
		}
		
		
		private IEnumerator PushSamplesThreadsafeAndReset () {
			while (true) {
				yield return new WaitForEndOfFrame ();

				if (_currentSamples.Count == 0)
					continue;

				lock (Lock) {
					// Push listed samples to all clients
					foreach (IEyeDataClient dataClient in _dataClients) {
						dataClient.ProcessCurrentSamples (_currentSamples);
					}

                    ResetCurrentSamplesCollection (); // Reset Queue
				}
			}
		}

		private void ResetCurrentSamplesCollection () {
			_currentSamples.Clear ();
		}

		/// <summary>
		/// Adds a new eye-tracking data package to the list of current samples.
		/// This method is called by the eye data converter to feed new data into the handler.
		/// </summary>
		/// <param name="latestSample">The latest eye-tracking sample to be added to the list.</param>
		public void AddEyeDataPackage (EyeDataPackage latestSample) {
            _currentSamples.Add (latestSample);
		}
	}
}