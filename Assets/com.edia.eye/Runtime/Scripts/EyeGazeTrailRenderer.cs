using UnityEngine;
using UnityEngine.Serialization;

namespace Edia.Eye {
    /// <summary>
    /// Generates a fading particle-based trail that visualizes the user's gaze points over time.
    /// Call <see cref="AddToTrail(Vector3)"/> with the gaze position each frame to emit new trail points.
    /// The trail appearance (size, lifetime, material) and visibility are configurable via public fields.
    /// </summary>
    public class EyeGazTrailRenderer : MonoBehaviour {
        
        public bool ShowTrail = false;
        [Tooltip("This depends on your render pipeline.")]
        public Material TrailMaterial;
        
        [Tooltip("For how long (in s) shall the trail points be shown?")]
        public int TrailPointLifetime = 10;

        [Tooltip("Size of trail points.")]
        public float TrailPointSize = 0.2f;
        
        
        private ParticleSystem _particleSystem;
        
        private void Awake() {
            CreateParticleTrail();
        }

        private void CreateParticleTrail() {
            _particleSystem = gameObject.AddComponent<ParticleSystem>();
            
            var main = _particleSystem.main;
            main.startColor    = new Color(0.85f, 0.45f, 0.05f);
            main.startSize     = TrailPointSize;
            main.startLifetime = TrailPointLifetime;
            main.startSpeed    = 0f;
            
            var visuals = _particleSystem.GetComponent<ParticleSystemRenderer>();
            visuals.material   = TrailMaterial;
            visuals.renderMode = ParticleSystemRenderMode.Mesh;
            GameObject renderSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere); 
            visuals.mesh = renderSphere.GetComponent<MeshFilter>().sharedMesh;
            DestroyImmediate(renderSphere);
            
            var colorOverLifetime = _particleSystem.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(main.startColor.color, 0.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            colorOverLifetime.color = gradient;
            
            var emission = _particleSystem.emission;
            emission.enabled = false;

            var shape = _particleSystem.shape;
            shape.enabled = false;
        }
        
        public void AddToTrail(Vector3 position) {
            if (!ShowTrail)
                return;
            
            var emitParams = new ParticleSystem.EmitParams();
            
            emitParams.position             = position;
            emitParams.applyShapeToPosition = false;
            _particleSystem.Emit(emitParams, 1);
        }
    }
}