using UnityEngine;

public class EyeRayCaster : MonoBehaviour {
    
    private float      rayDistance = 50f;
    private int        _gazeLayer;
    private RaycastHit _hit;

    private string _targetId = "";
    private Vector3 _intersectionPosition = Vector3.zero;
    
    public string TargetId
    {
        get => _targetId;
        private set => _targetId = value;
    }
    
    public Vector3 IntersectionPosition
    {
        get => _intersectionPosition;
        private set => _intersectionPosition = value;
    }

    private GameObject _recticle;
    public bool ShowRecticle = true;
    
    private void Awake() {
        _gazeLayer = LayerMask.GetMask("GazeCollision");

        GenerateRecticle();
    }

    private void GenerateRecticle() {
        _recticle                                             = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _recticle.transform.localScale                        = Vector3.one * 0.02f;
        _recticle.GetComponent<MeshRenderer>().material.color = new Color(0.17f, 1f, 0f);
        _recticle.transform.SetParent(transform);
        _recticle.GetComponent<MeshRenderer>().enabled = ShowRecticle;
    }

    private void Update() {
        TargetId = "";
        IntersectionPosition = Vector3.zero;

        if (Physics.Raycast(transform.position, transform.forward, out _hit, rayDistance, _gazeLayer)) {
            Debug.DrawRay(transform.position, transform.forward * _hit.distance, Color.green);

            TargetId = _hit.collider.gameObject.name;
            IntersectionPosition = _hit.point;
            _recticle.transform.position = IntersectionPosition;
        }
        else {
            Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);
            _recticle.transform.position = transform.position + transform.forward * rayDistance;
        }
    }
}