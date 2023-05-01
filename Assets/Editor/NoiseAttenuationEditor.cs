using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseAttenuationData), true), CanEditMultipleObjects]
public class NoiseAttenuationDataEditor : Editor
{
    public SerializedProperty
        Type_Prop,
        EquatorAttenuation_Prop,
        PoleAttenuation_Prop,
        Amplitude_Prop,
        Frequency_Prop,
        Offset_Prop,
        AsymptoteCutoff_Prop;

    void OnEnable()
    {
        // Setup the SerializedProperties
        EquatorAttenuation_Prop = serializedObject.FindProperty("EquatorAttenuation");
        PoleAttenuation_Prop = serializedObject.FindProperty("PoleAttenuation");
        Amplitude_Prop = serializedObject.FindProperty("Amplitude");
        Frequency_Prop = serializedObject.FindProperty("Frequency");
        Offset_Prop = serializedObject.FindProperty("Offset");
        AsymptoteCutoff_Prop = serializedObject.FindProperty("AsymptoteCutoff");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(Type_Prop);
        AttenutationType type = (AttenutationType)Type_Prop.enumValueIndex;

        if (type == AttenutationType.Sin || type == AttenutationType.Tan)
        {
            EditorGUILayout.Slider(Amplitude_Prop, 0.0f, 1.0f, new GUIContent("Amplitude"));
            EditorGUILayout.Slider(Frequency_Prop, 0.0f, 1.0f, new GUIContent("Frequency"));
            EditorGUILayout.Slider(Offset_Prop, 0.0f, 1.0f, new GUIContent("Offset"));

            if (type == AttenutationType.Tan)
                EditorGUILayout.Slider(AsymptoteCutoff_Prop, 0.1f, 0.25f, new GUIContent("AsymptoteCutoff"));
        }
        else if(type == AttenutationType.Parabola)
        {
            EditorGUILayout.Slider(EquatorAttenuation_Prop, 0.0f, 1.0f, new GUIContent("EquatorAttenuation"));
            EditorGUILayout.Slider(PoleAttenuation_Prop, 0.0f, 1.0f, new GUIContent("PoleAttenuation"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}