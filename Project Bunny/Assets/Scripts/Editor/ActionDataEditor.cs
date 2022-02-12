using AI;
using UnityEditor;

[CustomEditor(typeof(ActionData), true)]
public class GActionDataEditor : UnityEditor.Editor
{
    protected SerializedProperty hasTarget, targetTag, preconditionStates, afterEffectStates;
    protected new SerializedProperty name;

    /// <summary>
    /// Fetches the serializable properties of the GActionData class
    /// </summary>
    protected virtual void OnEnable()
    {
        name = serializedObject.FindProperty("name");
        hasTarget = serializedObject.FindProperty("hasTarget");
        targetTag = serializedObject.FindProperty("targetTag");
        preconditionStates = serializedObject.FindProperty("preconditionStates");
        afterEffectStates = serializedObject.FindProperty("afterEffectStates");
    }

    /// <summary>
    /// Updates the value of the properties when one is changed
    /// </summary>
    public override void OnInspectorGUI()
    {
        ShowInspector();
    }

    /// <summary>
    /// Updates the value of the properties when one is changed
    /// </summary>
    protected virtual void ShowInspector()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("DataObject", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(name);
        EditorGUILayout.PropertyField(hasTarget);

        if (target is ActionData {HasTarget: true})
        {
            EditorGUILayout.PropertyField(targetTag);
        }

        EditorGUILayout.PropertyField(preconditionStates);
        EditorGUILayout.PropertyField(afterEffectStates);
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
