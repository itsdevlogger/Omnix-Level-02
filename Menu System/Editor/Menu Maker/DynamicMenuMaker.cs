using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Omnix.Editor;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MenuManagement.Editor
{
    public class DynamicMenuMaker : EditorWindow
    {
        private static readonly HashSet<Type> SUPPORTED_TYPES = new HashSet<Type>()
        {
            typeof(float),
            typeof(int),
            typeof(double),
            typeof(long),
            typeof(short),
            typeof(char),
            typeof(DateTime),
            typeof(string),
            typeof(bool),
            typeof(Sprite)
        };

        private Type _dataClassType;
        private Object _folder;
        private string _prefabClassName;
        private string _menuClassName;
        private List<PropPair> _includedProperties;
        private CompilePair[] _compileVariables;
        private bool _hasCommonObject;
        private Vector2 _propsSelectorPosition;
        private Vector2 _compileSelectorPosition;

        [MenuItem(OmnixMenu.WINDOW_MENU + "Dynamic Menu Script Creator")]
        private static void ShowWindow()
        {
            GetWindow(typeof(DynamicMenuMaker), false, "Dynamic Menu Script Creator");
        }

        private void OnEnable()
        {
            // Debug.Log("s");
            _compileVariables = MenuEditorSettings.Instance.GetCompileVariables();
            ClassTypeSearchProvider.PopulateEntries();
        }

        private string GetErrorMessage()
        {
            if (_dataClassType == null) return "Select a data class";
            if (string.IsNullOrEmpty(_prefabClassName)) return "Select a name for prefab class";
            if (string.IsNullOrEmpty(_menuClassName)) return "Select a menu class";
            if (_folder == null) return "Select a path to create scripts";
            return null;
        }

        private void DrawClassPicker()
        {
            string typeName = (_dataClassType == null) ? "Not Selected" : _dataClassType.FullName;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Data Class Type");
            if (GUILayout.Button(typeName, EditorStyles.toolbarDropDown)) ClassTypeSearchProvider.CreatePopup(SetDataClassType);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFolderField()
        {
            Object folderTemp = EditorGUILayout.ObjectField("Folder", _folder, typeof(Object), false);
            if (folderTemp == _folder) return;

            if (folderTemp == null)
            {
                _folder = null;
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(folderTemp);
            if (File.Exists(folderPath))
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Provided folder is not a directory", "Ok");
                return;
            }

            if (Directory.Exists(folderPath) == false)
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Provided folder is not valid", "Ok");
                return;
            }

            _folder = folderTemp;
        }

        private void DrawPropertiesSelector()
        {
            if (_includedProperties == null) return;
            _propsSelectorPosition = EditorGUILayout.BeginScrollView(_propsSelectorPosition, false, false);
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Included Fields:");
            foreach (PropPair pair in _includedProperties)
            {
                pair.isIncluded = EditorGUILayout.ToggleLeft(pair.content, pair.isIncluded);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawCompileVariables()
        {
            if (_compileVariables == null) return;
            _compileSelectorPosition = EditorGUILayout.BeginScrollView(_compileSelectorPosition, false, false);
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Compile Settings:");
            foreach (CompilePair pair in _compileVariables)
            {
                pair.value = EditorGUILayout.ToggleLeft(pair.content, pair.value);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void DrawCreateButton()
        {
            bool guiEnabled = GUI.enabled;
            string error = GetErrorMessage();
            if (error != null)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                GUI.enabled = false;
            }

            if (GUILayout.Button("Create Dynamic Menu"))
            {
                CreateDynamicMenu();
            }

            GUI.enabled = guiEnabled;
        }

        private void OnGUI()
        {
            DrawClassPicker();
            _prefabClassName = EditorGUILayout.TextField("Prefab Class Type", _prefabClassName);
            _menuClassName = EditorGUILayout.TextField("Menu Class Type", _menuClassName);
            DrawFolderField();
            _hasCommonObject = EditorGUILayout.Toggle("Has Common Object", _hasCommonObject);
            EditorGUILayout.LabelField("Common object can be anything that is necessary for the item prefab.");
            EditorGUILayout.LabelField("This single object instance is shared between all prefab instances, making it inefficient to serialize it in prefab.");
            EditorGUILayout.LabelField("A simple solution can be to store it in menu, and provide it to prefab instance as required.");

            EditorGUILayout.BeginHorizontal();
            DrawCompileVariables();
            DrawPropertiesSelector();
            EditorGUILayout.EndHorizontal();
            DrawCreateButton();
        }

        private void CreateDynamicMenu()
        {
            ScriptGenerator settings = GetScriptSettings();
            Debug.Log(JsonConvert.SerializeObject(settings));
            string menuClassCode = settings.Process(MenuEditorSettings.GetMenuClassTemplate());
            string prefabClassCode = settings.Process(MenuEditorSettings.GetPrefabClassTemplate());

            string folderPath = AssetDatabase.GetAssetPath(_folder);
            string prefabClassPath = Path.Combine(folderPath, _prefabClassName + ".cs");
            string menuClassPath = Path.Combine(folderPath, _menuClassName + ".cs");
            File.WriteAllText(prefabClassPath, prefabClassCode);
            File.WriteAllText(menuClassPath, menuClassCode);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Created files successfully. Let unity compile now.", "Okay");
            EditorUtility.OpenWithDefaultApp(menuClassPath);
        }

        private void GetFieldAndSetupCodes(out string fieldCode, out string setupCode, out string usingCode)
        {
            StringBuilder fields = new StringBuilder();
            StringBuilder setup = new StringBuilder();

            HashSet<string> include = _includedProperties.Where(ip => ip.isIncluded).Select(ip => ip.content.text).ToHashSet();
            FieldInfo[] dataFields = _dataClassType.GetFields();
            foreach (FieldInfo field in dataFields)
            {
                if (field.IsPrivate || field.IsStatic) continue;
                if (include.Contains(field.Name) == false) continue;

                Type fieldType = field.FieldType;
                string holderName = $"{field.Name}Field";
                if (fieldType == typeof(int) || fieldType == typeof(float) || fieldType == typeof(short) || fieldType == typeof(long) || fieldType == typeof(char) || fieldType == typeof(double) || fieldType == typeof(DateTime))
                {
                    fields.AppendLine($"    [SerializeField] private TextMeshProUGUI {holderName};");
                    setup.AppendLine($"        {holderName}.text = data.{field.Name}.ToString();");
                }
                else if (fieldType == typeof(string))
                {
                    fields.AppendLine($"    [SerializeField] private TextMeshProUGUI {holderName};");
                    setup.AppendLine($"        {holderName}.text = data.{field.Name};");
                }
                else if (fieldType == typeof(bool))
                {
                    fields.AppendLine($"    [SerializeField] private Toggle {holderName};");
                    setup.AppendLine($"        {holderName}.isOn = data.{field.Name};");
                }
                else if (fieldType == typeof(Sprite))
                {
                    fields.AppendLine($"    [SerializeField] private Image {holderName};");
                    setup.AppendLine($"        {holderName}.sprite = data.{field.Name};");
                }
            }

            fieldCode = fields.ToString();
            setupCode = setup.ToString();
            usingCode = string.IsNullOrEmpty(_dataClassType.Namespace) ? "" : $"using {_dataClassType.Namespace};";
        }

        private void SetDataClassType(Type type)
        {
            _dataClassType = type;

            string typeName = type.Name;
            if (typeName.EndsWith("Info") || typeName.EndsWith("Data") || typeName.EndsWith("Item")) typeName = typeName.Substring(0, typeName.Length - 4);
            _prefabClassName = $"{typeName}ItemPrefab";
            _menuClassName = $"{typeName}Menu";
            _includedProperties = new List<PropPair>();

            foreach (FieldInfo field in _dataClassType.GetFields())
            {
                if (field.IsPrivate || field.IsStatic) continue;
                Type ft = field.FieldType;
                if (SUPPORTED_TYPES.Contains(ft))
                {
                    _includedProperties.Add(new PropPair() { content = new GUIContent(field.Name), isIncluded = true });
                }
            }
        }

        private ScriptGenerator GetScriptSettings()
        {
            GetFieldAndSetupCodes(out string fieldCode, out string setupCode, out string usingCode);

            var settings = new ScriptGenerator();
            if (!string.IsNullOrEmpty(usingCode)) settings.compileVariables.Add("has_name_space");
            settings.compileVariables.Add("true");
            if (_hasCommonObject) settings.compileVariables.Add("has_common_object");
            else settings.compileVariables.Add("no_common_object");
            settings.compileVariables.UnionWith(_compileVariables.Where(p => p.value).Select(p => p.name));

            settings.placeholderValues.Add("MENU_CLASS", _menuClassName);
            settings.placeholderValues.Add("DATA_CLASS", _dataClassType.Name);
            settings.placeholderValues.Add("PREFAB_CLASS", _prefabClassName);
            settings.placeholderValues.Add("NAME_SPACE", string.IsNullOrEmpty(_dataClassType.Namespace) ? "" : _dataClassType.Namespace);
            settings.placeholderValues.Add("CODE_USING", usingCode);
            settings.placeholderValues.Add("CODE_FIELDS", fieldCode);
            settings.placeholderValues.Add("CODE_SETUP", setupCode);

            return settings;
        }
    }
}