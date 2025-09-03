/*using UnityEngine;

namespace Code.MVC.Configs
{
    [CreateAssetMenu(fileName = "PackageSettings", menuName = "MyPackage/Settings")]
    public class PackageSettings : ScriptableObject
    {
        public bool useZenject = true; // головна опція для Zenject
        public string additionalOption = "default"; // можна додавати інші налаштування
    }
}*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.MVC.Configs
{
    [CreateAssetMenu(fileName = "PackageSettings", menuName = "MyPackage/Settings")]
    public class PackageSettings : ScriptableObject
    {
        public bool useZenject = false;
        public string additionalOption = "default";

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Prevent enabling integration if Zenject is not present
            if (useZenject && !ZenjectDetector.IsZenjectPresent())
            {
                Debug.LogWarning("[MVC] Zenject not found in project. Disabling integration.");
                useZenject = false;
            }

            // Mark dirty only. Synchronization is triggered from Editor (Inspector/Window/Startup).
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

#if UNITY_EDITOR
    public static class ZenjectDetector
    {
        public static bool IsZenjectPresent()
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.GetType("Zenject.DiContainer", throwOnError: false) != null)
                    return true;
            }
            return false;
        }
    }
#endif
}