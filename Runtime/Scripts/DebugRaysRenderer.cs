using UnityEngine;
using System.Collections.Generic;

namespace Edia.Eye {
    public class DebugRaysRenderer : MonoBehaviour {

        [SerializeField] private int   maxRayCount = 30;
        [SerializeField] private float rayDuration = 3.0f;
        [SerializeField] private bool  fadeRays    = true;

        [System.Serializable]
        public class RayInfo {
            public Vector3 Start;
            public Vector3 Direction;
            public Color   OriginalColor;
            public float   StartTime;
            public float   EndTime;

            public RayInfo(Vector3 start, Vector3 direction, Color color, float startTime, float endTime) {
                Start         = start;
                Direction     = direction;
                OriginalColor = color;
                StartTime     = startTime;
                EndTime       = endTime;
            }

            public Color GetCurrentColor() {
                float timeLeft  = EndTime - Time.time;
                float totalTime = EndTime - StartTime;
                float alpha     = timeLeft / totalTime;

                return new Color(
                    OriginalColor.r,
                    OriginalColor.g,
                    OriginalColor.b,
                    OriginalColor.a * alpha
                );
            }
        }

        public Queue<RayInfo> rays = new Queue<RayInfo>();
        public List<RayInfo>  keptRays;
        
        public void AddRay(Vector3 start, Vector3 direction) {
            float startTime      = Time.time;
            float endTime        = startTime + rayDuration;

            rays.Enqueue(new RayInfo(start, direction, Color.red, startTime, endTime));

            if (rays.Count > maxRayCount) {
                rays.Dequeue();
            }
        }

        private static void DrawRayLocal(Transform transform, Vector3 localStart, Vector3 localDirection, Color color)
        {
            Vector3 worldStart = transform.TransformPoint(localStart);
            // Vector3 worldDirection = transform.TransformDirection(localDirection);
            Debug.DrawRay(worldStart, localDirection, color);
        }
        
        private void Update() {
            keptRays = new List<RayInfo>();

            while (rays.Count > 0) {
                RayInfo ray = rays.Dequeue();
                if (ray.EndTime > Time.time) {
                    Color currentColor = fadeRays ? ray.GetCurrentColor() : ray.OriginalColor;
                    DrawRayLocal(transform, ray.Start, ray.Direction / 4, currentColor);
                    // Debug.DrawRay(ray.Start, ray.Direction * 2, currentColor);
                    keptRays.Add(ray);
                }
            }

            foreach (var ray in keptRays) {
                rays.Enqueue(ray);
            }
        }

    }
}

