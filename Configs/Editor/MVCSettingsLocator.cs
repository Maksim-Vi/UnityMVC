#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Code.MVC.Configs
{
    public static class MVCSettingsLocator
    {
        private const string DefaultFolder = "Assets/MVCSettings";
        private const string DefaultAssetPath = DefaultFolder + "/PackageSettings.asset";

        public static PackageSettings FindOrCreate()
        {
            // Find existing asset
            var guids = AssetDatabase.FindAssets("t:PackageSettings");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                var s = AssetDatabase.LoadAssetAtPath<PackageSettings>(path);
                if (s != null) return s;
            }

            // Create default asset
            EnsureFolder(DefaultFolder);
            var created = ScriptableObject.CreateInstance<PackageSettings>();
            AssetDatabase.CreateAsset(created, DefaultAssetPath);
            AssetDatabase.SaveAssets();
            return created;
        }

        private static void EnsureFolder(string fullPath)
        {
            fullPath = fullPath.Replace("\\", "/");
            var parts = fullPath.Split('/');
            var current = parts[0]; // "Assets"
            for (int i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }

    [InitializeOnLoad]
    public static class MVCDefineSynchronizer
    {
        private const string Symbol = "MVC_USE_ZENJECT";

        static MVCDefineSynchronizer()
        {
            // Sync on editor load
            EditorApplication.delayCall += SyncFromSettings;

#if UNITY_2019_1_OR_NEWER
            // Resync when active build target changes
            EditorUserBuildSettings.activeBuildTargetChanged += OnActiveBuildTargetChanged;
#endif
        }

        private static void OnActiveBuildTargetChanged()
        {
            EditorApplication.delayCall += SyncFromSettings;
        }

        public static void SyncFromSettings()
        {
            var s = MVCSettingsLocator.FindOrCreate();
            SyncFromSettings(s);
        }

        public static void SyncFromSettings(PackageSettings settings)
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (group == BuildTargetGroup.Unknown) return;

            SetDefineForGroup(group, Symbol, settings != null && settings.useZenject);
        }

        private static void SetDefineForGroup(BuildTargetGroup group, string symbol, bool enable)
        {
#if UNITY_2021_2_OR_NEWER
            UnityEditor.Build.NamedBuildTarget named;
            try
            {
                named = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group);
            }
            catch
            {
                // Skip invalid/legacy groups
                return;
            }

            string defines = PlayerSettings.GetScriptingDefineSymbols(named);
#else
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
#endif
            var set = new HashSet<string>(
                defines.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(d => d.Trim())
                       .Where(d => !string.IsNullOrEmpty(d)));

            bool changed = enable ? set.Add(symbol) : set.Remove(symbol);
            if (!changed) return;

            var newDefines = string.Join(";", set);
#if UNITY_2021_2_OR_NEWER
            PlayerSettings.SetScriptingDefineSymbols(named, newDefines);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, newDefines);
#endif
        }
    }

    internal class PackageSettingsChangeWatcher : AssetPostprocessor
    {
        // Schedule sync after asset changes/imports. No SaveAssets here.
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] movedTo, string[] movedFrom)
        {
            bool touched = imported.Any(IsSettings) || movedTo.Any(IsSettings);
            if (touched)
                EditorApplication.delayCall += MVCDefineSynchronizer.SyncFromSettings;
        }

        private static bool IsSettings(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<PackageSettings>(path);
            return obj != null;
        }
    }

    public static class ZenjectDetector
    {
        public static bool IsZenjectPresent()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.GetType("Zenject.DiContainer", throwOnError: false) != null)
                    return true;
            }
            return false;
        }
    }
}
#endif