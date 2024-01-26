//https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Attribute that require implementation of the provided interface.
/// </summary>
public class RequireInterfaceAttribute : PropertyAttribute
{
    // Interface type.
    public System.Type requiredType { get; private set; }
    /// <summary>
    /// Requiring implementation of the <see cref="T:RequireInterfaceAttribute"/> interface.
    /// </summary>
    /// <param name="type">Interface type.</param>
    public RequireInterfaceAttribute(System.Type type)
    {
        this.requiredType = type;
    }
}

#if UNITY_EDITOR
/// <summary>
/// Drawer for the RequireInterface attribute.
/// </summary>
[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if this is reference type property.
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            var requiredAttribute = this.attribute as RequireInterfaceAttribute;
            EditorGUI.BeginProperty(position, label, property);
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);
            EditorGUI.EndProperty();
        }
        else
        {
            var previousColor = GUI.color;
            GUI.color = Color.red;
            EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
            GUI.color = previousColor;
        }
    }
}
#endif