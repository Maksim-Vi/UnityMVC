using UnityEngine;

namespace Code.MVC.Configs
{
    [CreateAssetMenu(fileName = "PackageSettings", menuName = "MyPackage/Settings")]
    public class PackageSettings : ScriptableObject
    {
        public bool useZenject = true; // головна опція для Zenject
        public string additionalOption = "default"; // можна додавати інші налаштування
    }
}