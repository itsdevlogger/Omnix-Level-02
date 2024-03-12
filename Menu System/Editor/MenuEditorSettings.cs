using System;
using System.IO;
using Omnix.Editor;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    public class MenuEditorSettings : ScriptableObject
    {
        [Serializable]
        private class ItemInfo
        {
            [Tooltip("Name used in script template")] public string name;
            [Tooltip("Name displayed in script maker window")] public string displayName;
            [Tooltip("Tooltip displayed in script maker window (when mouse over this field)")] public string tooltip;
            [Tooltip("Default in the script maker window (User can toggle this on or off)")] public bool defaultValue;
        }
        
        private static string StorageName => "Menu Editor Settings";
        private static MenuEditorSettings INSTANCE;
        [SerializeField] private ItemInfo[] _compileVariables;

        public static MenuEditorSettings Instance
        {
            get
            {
                if (INSTANCE == null) LoadOrCreateStorage();
                return INSTANCE;
            }
        }

        private static void LoadOrCreateStorage()
        {
            INSTANCE = Resources.Load<MenuEditorSettings>(StorageName);
            if (INSTANCE != null) return;

            string resourcesFolderPath = "Assets/Resources/";
            string storagePath = resourcesFolderPath + StorageName + ".asset";
            if (!Directory.Exists(resourcesFolderPath))
            {
                Directory.CreateDirectory(resourcesFolderPath);
                AssetDatabase.ImportAsset(resourcesFolderPath);
            }

            INSTANCE = CreateInstance<MenuEditorSettings>();
            if (INSTANCE == null)
            {
                Debug.LogError("Not able to load Menu Editor Storage. Menu Maker wont work without it. Try restarting.");
                return;
            }

            AssetDatabase.CreateAsset(INSTANCE, storagePath);
            AssetDatabase.ImportAsset(storagePath);
            EditorUtility.SetDirty(INSTANCE);
        }

        public static string GetMenuClassTemplate()
        {
            var asset = Resources.Load<TextAsset>("MenuClass");
            if (asset != null) return asset.text;
            
            Debug.LogError("Unable to find MenuClass.txt file in Resources");
            return "";

        }
        
        public static string GetPrefabClassTemplate()
        {
            var asset = Resources.Load<TextAsset>("PrefabClass");
            if (asset != null) return asset.text;
            
            Debug.LogError("Unable to find PrefabClass.txt file in Resources");
            return "";

        }

        [MenuItem(OmnixMenu.STORAGE_MENU + "Dynamic Menu Storage")]
        private static void SelectStorage()
        {
            EditorGUIUtility.PingObject(Instance);
            Selection.activeObject = Instance;
        }

        public CompilePair[] GetCompileVariables()
        {
            CompilePair[] variables = new CompilePair[_compileVariables.Length];
            for (int i = 0; i < _compileVariables.Length; i++)
            {
                ItemInfo ii = _compileVariables[i];
                variables[i] = new CompilePair()
                {
                    name = ii.name,
                    content = new GUIContent(ii.displayName, ii.tooltip),
                    value = ii.defaultValue
                };
            }
            return variables;
        }
    }
}