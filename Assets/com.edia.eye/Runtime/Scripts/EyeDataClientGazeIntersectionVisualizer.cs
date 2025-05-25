using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Edia.Eye {
    [EdiaHeader("EDIA EYE", "EyeData Debug helper", "Trail showing gaze intersection on objects on layer 'GazeCollison'")]
    public class EyeDataClientGazeIntersectionVisualizer : EyeDataClient {
#region DECLARATIONS

        [Header("Which Eye?")]
        public Constants.EyeId Eye = Constants.EyeId.CENTER;

        [Header("Settings")]
        [Tooltip("Update only every Xth update.")]
        public int UpdateStep = 2;
        [Tooltip("Show recticle for current sample.")]
        public bool ShowRecticle = false;
        [Tooltip("Length in seconds.")]
        public int TrailLength = 10;
        
        private int            _counter = 0;
        private GameObject     _recticle;
        private ParticleSystem _particleSystem;

        private float _timeLastSample = -1f;
        private List<EyeDataPackage> _receivedEyeDataSamples = new List<EyeDataPackage>();

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region INITS

        private protected override void Awake() {
            base.Awake();
            GenerateRecticle();
            _particleSystem = GetComponent<ParticleSystem>();
            CreateParticleTrail();
        }

        void Start() {
            _counter = UpdateStep;
        }

        private void GenerateRecticle() {
            _recticle                                                = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _recticle.transform.localScale                           = Vector3.one * 0.022f;
            _recticle.GetComponent<MeshRenderer>().material.color    = new Color(0.85f, 0.45f, 0.05f);
            _recticle.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _recticle.GetComponent<MeshRenderer>().receiveShadows    = false;
            _recticle.name                                           = "Recticle_EyeDataClientGazeIntersectionVisualizer";
            _recticle.SetActive(ShowRecticle);
        }
        
        private void CreateParticleTrail() {
            var main = _particleSystem.main;
            main.startColor    = new Color(0.85f, 0.45f, 0.05f);
            main.startSize     = 0.05f;
            main.startLifetime = TrailLength; 

            var emission = _particleSystem.emission;
            emission.enabled = false; 

            var shape = _particleSystem.shape;
            shape.enabled = false; 
        }
        
        
#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region IEyeDataClient INTERFACE IMPLEMENTATION

        public override void ProcessCurrentSamples(List<EyeDataPackage> currentSamples) {
            foreach (var sample in currentSamples) {
                if (sample.eye.ToLower() == Eye.ToString().ToLower()) {
                    _receivedEyeDataSamples.Clear();
                    _receivedEyeDataSamples.Add(sample);
                    _timeLastSample = Time.time;
                }
            }
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
#region PROCESSING SAMPLES

        void Update() {
            _counter--;
            
            if (_counter < 0)
                UpdateGazeTrail();
        }

        void UpdateGazeTrail() {
            _counter = UpdateStep;

            EyeDataPackage validEyeDataPackage = _receivedEyeDataSamples.FirstOrDefault(x => x.isValid); // find first valid package

            if (validEyeDataPackage == default) 
                return;

            Vector3 intersectionPosition = new Vector3(
                validEyeDataPackage.intersection_x,
                validEyeDataPackage.intersection_y,
                validEyeDataPackage.intersection_z
            );
            
            _recticle.transform.position = intersectionPosition;

            // Emit a particle at the intersection position
            EmitParticleAt(intersectionPosition);
        }

        private void EmitParticleAt(Vector3 position) {
            if (_particleSystem == null) return;

            var emitParams = new ParticleSystem.EmitParams();
            emitParams.position             = position;
            emitParams.applyShapeToPosition = false;
            _particleSystem.Emit(emitParams, 1);
        }

#endregion // -------------------------------------------------------------------------------------------------------------------------------
    }
}