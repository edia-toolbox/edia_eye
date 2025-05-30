using UnityEngine;

namespace Edia.Eye {
    public class EyeGazeRecticleRenderer : MonoBehaviour {

        public  bool       ShowRecticle = false;
        public  float      RecticleSize = 0.1f;
        
        private GameObject _recticle;

        private void Awake() {
            GenerateRecticle();
        }

        private void GenerateRecticle() {
            _recticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(_recticle.GetComponent<SphereCollider>());
            _recticle.transform.localScale                           = Vector3.one * RecticleSize;
            _recticle.GetComponent<MeshRenderer>().material.color    = new Color(0.85f, 0.45f, 0.05f);
            _recticle.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _recticle.GetComponent<MeshRenderer>().receiveShadows    = false;
            _recticle.name                                           = "GazeRecticle";
            _recticle.SetActive(ShowRecticle);
        }

        public void UpdatePosition(Vector3 position) {
            _recticle.transform.position = position;
        }
    }
}