using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(InspectorHelpBoxAttribute))]
public class InspectorHelpBoxDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var helpBoxAttribute = attribute as InspectorHelpBoxAttribute;
        var helpBoxRect = position;
        helpBoxRect.height = GetHelpBoxHeight(helpBoxAttribute.message);
        
        var propertyRect = position;
        propertyRect.y      += helpBoxRect.height + EditorGUIUtility.standardVerticalSpacing;
        propertyRect.height =  EditorGUI.GetPropertyHeight(property, label, true);
        
        EditorGUI.HelpBox(helpBoxRect, helpBoxAttribute.message, helpBoxAttribute.type);
        EditorGUI.PropertyField(propertyRect, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var   helpBoxAttribute = attribute as InspectorHelpBoxAttribute;
        float helpBoxHeight    = GetHelpBoxHeight(helpBoxAttribute.message);
        float propertyHeight   = EditorGUI.GetPropertyHeight(property, label, true);
        
        return helpBoxHeight + EditorGUIUtility.standardVerticalSpacing + propertyHeight;
    }
    
    private float GetHelpBoxHeight(string message)
    {
        GUIStyle style   = GUI.skin.GetStyle("HelpBox");
        float    width   = EditorGUIUtility.currentViewWidth - 20; // account for margins
        var      content = new GUIContent(message);
        var      height  = style.CalcHeight(content, width);
        return height + 30; // padding
    }
}