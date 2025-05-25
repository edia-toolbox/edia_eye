using UnityEngine;
using UnityEditor;

public class InspectorHelpBoxAttribute : PropertyAttribute
{
    public string      message;
    public MessageType type;

    public InspectorHelpBoxAttribute(string message, MessageType type = MessageType.Info)
    {
        this.message = message;
        this.type    = MessageType.Info;
    }
}