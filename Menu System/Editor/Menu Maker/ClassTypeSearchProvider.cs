using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace MenuManagement.Editor
{
    public class ClassTypeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private Action<Type> onSelect;
        private static List<SearchTreeEntry> entries;

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Type entryData = (Type)searchTreeEntry.userData;
            onSelect(entryData);
            return true;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            return entries;
        }

        public static void PopulateEntries()
        {
            entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Select Class"), 0));
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyFullName = assembly.FullName.ToLower();
                if (assemblyFullName.StartsWith("unity") || assemblyFullName.StartsWith("system") || assemblyFullName.Contains("editor") || assemblyFullName.Contains("mscorlib"))
                {
                    continue;
                }

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || type.IsEnum)
                    {
                        continue;
                    }

                    var entry = new SearchTreeEntry(new GUIContent(type.Name, type.FullName))
                    {
                        userData = type,
                        level = 1
                    };
                    entries.Add(entry);
                }
            }
        }

        public static void CreatePopup(Action<Type> onSelect)
        {
            var current = CreateInstance<ClassTypeSearchProvider>();
            current.onSelect = onSelect;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), current);
        }
    }
}