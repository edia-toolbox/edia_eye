using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Edia.Eye {
    
    /// <summary>EyeData handler
    /// EyeDataHandler 
    /// - is a singleton
    /// - receives data from an EyeTracker (or something that pretends to be) via its public `AddEyeDataPackage()` method
    /// - puts these samples into an internal list `currentSamples`
    /// - has a list of registered `DataClients` (Instances that implement `IEyeDataClient`) 
    /// - on the end of each frame
    /// 	- activates the `Lock` to protect the access to `currentSamples` (to avoid new values being written by another thread while it's reading out)
    /// 	- pushes the samples currently in the `currentSamples` to all DataClients via their `ProcessCurrentSamples()` method
    /// 	- empties the `currentSamples`
    /// 	- releases the lock again
    /// </summary>
    public sealed class EyeDataHandler : MonoBehaviour {

        [Header("Refs")]
        public List<MonoBehaviour> DataClients;

        [Header("Debug")]
        public bool AutoStartProcessing = false;

        List<EyeDataPackage> _currentSamples { get; } = new List<EyeDataPackage>();

        // Lock for thread safety:
        private readonly object _lock = new object();

        // TODO: extend script from Edia.core <singeleton>  
        // Pseudo-Singleton pattern to make sure we have only one DataHandler in the scene:
        public static EyeDataHandler Instance;
        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else if (Instance != this) {
                Destroy(this.gameObject);
            }
        }

        private void Start() {
            if (AutoStartProcessing)
                StartPushingSamples();
        }

        /// <summary> Start the coroutine that sends data to the `DataClients`. Called when recording should start </summary>
        public void StartPushingSamples() {
            Debug.Log("Started pushing");
            ResetCurrentSamplesCollection();
            StartCoroutine(PushSamplesThreadsafeAndReset());
        }

        /// <summary> Coroutine that pushes the items in `currentSamples` to all `DataClients` in a somewhat threadsafe way. 
        /// Empties the list after all samples are sent.  </summary>
        private IEnumerator PushSamplesThreadsafeAndReset() {
            while (true) {
                yield return new WaitForEndOfFrame();

                if (_currentSamples.Count == 0)
                    continue;

                lock (_lock) {
                    // Push listed samples to all clients
                    foreach (IEyeDataClient dataClient in DataClients) {
                        dataClient.ProcessCurrentSamples(_currentSamples);
                    }

                    ResetCurrentSamplesCollection(); // Reset Queue
                }
            }
        }

        /// <summary>Empties the List of `currentSamples`. </summary>
        private void ResetCurrentSamplesCollection() {
            _currentSamples.Clear();
        }

        /// <summary> Add samples to the list of `currentSamples`.  </summary>
        /// <param name="latestSample">Latest sample recorded by the eye tracker.</param>
        public void AddEyeDataPackage(EyeDataPackage latestSample) {
            _currentSamples.Add(latestSample);
        }
    }
}