using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float rotationSpeed = 1f;
    
    private float timeElapsed;
    
    private void Update()
    {
        timeElapsed += Time.deltaTime * rotationSpeed;
        float normalizedSin = (Mathf.Sin(timeElapsed) + 1f) * 0.5f;
        float currentAngle = Mathf.Lerp(minAngle, maxAngle, normalizedSin);
        transform.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
    }
}
