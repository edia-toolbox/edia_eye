using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Edia.Eye {
    [AddComponentMenu("EDIA/Eye/Eye Gaze Hit Registration")]
    [EdiaHeader("EDIA EYE", "EyeGaze Hit Registration", "Queries if EyeGazeInteractor hits this objects collider(s) and fires events with raycast hit data.")]
    public class EyeGazeHitRegistration : XRSimpleInteractable {

        [Header("Settings")]
        [Tooltip("Track gaze in update loop and provides hitdata if any.")]
        public bool TrackGaze = false;

        [Tooltip("Show debug ray in scene view.")]
        public bool ShowDebugRays = false;

        [Header("Default events:")]
        public UnityEvent<GameObject> GazeHoverEnter;
        public UnityEvent GazeHoverExit;

        [Header("Events fired only when TrackGaze is on:")]
        public UnityEvent<Vector3> NewHitLocalPosition;
        public UnityEvent<Vector2>    NewHitUV;
        public UnityEvent<float[]>    NewHit;
        public UnityEvent<RaycastHit> NewHitRaycastHit;

        [SerializeField] private float[] _currentSample = { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN };

        private bool              _isRecording { get; set; }
        private XRRayInteractor   _rayInteractor;
        private DebugRaysRenderer _debugRaysRenderer;

        // -------------------------------------------------------------------------------------------------------------------------------

        protected override void Awake() {
            base.Awake();

            allowGazeInteraction = true;

            if (ShowDebugRays && GetComponent<DebugRaysRenderer>() == null)
                _debugRaysRenderer = gameObject.AddComponent<DebugRaysRenderer>();
        }

        // -------------------------------------------------------------------------------------------------------------------------------

        protected override void OnHoverEntered(HoverEnterEventArgs args) {
            base.OnHoverEntered(args);

            _rayInteractor = args.interactorObject as XRRayInteractor;
            if (TrackGaze)
                _isRecording = true;

            if (_rayInteractor != null && _rayInteractor.TryGetCurrent3DRaycastHit(out var hit)) {
                GazeHoverEnter?.Invoke(hit.collider.gameObject);
                XRManager.Instance.AddToConsole($"GazeHoverEnter: {hit.collider.name}");
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args) {
            base.OnHoverExited(args);

            _isRecording   = false;
            _rayInteractor = null;

            GazeHoverExit?.Invoke();
        }

        // -------------------------------------------------------------------------------------------------------------------------------

        private void Update() {
            if (!_isRecording || _rayInteractor == null) {
                _currentSample = new float[] { float.NaN, float.NaN, float.NaN, float.NaN, float.NaN };
            }
            else if (_rayInteractor.TryGetCurrent3DRaycastHit(out var hit)) {
                Vector3 localPosition = transform.InverseTransformPoint(hit.point);

                NewHitLocalPosition?.Invoke(localPosition);
                NewHitUV?.Invoke(hit.textureCoord);
                NewHitRaycastHit?.Invoke(hit);

                _currentSample[0] = localPosition.x;
                _currentSample[1] = localPosition.y;
                _currentSample[2] = localPosition.z;
                _currentSample[3] = hit.textureCoord.x;
                _currentSample[4] = hit.textureCoord.y;

                if (ShowDebugRays)
                    _debugRaysRenderer.AddRay(localPosition, hit.normal);
            }

            NewHit?.Invoke(_currentSample);
        }
    }
}