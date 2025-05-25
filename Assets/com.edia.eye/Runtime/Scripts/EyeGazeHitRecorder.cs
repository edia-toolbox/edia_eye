using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Edia.Eye {
    /// <summary>
    /// A component for recording eye gaze interactions, including local ray hits and UV coordinates.
    /// Integrates with Unity's XR Interaction Toolkit and handles gaze-based hover detection.
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    [AddComponentMenu("EDIA/Eye/Eye Gaze Hit Recorder")]
    [EdiaHeader("EDIA EYE", "EyeGaze Hit Recorder", "Records local ray hits and UV coordinates of the gaze ray.")]
    public class EyeGazeHitRecorder : XRSimpleInteractable {

        [Tooltip("Record the raycast hit points to memory")]
        public bool RecordLocalRayHits  = true;
        [Tooltip("Record objects UV coordinates too.")]
        public bool RecordUVCoordinates = true;
        public bool ShowDebugRays = false;
        
        public UnityEvent<GameObject> GazeHoverEnter;
        public UnityEvent<GameObject> GazeHoverExit;
        [Tooltip("New valid hitpoint registered")]
        public UnityEvent<Vector3>    NewGazeHitPosition;
        [Tooltip("New valid UV coordinate registered")]
        public UnityEvent<Vector2>    NewUVHitCoordinate;

        private MeshCollider    _meshCollider;
        private bool            _isRecording { get; set; }
        private XRRayInteractor _rayInteractor;
        private List<Vector3>   _localHitPositions = new List<Vector3>();
        private DebugRaysRenderer _debugRaysRenderer;
        
        protected override void Awake() {
            base.Awake();

            allowGazeInteraction = true;
            
            if (ShowDebugRays && GetComponent<DebugRaysRenderer>() == null)
                _debugRaysRenderer = gameObject.AddComponent<DebugRaysRenderer>();
            
            if (colliders.Count == 0) {
                Debug.Log("No collider found, attempting get-component!");
                try {
                    _meshCollider = GetComponent<MeshCollider>();
                    colliders.Add(_meshCollider);
                }
                catch {
                    Debug.Log("No collider found!");
                }
            }
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args) {
            base.OnHoverEntered(args);

            if (RecordLocalRayHits) {
                _rayInteractor = args.interactorObject as XRRayInteractor;
                _isRecording   = true;
            }

            GazeHoverEnter?.Invoke(this.gameObject);
            XRManager.Instance.AddToConsole($"GazeHoverEnter: {transform.name}");
        }

        private void Update() {
            if (!_isRecording || _rayInteractor == null)
                return;

            if (_rayInteractor != null) {
                if (_rayInteractor.TryGetCurrent3DRaycastHit(out var hit)) {

                    Vector3 localPosition = transform.InverseTransformPoint(hit.point);
                    _localHitPositions.Add(localPosition);
                    NewGazeHitPosition?.Invoke(localPosition);
                    if (ShowDebugRays)
                            _debugRaysRenderer.AddRay(localPosition, hit.normal);

                    if (RecordUVCoordinates) {
                        NewUVHitCoordinate?.Invoke(hit.textureCoord);
                    }
                }
            }
        }
        
        protected override void OnHoverExited(HoverExitEventArgs args) {
            base.OnHoverExited(args);

            _isRecording   = false;
            _rayInteractor = null;

            GazeHoverExit?.Invoke(this.gameObject);
            ClearHitPositions();
            XRManager.Instance.AddToConsole($"GazeHoverExit: {transform.name}");
        }

        // Method to clear recorded hit positions
        public void ClearHitPositions() {
            _localHitPositions.Clear();
        }

    }
}