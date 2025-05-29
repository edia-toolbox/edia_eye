using UnityEditor;
using UnityEngine;

/// <summary>
/// A class to handle eye gaze interaction feedback by changing object colors when hovered over.
/// </summary>
/// <remarks>
/// This class is designed to provide visual feedback for object interactions based on eye gaze.
/// The default behavior changes the color of an object when it is hovered over or exited using specific hover events.
/// </remarks>
public class EyeGazeResponsExample : MonoBehaviour
{
    [InspectorHelpBox("Highlight color for the gaze hovered object.", MessageType.Info)]
    public Color HoverColor = Color.green;
    private Color _defaultColor;
    
    private void Awake() {
        _defaultColor = GetComponent<Renderer>().material.color;    
    }

    public void OnHoverEnter(GameObject obj) {
        obj.GetComponent<Renderer>().material.color = HoverColor;
    }
    
    public void OnHoverExit(GameObject obj) {
        obj.GetComponent<Renderer>().material.color = _defaultColor;
    }
}
