#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Code.MVC.Configs
{
    public class PackageSettingsWindow : EditorWindow
    {
        private PackageSettings _settings;
        private string _manualPackagePath = "";
        private string _settingsFolderPath;

        [MenuItem("MVC/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageSettingsWindow>("Package Settings");
            window.Show();
        }

        private void OnEnable()
        {
            UpdateSettingsFolderPath();
        }

        private void UpdateSettingsFolderPath()
        {
            if (!string.IsNullOrEmpty(_manualPackagePath))
            {
                // Користувацький шлях + MVCSettings
                _settingsFolderPath = Path.Combine(_manualPackagePath, "MVCSettings");
            }
            else
            {
                // Дефолтний шлях
                _settingsFolderPath = Path.Combine("Assets", "MVCSettings");
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("MVC Package Settings", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Current path for MVCSettings:", _settingsFolderPath);

            EditorGUILayout.BeginHorizontal();
            _manualPackagePath = EditorGUILayout.TextField("Manual Package Path", _manualPackagePath);

            if (GUILayout.Button("Set Path"))
            {
                if (!string.IsNullOrEmpty(_manualPackagePath))
                    UpdateSettingsFolderPath();
                else
                    EditorUtility.DisplayDialog("Error", "Manual package path cannot be empty.", "OK");
            }

            if (GUILayout.Button("Reset to Default"))
            {
                _manualPackagePath = "";
                UpdateSettingsFolderPath();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (_settings == null)
                _settings = ScriptableObject.CreateInstance<PackageSettings>();

            _settings.useZenject = EditorGUILayout.Toggle("Use Zenject", _settings.useZenject);
            _settings.additionalOption = EditorGUILayout.TextField("Additional Option", _settings.additionalOption);

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Settings"))
            {
                ApplySettings(_settings);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Delete MVCSettings Folder"))
            {
                if (EditorUtility.DisplayDialog("Delete MVCSettings Folder",
                    "Are you sure you want to delete the MVCSettings folder and all its contents?", "Yes", "No"))
                {
                    DeleteSettingsFolder();
                }
            }
        }

        private void ApplySettings(PackageSettings settings)
        {
            // Перевіряємо та створюємо всю структуру папок до MVCSettings
            EnsureFolderStructureExists(_settingsFolderPath);

            string assetPath = Path.Combine(_settingsFolderPath, "PackageSettings.asset").Replace("\\", "/");

            var loadedSettings = AssetDatabase.LoadAssetAtPath<PackageSettings>(assetPath);
            if (loadedSettings == null)
            {
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            // Define symbols
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (settings.useZenject)
            {
                if (!defineSymbols.Contains("ZENJECT"))
                    defineSymbols += ";ZENJECT";
            }
            else
            {
                defineSymbols = defineSymbols.Replace("ZENJECT", "");
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defineSymbols);
            EditorUtility.SetDirty(settings);

            Debug.Log("[PackageSettings] Settings applied at: " + _settingsFolderPath + "\nCurrent define symbols: " + defineSymbols);
        }

        // Метод створює всі папки по черзі, якщо їх нема
        private void EnsureFolderStructureExists(string fullPath)
        {
            fullPath = fullPath.Replace("\\", "/");
            string[] folders = fullPath.Split('/');

            string currentPath = folders[0]; // зазвичай "Assets"
            for (int i = 1; i < folders.Length; i++)
            {
                string nextPath = currentPath + "/" + folders[i];

                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }

                currentPath = nextPath;
            }
        }

        private void DeleteSettingsFolder()
        {
            if (AssetDatabase.DeleteAsset(_settingsFolderPath))
            {
                AssetDatabase.Refresh();
                Debug.Log("[PackageSettings] MVCSettings folder deleted successfully.");
            }
            else
            {
                Debug.LogError("[PackageSettings] Failed to delete MVCSettings folder.");
            }
        }
    }
}
#endif