using System.Linq;
using UnityEditor;
using UnityEngine;
using Edia.Eye;

[CustomEditor(typeof(EyeGazeHitRegistration))]
public class EyeGazeHitRegistrationEditor : UnityEditor.XR.Interaction.Toolkit.Interactables.XRBaseInteractableEditor {
    private SerializedProperty gazeHoverEnterProperty;
    private SerializedProperty gazeHoverExitProperty;
    private SerializedProperty NewGazeHitPosition;
    private SerializedProperty NewUVHitCoordinate;
    private SerializedProperty NewHit;
    private SerializedProperty ShowDebugRaysProperty;

    private Texture2D           iconTexture;
    private bool                XRSimpleInteractableProps = false;
    private Texture2D           _headerBG;
    private EdiaHeaderAttribute _headerAttribute;
    private GUIStyle            labelStyle;
    private GUIStyle            subLabelStyle;
    private GUIStyle            descriptionStyle;

    protected override void OnEnable() {
        base.OnEnable();

        try {
            // Initialize the header resources
            if (target != null) {
                _headerAttribute = (EdiaHeaderAttribute)target.GetType().GetCustomAttributes(typeof(EdiaHeaderAttribute), true).FirstOrDefault();
                if (_headerAttribute != null) {
                    _headerBG = Resources.Load<Texture2D>("Icons/EdiaHeader");

                    // Icon
                    string iconName = "IconEdia";
                    if (_headerAttribute.Title != null && _headerAttribute.Title.ToUpper().Contains("EYE"))
                        iconName = "IconEye";
                    else if (_headerAttribute.Title != null && _headerAttribute.Title.ToUpper().Contains("LSL"))
                        iconName = "IconLSL";
                    else if (_headerAttribute.Title != null && _headerAttribute.Title.ToUpper().Contains("STREAM"))
                        iconName = "IconStreamer";
                    else if (_headerAttribute.Title != null && _headerAttribute.Title.ToUpper().Contains("RCAS"))
                        iconName = "IconRCAS";

                    iconTexture = Resources.Load<Texture2D>($"Icons/{iconName}");
                }
            }

            // Get the local properties
            if (serializedObject != null) {
                gazeHoverEnterProperty = serializedObject.FindProperty("GazeHoverEnter");
                gazeHoverExitProperty  = serializedObject.FindProperty("GazeHoverExit");
                NewGazeHitPosition     = serializedObject.FindProperty("NewHitLocalPosition");
                NewUVHitCoordinate     = serializedObject.FindProperty("NewHitUV");
                NewHit                 = serializedObject.FindProperty("NewHit");
                ShowDebugRaysProperty  = serializedObject.FindProperty("ShowDebugRays");
            }

            // Initialize styles - only create them, don't use GUILayout methods
            labelStyle = new GUIStyle {
                fontSize  = 36,
                alignment = TextAnchor.UpperLeft,
                font      = Resources.Load<Font>("Fonts/Bahnschrift-BoldSemiCondensed"),
                normal    = { textColor = Edia.Constants.EdiaColors["white"] }
            };

            subLabelStyle = new GUIStyle {
                fontSize  = 22,
                alignment = TextAnchor.LowerRight,
                font      = Resources.Load<Font>("Fonts/Bahnschrift-Condensed"),
                normal    = { textColor = Edia.Constants.EdiaColors["grey"] }
            };

            descriptionStyle = new GUIStyle {
                fontSize = 12,
                wordWrap = true,
                font     = Resources.Load<Font>("Fonts/Bahnschrift-Regular"),
                normal   = { textColor = Edia.Constants.EdiaColors["grey"] }
            };
        }
        catch (System.Exception ex) {
            Debug.LogError($"Error in OnEnable: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public override void OnInspectorGUI() {
        try {
            // Always call Update
            serializedObject.Update();

            // Draw header at the beginning of OnInspectorGUI where GUI methods are valid
            if (_headerAttribute != null && _headerBG != null) {
                DrawHeader(_headerAttribute);
            }

            // Hit Recording section
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            if (ShowDebugRaysProperty != null) {
                EditorGUILayout.PropertyField(ShowDebugRaysProperty, true);
            }

            EditorGUILayout.Space(5);

            // Events section
            EditorGUILayout.LabelField("Eye Gaze Events", EditorStyles.boldLabel);
            if (gazeHoverEnterProperty != null) {
                EditorGUILayout.PropertyField(gazeHoverEnterProperty);
            }

            if (gazeHoverExitProperty != null) {
                EditorGUILayout.PropertyField(gazeHoverExitProperty);
            }

            if (NewGazeHitPosition != null) {
                EditorGUILayout.PropertyField(NewGazeHitPosition);
            }

            if (NewUVHitCoordinate != null) {
                EditorGUILayout.PropertyField(NewUVHitCoordinate);
            }

            if (NewHit != null) {
                EditorGUILayout.PropertyField(NewHit);
            }

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
        catch (System.Exception ex) {
            EditorGUILayout.HelpBox($"Error in inspector: {ex.Message}", MessageType.Error);
            Debug.LogError($"Error in OnInspectorGUI: {ex.Message}\n{ex.StackTrace}");
        }
    }

    protected virtual void DrawBaseInspector() {
        if (serializedObject != null) {
            // Draw the default inspector excluding properties we're handling explicitly
            DrawPropertiesExcluding(serializedObject,
                "GazeHoverEnter",
                "GazeHoverExit",
                "NewHitLocalPosition",
                "NewHitUV",
                "NewHit",
                "m_Script");
        }
    }

    private void DrawHeader(EdiaHeaderAttribute header) {
        try {
            // Full rect
            Rect rect = EditorGUILayout.GetControlRect(false, 120);

            // BG Texture
            if (_headerBG != null) {
                GUI.Box(new Rect(0, 0, 1000, 100), GUIContent.none, new GUIStyle { normal = { background = _headerBG } });
            }

            GUI.color = Color.clear;

            if (iconTexture != null) {
                EditorGUI.DrawTextureTransparent(new Rect(18, 20, 60, 60), iconTexture, ScaleMode.ScaleToFit);
            }

            GUI.color = Color.white;

            // Title
            if (header.Title != null && labelStyle != null) {
                EditorGUI.LabelField(new Rect(90, 34, 200, 40), header.Title, labelStyle);
            }

            // Subtitle
            if (header.Subtitle != null && subLabelStyle != null) {
                float textWidth = subLabelStyle.CalcSize(new GUIContent(header.Subtitle)).x;
                EditorGUI.LabelField(new Rect(rect.width - textWidth - 22 + rect.x, 70, textWidth + 20, 22), header.Subtitle, subLabelStyle);
            }

            // Line
            if (Edia.Constants.EdiaColors.ContainsKey("grey")) {
                Rect lineRect = new Rect(rect.x, rect.y + 120 - 24, rect.width, 1);
                EditorGUI.DrawRect(lineRect, Edia.Constants.EdiaColors["grey"]);
            }

            // Description
            if (header.Description != null && descriptionStyle != null) {
                EditorGUI.LabelField(new Rect(18, rect.height - 14, rect.width, 22), header.Description, descriptionStyle);
            }
        }
        catch (System.Exception ex) {
            Debug.LogError($"Error in DrawHeader: {ex.Message}\n{ex.StackTrace}");
        }
    }
}