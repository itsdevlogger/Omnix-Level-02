using System;
using System.Linq;
using System.Collections.Generic;
using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Perception
{
    public static class Cache
    {
        #region Classes
        /// <summary> What to do with a menu if its not being tracked or the cache is outdated </summary>
        public enum MenuState
        {
            Ignore,
            Loaded,
            Unloaded
        }

        /// <summary> Set the screen state </summary>
        public static class ScreenState
        {
            public const int INACTIVE = -2;
            public const int DO_NOTHING = -1;
            public static int LoadMenuWithIndex(int index) => index;
        }
        #endregion

        #region Fields
        public static event Action OnRecordCache;
        public static bool IsOutdated => lastStoreTime + CACHE_LIFETIME < Time.time;
        public static bool IsValid => !IsOutdated;

        /// <summary> Time (In Seconds) after which the cache should be considered outdated </summary>
        private const float CACHE_LIFETIME = 5f;

        private static float lastStoreTime;
        private static readonly Dictionary<BaseMenu, bool> MANAGING_MENUS = new Dictionary<BaseMenu, bool>();
        private static readonly Dictionary<MenuScreenManager, BaseMenu> MANAGING_SCREENS = new Dictionary<MenuScreenManager, BaseMenu>();
        #endregion

        #region Extensions
        /// <summary> Starts tracking this menu. </summary>
        /// <remarks> State is recorded only when <see cref="Record"/> method is called </remarks>
        public static void StartTrackingCache(this BaseMenu menu) => MANAGING_MENUS.TryAdd(menu, false);

        /// <summary> Stop tracking this menu. </summary>
        public static void StopTrackingCache(this BaseMenu menu) => MANAGING_MENUS.Remove(menu);

        /// <summary> Starts tracking this menu manager. </summary>
        /// <remarks> State is recorded only when <see cref="Record"/> method is called </remarks>
        public static void StartTrackingCache(this MenuScreenManager screen) => MANAGING_SCREENS.TryAdd(screen, null);

        /// <summary> Stop tracking this menu manager. </summary>
        public static void StopTrackingCache(this MenuScreenManager screen) => MANAGING_SCREENS.Remove(screen);

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
                if (MANAGING_MENUS.TryGetValue(menu, out bool isLoaded))
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
        /// see <see cref="ScreenState"/>
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

            if (MANAGING_SCREENS.TryGetValue(manager, out BaseMenu activeMenu) && activeMenu != null)
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

        /// <summary> Gets the last recorded active menu. </summary>
        /// <returns> null if cache is not recorded or outdated. </returns>
        public static BaseMenu GetLastKnownActiveMenu(this MenuScreenManager manager)
        {
            if (MANAGING_SCREENS.TryGetValue(manager, out BaseMenu menu)) return menu;
            return null;
        }

        /// <summary> Gets the last recorded menu state </summary>
        /// <returns> <see cref="MenuState.Ignore"/> if cache is not recorded or outdated. </returns>
        public static MenuState GetLastKnownState(this BaseMenu menu)
        {
            if (MANAGING_MENUS.TryGetValue(menu, out bool state)) return state ? MenuState.Loaded : MenuState.Unloaded;
            return MenuState.Ignore;
        }
        #endregion

        /// <summary> Records state of all the menus and managers that are being tracked </summary>
        public static void Record()
        {
            var menus = new List<BaseMenu>(MANAGING_MENUS.Keys.Where(m => m != null));
            foreach (BaseMenu menu in menus)
            {
                MANAGING_MENUS[menu] = menu.Status is MenuStatus.Loaded or MenuStatus.InLoading;
            }

            var screens = new List<MenuScreenManager>(MANAGING_SCREENS.Keys.Where(m => m != null && m.ActiveMenuWrapper != null));
            foreach (MenuScreenManager screen in screens)
            {
                MANAGING_SCREENS[screen] = screen.ActiveMenuWrapper.menu;
            }

            OnRecordCache?.Invoke();
            lastStoreTime = Time.time;
        }
    }
}