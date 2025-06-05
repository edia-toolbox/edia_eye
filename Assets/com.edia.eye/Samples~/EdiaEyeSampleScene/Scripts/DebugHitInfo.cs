using UnityEngine;

public class DebugHitInfo : MonoBehaviour {
    public void HitInfo(RaycastHit hit) {
        Debug.Log(hit.collider.gameObject.name);
    }
}