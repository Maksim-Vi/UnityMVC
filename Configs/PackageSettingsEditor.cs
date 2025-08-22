using System.IO;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Code.MVC.Configs
{
    public class PackageSettingsWindow : EditorWindow
    {
        private PackageSettings _settings;
        private string _settingsFolderPath;
        private string _packageRoot;
        private string _manualPackagePath = "";

        [MenuItem("MVC/Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageSettingsWindow>("Package Settings");
            window.Show();
        }

        private void OnEnable()
        {
            // Знаходимо шлях до скрипта
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            var packageInfo = PackageInfo.FindForAssetPath(scriptPath);

            if (packageInfo != null)
            {
                _packageRoot = packageInfo.resolvedPath;
            }
            else
            {
                // Якщо не знайдено, залишаємо порожній шлях для ручного введення
                _packageRoot = "";
            }

            // Попередньо формуємо шлях до MVCSettings (ще не створюємо)
            UpdateSettingsFolderPath();
        }

        private void UpdateSettingsFolderPath()
        {
            if (!string.IsNullOrEmpty(_packageRoot))
            {
                _settingsFolderPath = Path.Combine(_packageRoot, "MVCSettings");
            }
            else if (!string.IsNullOrEmpty(_manualPackagePath))
            {
                _settingsFolderPath = Path.Combine(_manualPackagePath, "MVCSettings");
            }
            else
            {
                _settingsFolderPath = Path.Combine("Assets", "MVCSettings");
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("MVC Package Settings", EditorStyles.boldLabel);

            // Поточний шлях до пакета
            EditorGUILayout.LabelField("Selected path for MVCSettings:", _settingsFolderPath);

            // Поле для ручного вводу шляху, якщо не підходить автоматичний
            EditorGUILayout.BeginHorizontal();
            _manualPackagePath = EditorGUILayout.TextField("Manual Package Path", _manualPackagePath);
            if (GUILayout.Button("Set Path"))
            {
                if (!string.IsNullOrEmpty(_manualPackagePath))
                {
                    _packageRoot = _manualPackagePath;
                    UpdateSettingsFolderPath();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Manual package path cannot be empty.", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Поля для налаштувань (тимчасово редагуємо у пам’яті)
            if (_settings == null)
            {
                _settings = ScriptableObject.CreateInstance<PackageSettings>();
            }

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
            // Перевіряємо і створюємо всі потрібні папки до MVCSettings
            EnsureSettingsFolderExists(_settingsFolderPath);

            // Формуємо шлях до asset
            string assetPath = Path.Combine(_settingsFolderPath, "PackageSettings.asset").Replace("\\", "/");

            // Створюємо asset, якщо його ще немає
            var loadedSettings = AssetDatabase.LoadAssetAtPath<PackageSettings>(assetPath);
            if (loadedSettings == null)
            {
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            // Застосовуємо define symbols
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

// Метод для перевірки та створення всіх проміжних папок до MVCSettings
        private void EnsureSettingsFolderExists(string fullPath)
        {
            fullPath = fullPath.Replace("\\", "/");
            string[] folders = fullPath.Split('/');
            string currentPath = folders[0]; // наприклад "Assets" або корінь пакета

            for (int i = 1; i < folders.Length; i++)
            {
                string nextPath = currentPath + "/" + folders[i];

                // Перевіряємо чи папка існує, якщо ні — створюємо
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
