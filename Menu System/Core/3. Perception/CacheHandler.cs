using System;
using System.Collections.Generic;
using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Perception
{
    public static class Cache
    {
        /// <summary> What to do with a menu if its not being tracked or the cache is outdated </summary>
        public enum MenuState
        {
            Ignore,
            Loaded,
            Unloaded
        }
        
        private const float CACHE_LIFETIME = 5f;
        public static event Action OnSaveCache;
        public static bool IsOutdated => lastStoreTime + CACHE_LIFETIME < Time.time;

        private static readonly Dictionary<BaseMenu, bool> ManagingMenus = new Dictionary<BaseMenu, bool>();
        private static readonly Dictionary<MenuScreenManager, BaseMenu> ManagingScreens = new Dictionary<MenuScreenManager, BaseMenu>();
        private static float lastStoreTime;

        public static void StartTrackingCache(this BaseMenu menu) => ManagingMenus.TryAdd(menu, false);
        public static void StartTrackingCache(this MenuScreenManager screen) => ManagingScreens.TryAdd(screen, null);
        public static void StopTrackingCache(this BaseMenu menu) => ManagingMenus.Remove(menu);
        public static void StopTrackingCache(this MenuScreenManager screen) => ManagingScreens.Remove(screen);

        /// <summary> Restores the last known state of the menu (if the cache is not outdated) </summary>
        /// <param name="menu"> menu </param>
        /// <param name="defaultTo"> menu state if the menu is not being tracked or the cache is outdated </param>
        public static void RestoreCachedState(this BaseMenu menu, MenuState defaultTo)
        {
            MenuState finalState;
            if (IsOutdated)
            {
                finalState = defaultTo;
            }
            else
            {
                if (ManagingMenus.TryGetValue(menu, out bool isLoaded))
                {
                    finalState = isLoaded ? MenuState.Loaded : MenuState.Unloaded;
                }
                else
                {
                    finalState = defaultTo;
                }
            }

            switch (finalState)
            {
                case MenuState.Ignore: break;
                case MenuState.Loaded:
                    if (menu.Status != MenuStatus.InLoading && menu.Status != MenuStatus.Loaded) menu.BeginLoading();
                    break;
                case MenuState.Unloaded:
                    if (menu.Status != MenuStatus.InUnloading && menu.Status != MenuStatus.Unloaded) menu.BeginUnloading();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Restores the last known state of the menu (if the cache is not outdated) </summary>
        /// <param name="manager"> manager </param>
        /// <param name="defaultTo">
        /// index of menu to load if manager is not being tracked or the cache is outdated.
        /// -2 means deactivate screen.
        /// -1 means do nothing
        /// </param>
        public static void RestoreCachedState(this MenuScreenManager manager, int defaultTo)
        {
            if (IsOutdated)
            {
                Default();
                return;
            }

            if (ManagingScreens.TryGetValue(manager, out BaseMenu activeMenu) && activeMenu != null)
            {
                manager.ActivateScreen(activeMenu);
            }
            else
            {
                Default();
            }

            return;

            void Default()
            {
                switch (defaultTo)
                {
                    case -2:
                        if (manager.IsActive) manager.DeactivateScreen();
                        break;
                    case -1:
                        // DO NOTHING 
                        break;
                    case >= 0:
                        manager.ActivateScreen(defaultTo);
                        break;
                }
            }
        }
        
        public static void RecordStates()
        {
            var menus = new List<BaseMenu>(ManagingMenus.Keys);
            foreach (BaseMenu menu in menus)
            {
                ManagingMenus[menu] = menu.Status is MenuStatus.Loaded or MenuStatus.InLoading;
            }

            var screens = new List<MenuScreenManager>(ManagingScreens.Keys);
            foreach (MenuScreenManager screen in screens)
            {
                ManagingScreens[screen] = screen.ActiveMenuWrapper?.menu;
            }

            OnSaveCache?.Invoke();
            lastStoreTime = Time.time;
        }
    }
}