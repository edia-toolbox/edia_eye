using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Edia.Eye {
    /// <summary>
    /// Represents an interactable object that supports interaction with eye gaze.
    /// This class extends the XR toolkit's <see cref="XRSimpleInteractable"/> to allow
    /// eye-tracking interactions such as gaze-based hovering.
    /// </summary>
    /// <seealso cref="XRSimpleInteractable"/>
    [RequireComponent(typeof(MeshCollider))]
    [AddComponentMenu("EDIA/Eye/Eye Gaze Interactable")]
    public class EyeGazeInteractable : XRSimpleInteractable {
        [InspectorHelpBox("Gaze Hover Enter & Gaze Hover Exit are EDIA provided event triggers.")]
        public UnityEvent<GameObject> GazeHoverEnter;
        public UnityEvent<GameObject> GazeHoverExit;

        protected override void Awake() {
            base.Awake();

            allowGazeInteraction = true;

            if (colliders.Count == 0) {
                Debug.Log("No collider found, attempting get-component!");
                try {
                    colliders.Add(GetComponent<Collider>());
                }
                catch {
                    Debug.Log("No collider found!");
                }
            }
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args) {
            base.OnHoverEntered(args);
            GazeHoverEnter?.Invoke(this.gameObject);
            XRManager.Instance.AddToConsole($"GazeHoverEnter: {transform.name}");
        }

        protected override void OnHoverExited(HoverExitEventArgs args) {
            base.OnHoverExited(args);
            GazeHoverExit?.Invoke(this.gameObject);
            XRManager.Instance.AddToConsole($"GazeHoverExit: {transform.name}");
        }
    }
}