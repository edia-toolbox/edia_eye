using Edia;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Gaze;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// The EyeDataClientGazeInteractor class is responsible for managing gaze interaction using Unity's Gaze Interactor.
/// </summary>
/// <remarks>
/// This class tracks the gaze direction of the user, allowing for interactions based on where the user is looking.
/// It leverages Unity's built-in gaze interaction systems to implement gaze-related functionality.
/// </remarks>
[EdiaHeader("EDIA EYE", "`Base Gaze Interactor", "Uses Unity's Gaze Interactor to track the gaze direction of the user.")]
public class EyeDataClientGazeInteractor : MonoBehaviour
{
    XRGazeAssistance gazeAssistance;

    private void Awake() {
        this.transform.SetParent(null); // SHould ALWAYS live in the root to avoid offsets
    }

    void Start() {
        gazeAssistance                = XRManager.Instance.transform.GetComponentInChildren<XRGazeAssistance>();
        gazeAssistance.enabled        = true;
        gazeAssistance.gazeInteractor = this.transform.GetChild(0).GetComponent<XRGazeInteractor>();
    }
}
