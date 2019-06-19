using UnityEditor;
using UnityEditor.UI;

namespace LeTai.Asset.TranslucentImage.Editor
{
    [CustomEditor(typeof(TranslucentImage))]
    [CanEditMultipleObjects]
    public class TranslucentImageEditor : ImageEditor
    {
        SerializedProperty spriteBlending;
        SerializedProperty source;
        SerializedProperty vibrancy;
        SerializedProperty brightness;
        SerializedProperty flatten;

        protected override void OnEnable()
        {
            base.OnEnable();

            source         = serializedObject.FindProperty("source");
            spriteBlending = serializedObject.FindProperty("spriteBlending");
            vibrancy       = serializedObject.FindProperty("vibrancy");
            brightness     = serializedObject.FindProperty("brightness");
            flatten        = serializedObject.FindProperty("flatten");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(source);
            EditorGUILayout.PropertyField(spriteBlending);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shared settings", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.PropertyField(vibrancy);
            EditorGUILayout.PropertyField(brightness);
            EditorGUILayout.PropertyField(flatten);
            serializedObject.ApplyModifiedProperties();
        }
    }
}