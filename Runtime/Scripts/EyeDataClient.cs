using UnityEngine;
using System.Collections.Generic;

namespace Edia.Eye {
    [EdiaHeader("EDIA EYE", "EyeData Client", "Eye data package client")]
    public abstract class EyeDataClient : MonoBehaviour, IEyeDataClient {

        private void AddToEyeDataHandler()
        {
            if (EyeDataHandler.Instance == null) {
                Debug.LogError("EyeDataHandler is not in the scene. Please add it to the scene.");
                return;
            }
            
            // add to the list of data clients on the EyeDataHandler
            EyeDataHandler.Instance.AddDataClient(this);
        }

        public abstract void ProcessCurrentSamples(List<EyeDataPackage> currentSamples);

        private protected virtual void Awake()
        {
            AddToEyeDataHandler();
        }
    }
}