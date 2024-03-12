using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MenuManagement.Base;
using Omnix.DesignPatterns;
using UnityEngine;
using UnityEngine.UI;

namespace MenuManagement.Perception
{
    [DefaultExecutionOrder(1)]
    [GroupRuntimeConstant("managedMenus")]
    [GroupProperties("Default", "defaultMenuIndex","defaultTransition")]
    public class MenuScreenManager : BaseMenu
    {
        [Serializable]
        public class Wrapper
        {
            public BaseMenu menu;
            public Toggle toggle;
        }

        [SerializeField] private List<Wrapper> managedMenus;
        [SerializeField] private int defaultMenuIndex;
        public BaseTransitionBlendable defaultTransition;

        public Wrapper ActiveMenuWrapper { get; private set; }
        public bool IsActive { get; private set; }
        private bool _ignoreToggleCallbacks = false;

        private TaskQueue queue;

        protected override IEnumerator BeforeLoad() { yield break; }
        protected override IEnumerator AfterUnload() { yield break; }

        protected override void Initialize()
        {
            queue = new TaskQueue();
            onLoad.AddListener(ActivateScreen);
            onUnload.AddListener(DeactivateScreen);
            

            // Bind Toggles
            foreach (Wrapper wrapper in managedMenus)
            {
                wrapper.toggle.onValueChanged.AddListener(value =>
                {
                    if (_ignoreToggleCallbacks) return;
                    queue.BeginTask(Tasks.OnToggleValueChanged, this, wrapper, value);
                });
            }
        }

        private void UpdateToggles()
        {
            _ignoreToggleCallbacks = true;
            foreach (Wrapper wrapper in managedMenus)
            {
                bool value = (wrapper.menu == ActiveMenuWrapper.menu);
                if (wrapper.toggle.isOn != value)
                {
                    wrapper.toggle.isOn = value;
                }
            }
            _ignoreToggleCallbacks = false;
        }
        
        public void ActivateScreen()
        {
            // Get the wrapper
            Wrapper wrapper;
            if (defaultMenuIndex <= 0) wrapper = managedMenus[0];
            else if (defaultMenuIndex >= managedMenus.Count) wrapper = managedMenus[managedMenus.Count - 1];
            else wrapper = managedMenus[defaultMenuIndex];
            queue.BeginTask(Tasks.ActivateScreen, this, wrapper);
        }

        public void ActivateScreen(int index)
        {
            Wrapper wrapper;
            if (index <= 0) wrapper = managedMenus[0];
            else if (index >= managedMenus.Count) wrapper = managedMenus[managedMenus.Count - 1];
            else wrapper = managedMenus[index];
            queue.BeginTask(Tasks.ActivateScreen, this, wrapper);
        }
        
        public void ActivateScreen(BaseMenu menu)
        {
            // Get the wrapper
            Wrapper wrapper = managedMenus.Find(w => w.menu == menu);
            if (wrapper != null) queue.BeginTask(Tasks.ActivateScreen, this, wrapper);
        }

        
        public void DeactivateScreen()
        {
            queue.BeginTask(Tasks.DeactivateScreen, this);
        }

        /// <summary> Load a menu specified by index set in inspector </summary>
        /// <param name="index"> if index out of range, does nothing </param>
        /// <param name="onLoaded"> OnLoad callback</param>
        public void LoadMenu(int index, Action onLoaded = null)
        {
            if (managedMenus.Count == 0) return;
            if (index < 0 || index >= managedMenus.Count) return;
            
            LoadMenu(managedMenus[index].menu, onLoaded);
        }

        /// <summary> Load a menu </summary>
        /// <param name="menu"> If menu is not present in managedMenus list, then does nothing </param>
        /// <param name="onLoaded"> OnLoad callback</param>
        public void LoadMenu([NotNull] BaseMenu menu, Action onLoaded = null)
        {
            foreach (var wrapper in managedMenus)
            {
                if (wrapper.menu == menu)
                {
                    queue.BeginTask(Tasks.LoadMenu, this, wrapper, onLoaded);
                }
            }
        }

        /// <summary> Refresh the screen and reload the active menu </summary>
        public void Refresh()
        {
            if (managedMenus.Count == 0) return;

            Wrapper wrapper = ActiveMenuWrapper;
            if (wrapper == null || wrapper.menu == null) wrapper = managedMenus[0];
            queue.BeginTask<MenuScreenManager, Wrapper, Action>(Tasks.LoadMenu, this, wrapper, null);
        }

        private static class Tasks
        {
            public static void OnToggleValueChanged(MenuScreenManager m, Wrapper wrapper, bool value)
            {
                // user trying to disable active menu
                if (m.ActiveMenuWrapper != null && wrapper.menu == m.ActiveMenuWrapper.menu)
                {
                    m._ignoreToggleCallbacks = true;
                    wrapper.toggle.isOn = true;
                    m._ignoreToggleCallbacks = false;
                    m.queue.TaskDone();
                    return;
                }

                // update only if this menu is is activated to avoid multiple callbacks
                if (value)
                {
                    if (m.ActiveMenuWrapper != null)
                    {
                        m._ignoreToggleCallbacks = true;
                        m.ActiveMenuWrapper.toggle.isOn = false;
                        m._ignoreToggleCallbacks = false;
                    }

                    LoadMenu(m, wrapper); // calls m.TaskDone()
                }
                else
                {
                    m.queue.TaskDone();
                }
            }

            public static void LoadMenu(MenuScreenManager m, Wrapper wrapper, Action onLoaded = null)
            {
                BaseMenu activeMenu = null;
                if (m.ActiveMenuWrapper != null) activeMenu = m.ActiveMenuWrapper.menu;
                MenuLoader.LoadAndUnload(wrapper.menu, activeMenu, m.defaultTransition, FinishUp);

                void FinishUp()
                {
                    m.ActiveMenuWrapper = wrapper;
                    m.UpdateToggles();
                    onLoaded?.Invoke();
                    m.queue.TaskDone();
                }
            }

            public static void ActivateScreen(MenuScreenManager m, Wrapper wrapper)
            {
                // Validate
                if (m.managedMenus.Count == 0)
                {
                    Debug.LogError("Empty menu screen. ManagedMenus cannot be empty");
                    m.IsActive = false;
                    return;
                }

                // Update fields
                m.IsActive = true;
                LoadMenu(m, wrapper); // m.TaskDone();
            }

            public static void DeactivateScreen(MenuScreenManager m)
            {
                foreach (Wrapper wrapper in m.managedMenus)
                {
                    MenuLoader.Unload(wrapper.menu);
                }

                m.ActiveMenuWrapper = null;
                m.IsActive = false;
                m.queue.TaskDone();
            }
        }
    }
}