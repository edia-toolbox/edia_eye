using Edia.Eye;
using UnityEngine;
using System.Collections.Generic;

namespace Edia.Eye {
    [EdiaHeader("EDIA EYE", "EyeData Client", "Eye data package client")]
    public abstract class EyeDataClient : MonoBehaviour, IEyeDataClient
    {
        [Header("References")]
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