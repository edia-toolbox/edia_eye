using UnityEngine;

namespace Edia.Eye {
    public class EyeGazeReticleRenderer : MonoBehaviour {

        public  bool       ShowReticle = false;
        public  float      ReticleSize = 0.1f;
        
        private GameObject _reticle;

        private void Awake() {
            GenerateReticle();
        }

        private void GenerateReticle() {
            _reticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(_reticle.GetComponent<SphereCollider>());
            _reticle.transform.localScale                           = Vector3.one * ReticleSize;
            _reticle.GetComponent<MeshRenderer>().material.color    = new Color(0.85f, 0.45f, 0.05f);
            _reticle.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _reticle.GetComponent<MeshRenderer>().receiveShadows    = false;
            _reticle.name                                           = "GazeReticle";
            _reticle.SetActive(ShowReticle);
        }

        public void UpdatePosition(Vector3 position) {
            _reticle.transform.position = position;
            _reticle.SetActive(ShowReticle);
            _reticle.transform.localScale = Vector3.one * ReticleSize;
        }
    }
}