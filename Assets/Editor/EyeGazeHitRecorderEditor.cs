using System.Linq;
using UnityEditor;
using UnityEngine;
using Edia.Eye;

[CustomEditor(typeof(EyeGazeHitRecorder))]
public class EyeGazeHitRecorderEditor : UnityEditor.XR.Interaction.Toolkit.Interactables.XRBaseInteractableEditor {
    private SerializedProperty gazeHoverEnterProperty;
    private SerializedProperty gazeHoverExitProperty;
    private SerializedProperty NewGazeHitPosition;
    private SerializedProperty NewUVHitCoordinate;
    private SerializedProperty recordHitPositionsProperty;
    private SerializedProperty recordUVCoordinatesProperty;
    private SerializedProperty ShowDebugRaysProperty;
    
    private bool               XRSimpleInteractableProps = false;
    private Texture2D          _headerBG;
    
    protected override void OnEnable() {
        base.OnEnable(); 

        // Get the local properties
        gazeHoverEnterProperty      = serializedObject.FindProperty("GazeHoverEnter");
        gazeHoverExitProperty       = serializedObject.FindProperty("GazeHoverExit");
        NewGazeHitPosition          = serializedObject.FindProperty("NewGazeHitPosition");
        NewUVHitCoordinate          = serializedObject.FindProperty("NewUVHitCoordinate");
        recordHitPositionsProperty  = serializedObject.FindProperty("RecordLocalRayHits");
        recordUVCoordinatesProperty = serializedObject.FindProperty("RecordUVCoordinates");
        ShowDebugRaysProperty       = serializedObject.FindProperty("ShowDebugRays");
    }

    public override void OnInspectorGUI() {
        
        // Big ass workaround to show the Edia header
        var headerAttribute = (EdiaHeaderAttribute)target.GetType().GetCustomAttributes(typeof(EdiaHeaderAttribute), true).FirstOrDefault();
        if (headerAttribute != null) {
            _headerBG = Resources.Load<Texture2D>("Icons/EdiaHeader");
            DrawHeader(headerAttribute);
        }

        serializedObject.Update();

        // Hit Recording section
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(recordHitPositionsProperty, true);
        EditorGUILayout.PropertyField(recordUVCoordinatesProperty, true);
        EditorGUILayout.PropertyField(ShowDebugRaysProperty, true);
       
        EditorGUILayout.Space(5);
        
        // Events section
        EditorGUILayout.LabelField("Eye Gaze Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(gazeHoverEnterProperty);
        EditorGUILayout.PropertyField(gazeHoverExitProperty);
        EditorGUILayout.PropertyField(NewGazeHitPosition);
        EditorGUILayout.PropertyField(NewUVHitCoordinate);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(5);

        XRSimpleInteractableProps = EditorGUILayout.Foldout(XRSimpleInteractableProps, "XRSimpleInteractable Properties", true, EditorStyles.foldoutHeader);
        if (XRSimpleInteractableProps) {
            EditorGUILayout.LabelField("XRSimpleInteractable Properties", EditorStyles.boldLabel);
            DrawBaseInspector();
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawBaseInspector() {
        // Draw the default inspector excluding properties we're handling explicitly
        DrawPropertiesExcluding(serializedObject,
            "GazeHoverEnter",
            "GazeHoverExit",
            "GazeNewHitPosition",
            "NewUVHitCoordinate",
            "RecordLocalRayHits",
            "RecordUVCoordinates",
            "m_Script"); 
    }
    
    private void DrawHeader(EdiaHeaderAttribute header) {

        float headerHeight = 120;

        var labelStyle = new GUIStyle {
            fontSize  = 36,
            alignment = TextAnchor.UpperLeft,
            font      = Resources.Load<Font>("Fonts/Bahnschrift-BoldSemiCondensed"),
            normal    = { textColor = Edia.Constants.EdiaColors["white"] }
        };

        var subLabelStyle = new GUIStyle {
            fontSize  = 22,
            alignment = TextAnchor.LowerRight,
            font      = Resources.Load<Font>("Fonts/Bahnschrift-Condensed"),
            normal    = { textColor = Edia.Constants.EdiaColors["grey"] }
        };

        var descriptionStyle = new GUIStyle {
            fontSize = 12,
            wordWrap = true,
            font     = Resources.Load<Font>("Fonts/Bahnschrift-Regular"),
            normal   = { textColor = Edia.Constants.EdiaColors["grey"] }
        };

        // Full rect
        Rect rect = EditorGUILayout.GetControlRect(false, headerHeight);
        
        // BG Texture
        
        GUI.Box(new Rect(0, 0, 1000, 100), GUIContent.none, new GUIStyle { normal = { background = _headerBG } }); 
        GUI.color = Color.clear;

        // Icon
        string iconName = "IconEdia";
        if (header.Title.ToUpper().Contains("EYE"))
            iconName = "IconEye";
        else if (header.Title.ToUpper().Contains("LSL"))
            iconName = "IconLSL";
        else if (header.Title.ToUpper().Contains("STREAM"))
            iconName = "IconStreamer";
        else if (header.Title.ToUpper().Contains("RCAS"))
            iconName = "IconRCAS";

        Texture2D iconTexture = Resources.Load<Texture2D>($"Icons/{iconName}");
        EditorGUI.DrawTextureTransparent(new Rect(18, 20, 60, 60), iconTexture, ScaleMode.ScaleToFit);
        GUI.color = Color.white;

        // Title
        EditorGUI.LabelField(new Rect(90, 34, 200, 40), header.Title, labelStyle);

        // Subtitle
        float textWidth = subLabelStyle.CalcSize(new GUIContent(header.Subtitle)).x;
        EditorGUI.LabelField(new Rect(rect.width - textWidth - 22 + rect.x, 70, textWidth + 20, 22), header.Subtitle, subLabelStyle);

        // Line
        Rect lineRect = new Rect(rect.x, rect.y + headerHeight - 24, rect.width, 1);
        EditorGUI.DrawRect(lineRect, Edia.Constants.EdiaColors["grey"]);
        
        // Description
        EditorGUI.LabelField(new Rect(18, rect.height - 14, rect.width, 22), header.Description, descriptionStyle);
    }
}