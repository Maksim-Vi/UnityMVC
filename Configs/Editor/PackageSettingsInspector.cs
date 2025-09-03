#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Code.MVC.Configs
{
    [CustomEditor(typeof(PackageSettings))]
    public class PackageSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var useZenjectProp = serializedObject.FindProperty("useZenject");
            var additionalProp = serializedObject.FindProperty("additionalOption");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(useZenjectProp);
            EditorGUILayout.PropertyField(additionalProp);
            bool changed = EditorGUI.EndChangeCheck();

            if (changed)
            {
                serializedObject.ApplyModifiedProperties();

                var s = (PackageSettings)target;

                if (s.useZenject && !ZenjectDetector.IsZenjectPresent())
                {
                    Debug.LogWarning("[MVC] Zenject not found in project. Disabling integration.");
                    s.useZenject = false;
                    EditorUtility.SetDirty(s);
                }

                // Safe to call directly here
                MVCDefineSynchronizer.SyncFromSettings(s);
            }

            EditorGUILayout.HelpBox(
                ZenjectDetector.IsZenjectPresent()
                    ? "Zenject detected in project."
                    : "Zenject not found in project.",
                MessageType.Info);
        }
    }
}
#endif