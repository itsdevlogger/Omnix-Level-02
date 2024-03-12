using System.Collections.Generic;
using UnityEngine;

namespace MenuManagement.Perception
{
    public class PopupMenuManger : MonoBehaviour
    {
        private static PopupMenuManger instance;

        public static PopupMenuManger Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("[PopupMenuManger]");
                    instance = go.AddComponent<PopupMenuManger>();
                    DontDestroyOnLoad(go);
                }

                return instance;
            }
        }

        private List<PopupMenuSettings> allMenus = new List<PopupMenuSettings>();

        private void Update()
        {
            foreach (PopupMenuSettings menu in allMenus)
            {
                menu.UpdateCallback();
            }
        }
        
        internal void Add(PopupMenuSettings settings)
        {
            allMenus.Add(settings);
        }

        internal void Remove(PopupMenuSettings menu)
        {
            allMenus.Remove(menu);
        }
    }
}