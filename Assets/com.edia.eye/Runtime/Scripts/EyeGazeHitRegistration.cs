using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Edia.Eye {
    /// <summary>
    /// A component for registering eye gaze hits
    /// </summary>
    [RequireComponent(typeof(MeshCollider))]
    [AddComponentMenu("EDIA/Eye/Eye Gaze Hit Registration")]
    [EdiaHeader("EDIA EYE", "EyeGaze Hit Registration", "Registers ray hits -> provides local pos and UV coordinates of the gaze ray.")]
    public class EyeGazeHitRegistration : XRSimpleInteractable {

        [InspectorHelpBox("Queries if EyeGazeInteractor hits this objects MeshCollider and fires events with raycast hit data")]
        public bool ShowDebugRays = false;

        [Header("Fired when the GazeInteractor hits this objects meshcollider")]
        public UnityEvent<GameObject> GazeHoverEnter;
        [Header("Fired when the GazeInteractor exits this objects meshcollider")]
        public UnityEvent<GameObject> GazeHoverExit;

        [Header("Provides current raycast hit data: local x,y,z as vector3")]
        public UnityEvent<Vector3> NewHitLocalPosition;
        [Header("Provides current raycast hit data: u,v coordinate as vector2")]
        public UnityEvent<Vector2> NewHitUV;

        float[] sample = new float[] {0f,0f,0f,0f,0f};
        [Header("Provides current raycast hit data: local x,y,z + u,v coordinate as float[]")]
        public UnityEvent<float[]> NewHit;
        
        private MeshCollider      _meshCollider;
        private bool              _isRecording { get; set; }
        private XRRayInteractor   _rayInteractor;
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

            _rayInteractor = args.interactorObject as XRRayInteractor;
            _isRecording   = true;

            GazeHoverEnter?.Invoke(this.gameObject);
            XRManager.Instance.AddToConsole($"GazeHoverEnter: {transform.name}");
        }

        private void Update() {

            if (!_isRecording || _rayInteractor == null) {
                sample = new float[] {float.NaN, float.NaN, float.NaN, float.NaN, float.NaN}; 
            } else if (_rayInteractor.TryGetCurrent3DRaycastHit(out var hit)) {
                Vector3 localPosition = transform.InverseTransformPoint(hit.point);

                NewHitLocalPosition?.Invoke(localPosition);
                NewHitUV?.Invoke(hit.textureCoord);
                
                sample[0] = localPosition.x;
                sample[1] = localPosition.y;
                sample[2] = localPosition.z;
                sample[3] = hit.textureCoord.x;
                sample[4] = hit.textureCoord.y;
                
                if (ShowDebugRays)
                    _debugRaysRenderer.AddRay(localPosition, hit.normal);
            }  
            
            NewHit?.Invoke(sample);
        }

        protected override void OnHoverExited(HoverExitEventArgs args) {
            base.OnHoverExited(args);

            _isRecording   = false;
            _rayInteractor = null;

            GazeHoverExit?.Invoke(this.gameObject);
            XRManager.Instance.AddToConsole($"GazeHoverExit: {transform.name}");
        }
    }
}