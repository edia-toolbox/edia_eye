using Edia.Eye;
using UnityEngine;
using System.Collections.Generic;

namespace Edia.Eye {
    public abstract class EyeDataClient : MonoBehaviour, IEyeDataClient
    {
        [Header("References")]
        [InspectorHeader("EDIA EYE", "Gaze Visualizer", "Shows the eye gaze ray in the scene.")]
        [Tooltip("Add link to Eye Data Handler Prefab in the scene.")]
        public GameObject EyeDataHandler;

        private void AddToEyeDataHandler()
        {
            // add to the list of data clients on the EyeDataHandler
            EyeDataHandler.GetComponent<EyeDataHandler>().AddDataClient(this);
        }

        public abstract void ProcessCurrentSamples(List<EyeDataPackage> currentSamples);

        private protected virtual void Awake()
        {
            AddToEyeDataHandler();
        }
    }
}