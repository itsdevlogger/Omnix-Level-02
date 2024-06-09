using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace MenuManagement.Base
{
    /// <summary>
    /// Lowest-Level representation of a Menu in UI.
    /// User may never need to implement this class, instead derive from one of the other MenuBase classes depending on requirement.
    /// </summary>
    [GroupProperties(_.AudioClips, "loadClip", "unloadClip", "loadDelay", "unloadDelay")]
    [GroupEvents("onLoad", "onLoadDelayed", "onUnload", "onUnloadDelayed", "onRefresh")]
    [GroupRuntimeConstant("thisObjectActiveWithMenu", "loadInAwake", "activeWithMenu", "activeAgainstMenu", "activeWhileRefresh", "inactiveWhileRefresh")]
    public class BaseMenu : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("Should the menu be loaded at Awake callback.")]
        private bool loadInAwake;

        [SerializeField, Tooltip("Should this object be active if and only if the menu is loaded.")]
        private bool thisObjectActiveWithMenu;

        [SerializeField, Tooltip("Audio clip is played while loading this menu.")]
        private AudioClip loadClip;

        [SerializeField, Tooltip("Control menu loading delay after the clip is played")]
        private float loadDelay;

        [SerializeField, Tooltip("Audio clip is played when unloading this menu.")]
        private AudioClip unloadClip;

        [SerializeField, Tooltip("Control menu unloading delay after the clip is played")]
        private float unloadDelay;

        [SerializeField, Tooltip("Objects that should be active when the menu is loaded adn inactive when the menu is unloaded")]
        private GameObject[] activeWithMenu;

        [SerializeField, Tooltip("Objects that should be inactive when the menu is loaded adn active when the menu is unloaded")]
        private GameObject[] activeAgainstMenu;

        [SerializeField, Tooltip("Objects that should be active when the menu is being refreshed")]
        private GameObject[] activeWhileRefresh;

        [SerializeField, Tooltip("Objects that should be inactive when the menu is being refreshed")]
        private GameObject[] inactiveWhileRefresh;


        /// <summary> If you are using Helper Methods or any MenuManager, then no need to update this. </summary>
        public MenuStatus Status { get; private set; } = MenuStatus.Unloaded;
        public bool IsManaged { get; internal set; } = false;
        public bool LoadInAwake => loadInAwake;
        #endregion

        #region Events
        [Tooltip("Called to load this menu")] 
        public UnityEvent onLoad;

        [Tooltip("Called 2 frame after the menu loading ends")]
        public UnityEvent onLoadDelayed;

        [Tooltip("Called to unload this menu")]
        public UnityEvent onUnload;

        [Tooltip("Called 2 frame after the menu unloading ends")]
        public UnityEvent onUnloadDelayed;

        [Tooltip("Called to refresh this menu")]
        public UnityEvent onRefresh;

        private bool notInitialized = true;
        #endregion

        #region Virtual
        public BaseMenu()
        {
            if (MenuLoader.Instance != null)
            {
                MenuLoader.RegisterMenu(this);
            }
        }
        //{
        //    if (InitMenus.Instance == null)
        //    {
        //        // Means we are in editor mode
        //        Debug.Log("Its null");
        //        return;
        //    }
        //    InitMenus.Instance.RegisterCallback(Init);
        //}


        /// <summary> Will be called only once when the menu is either loaded/unloaded for the first time (Whichever happens first) </summary>
        protected virtual void Initialize() { }

        /// <summary> Coroutine to hold loading of this menu. </summary>
        protected virtual IEnumerator BeforeLoad() { yield break; }

        /// <summary> Coroutine to hold unloading of this menu. </summary>
        protected virtual IEnumerator AfterUnload() { yield break; }

        /// <summary> Coroutine to hold refresh of this menu. </summary>
        protected virtual IEnumerator BeforeRefresh() { yield break; }

        /// <summary> Called to load this menu </summary>
        protected virtual void OnLoad() { }

        /// <summary> Called 2 frame after the menu loading ends </summary>
        protected virtual void OnLoadDelayed() { }

        /// <summary> Called to unload this menu </summary>
        protected virtual void OnUnload() { }

        /// <summary> Called 2 frame after the menu unloading ends </summary>
        protected virtual void OnUnloadDelayed() { }

        /// <summary> Called to refresh this menu </summary>
        protected virtual void OnRefresh() { }
        #endregion

        #region Functions

        private void SetStateOfConnectedGameObjects()
        {
            if (Status == MenuStatus.InRefresh) return;
            
            bool active = Status == MenuStatus.Loaded || Status == MenuStatus.InLoading;
            if (thisObjectActiveWithMenu) gameObject.SetActive(active);

            foreach (GameObject go in activeWithMenu)
            {
                go.SetActive(active);
            }

            active = !active;
            foreach (GameObject go in activeAgainstMenu)
            {
                go.SetActive(active);
            }
        }

        /// <summary>
        /// Starts the coroutine that will load this menu.
        /// Does not have any parameters because this method is intended to be called from the inspector.
        /// </summary>
        /// <remark>
        /// <para>Do not call this method if you are using Menu Manager</para>
        /// <para>Use <see cref="MenuLoader.Load"/> or <see cref="MenuLoader.LoadAndUnload"/> method to specify other parameters</para> 
        /// </remark>
        public void BeginLoading() => MenuLoader.Load(this);

        /// <summary>
        /// Starts the coroutine that will unload this menu
        /// Does not have any parameters because this method is intended to be called from the inspector.
        /// </summary>
        /// <para>Do not call this method if you are using Menu Manager</para>
        /// <para>Use <see cref="MenuLoader.Unload"/> or <see cref="MenuLoader.LoadAndUnload"/> method to specify other parameters</para> 
        public void BeginUnloading() => MenuLoader.Unload(this);

        /// <summary>
        /// Starts the coroutine that will refresh this menu
        /// Does not have any parameters because this method is intended to be called from the inspector.
        /// </summary>
        /// <para>Do not call this method if you are using Menu Manager</para>
        /// <para>Use <see cref="MenuLoader.Refresh"/> to specify other parameters</para> 
        public void BeginRefresh() => MenuLoader.Refresh(this);

        /// <summary>
        /// Starts the coroutine that will load this menu if its unloaded and unload this menu if its loaded.
        /// Does not have any parameters because this method is intended to be called from the inspector.
        /// </summary>
        /// <para>Do not call this method if you are using Menu Manager</para>
        /// <para>Use <see cref="MenuLoader"/> to specify other parameters</para>
        public void BeginToggleState()
        {
            switch (Status)
            {
                case MenuStatus.Loaded:
                case MenuStatus.InLoading:
                    BeginUnloading();
                    break;
                case MenuStatus.Unloaded:
                case MenuStatus.InUnloading:
                    BeginLoading();
                    break;
            }
        }

        /// <summary> Coroutine to load one menu while unloading another. </summary>
        /// <remarks> Do not call StartCoroutine to start this Coroutine, the gameObject might be inactive. Instead use <see cref="MenuLoader.LoadAndUnload"/>. </remarks>
        /// <param name="load"> Menu to load. </param>
        /// <param name="unload"> Menu to unload. </param>
        /// <param name="transition"> Transition to be used for this operation. </param>
        /// <param name="onComplete"> Callback after process is finished. </param>
        /// <param name="onFail"> Callback for when the menu state is not set properly which is causing the load operation to fail </param>
        internal static IEnumerator LoadAndUnload([NotNull] BaseMenu load, [NotNull] BaseMenu unload, [CanBeNull] IMenuTransition transition, Action onComplete, Action onFail)
        {
            #region Initialize
            if (load.notInitialized)
            {
                load.notInitialized = false;
                load.Initialize();
            }

            if (unload.notInitialized)
            {
                unload.notInitialized = false;
                unload.Initialize();
            }
            #endregion

            #region Play Clips
            if (load.loadClip != null) MenuLoader.PlayClip(load.loadClip);
            if (unload.unloadClip != null) MenuLoader.PlayClip(unload.unloadClip);
            #endregion

            #region Load & Unload Delays (Union)
            float maximum = Mathf.Max(load.loadDelay, unload.unloadDelay);
            if (maximum > 0) yield return new WaitForSeconds(maximum);
            #endregion

            #region Wait For Menu Status
            int frameCount = 0;
            while (load.Status == MenuStatus.InLoading || load.Status == MenuStatus.InUnloading || unload.Status == MenuStatus.InLoading || unload.Status == MenuStatus.InUnloading)
            {
                frameCount++;
                if (frameCount > 1000)
                {
                    Debug.LogError($"Failed Operation LoadAndUnload({load}, {unload}, {transition}).");
                    onFail?.Invoke();
                    yield break;
                }

                yield return null;
            }
            #endregion

            #region Set Menu Status
            load.Status = MenuStatus.InLoading;
            unload.Status = MenuStatus.InUnloading;
            #endregion

            #region Execute LoadMenu.BeforeLoad Coroutine
            load.SetStateOfConnectedGameObjects();
            IEnumerator beforeLoad = load.BeforeLoad();
            while (beforeLoad.MoveNext()) yield return beforeLoad.Current;
            #endregion

            #region Callback: LoadMenu.onLoad 
            load.OnLoad();
            load.onLoad?.Invoke();
            #endregion

            #region Transition 
            if (transition != null)
            {
                IEnumerator trans = transition.Animate(unload, load);
                while (trans.MoveNext()) yield return trans.Current;
            }
            #endregion

            #region Callback: UnloadMenu.onUnload 
            unload.OnUnload();
            unload.onUnload?.Invoke();
            #endregion

            #region Execute UnloadMenu.AfterUnload Coroutine
            IEnumerator afterUnload = unload.AfterUnload();
            while (afterUnload.MoveNext()) yield return afterUnload.Current;
            unload.SetStateOfConnectedGameObjects();
            #endregion

            #region Set Menu Status
            load.Status = MenuStatus.Loaded;
            unload.Status = MenuStatus.Unloaded;
            #endregion

            #region Finish Up
            onComplete?.Invoke();
            // 12. 1 frame delay
            yield return null;

            // 13. Cleanup any mess made during transition
            if (transition != null)
                transition.Cleanup(unload, load);

            // 14. 1 frame delay
            yield return null;

            // 15. Delayed Callbacks
            load.OnLoadDelayed();
            load.onLoadDelayed?.Invoke();
            
            unload.OnUnloadDelayed();
            unload.onUnloadDelayed?.Invoke();
            #endregion
        }

        /// <summary> Coroutine to load given menu. </summary>
        /// <remarks> Do not call StartCoroutine to start this Coroutine, the gameObject might be inactive. Instead use <see cref="MenuLoader"/>. </remarks>
        /// <param name="load"> Menu to load. </param>
        /// <param name="transition"> Transition to be used for this operation. </param>
        /// <param name="onComplete"> Callback after process is finished. </param>
        /// <param name="onFail"> Callback for when the menu state is not set properly which is causing the load operation to fail </param>
        internal static IEnumerator Load([NotNull] BaseMenu load, [CanBeNull] IMenuTransition transition, Action onComplete, Action onFail)
        {
            #region Initialize
            if (load.notInitialized)
            {
                load.notInitialized = false;
                load.Initialize();
            }
            #endregion

            #region Play Clips
            if (load.loadClip != null) MenuLoader.PlayClip(load.loadClip);
            #endregion

            #region Load & Unload Delays (Union)
            if (load.loadDelay > 0) yield return new WaitForSeconds(load.loadDelay);
            #endregion

            #region Wait For Menu Status
            int frameCount = 0;
            while (load.Status == MenuStatus.InLoading || load.Status == MenuStatus.InUnloading)
            {
                frameCount++;
                if (frameCount > 1000)
                {
                    Debug.LogError($"Failed Operation Load({load}, {transition}).");
                    onFail?.Invoke();
                    yield break;
                }

                yield return null;
            }
            #endregion

            #region Set Menu Status
            load.Status = MenuStatus.InLoading;
            load.SetStateOfConnectedGameObjects();
            #endregion

            #region Execute LoadMenu.BeforeLoad Coroutine
            IEnumerator beforeLoad = load.BeforeLoad();
            while (beforeLoad.MoveNext()) yield return beforeLoad.Current;
            #endregion

            #region Callback: LoadMenu.onLoad 
            load.OnLoad();
            load.onLoad?.Invoke();
            load.SetStateOfConnectedGameObjects();
            #endregion

            #region Transition 
            if (transition != null)
            {
                IEnumerator trans = transition.Animate(null, load);
                while (trans.MoveNext()) yield return trans.Current;
            }
            #endregion

            #region Set Menu Status
            load.Status = MenuStatus.Loaded;
            #endregion

            #region Finish Up
            onComplete?.Invoke();
            // 12. 1 frame delay
            yield return null;

            // 13. Cleanup any mess made during transition
            if (transition != null)
                transition.Cleanup(null, load);

            // 14. 1 frame delay
            yield return null;

            // 15. Delayed Callbacks
            load.OnLoadDelayed();
            load.onLoadDelayed?.Invoke();
            #endregion
        }

        /// <summary> Coroutine to unload given menu. </summary>
        /// <remarks> Do not call StartCoroutine to start this Coroutine, the gameObject might be inactive. Instead use <see cref="MenuLoader"/>. </remarks>
        /// <param name="unload"> Menu to unload. </param>
        /// <param name="transition"> Transition to be used for this operation. </param>
        /// <param name="onComplete"> Callback after process is finished. </param>
        /// <param name="onFail"> Callback for when the menu state is not set properly which is causing the load operation to fail </param>
        internal static IEnumerator Unload([NotNull] BaseMenu unload, [CanBeNull] IMenuTransition transition, Action onComplete, Action onFail)
        {
           #region Initialize
            if (unload.notInitialized)
            {
                unload.notInitialized = false;
                unload.Initialize();
            }
            #endregion

            #region Play Clips
            if (unload.unloadClip != null) MenuLoader.PlayClip(unload.unloadClip);
            #endregion

            #region Load & Unload Delays (Union)
            if (unload.unloadDelay > 0) yield return new WaitForSeconds(unload.unloadDelay);
            #endregion

            #region Wait For Menu Status
            int frameCount = 0;
            while (unload.Status == MenuStatus.InLoading || unload.Status == MenuStatus.InUnloading)
            {
                frameCount++;
                if (frameCount > 1000)
                {
                    Debug.LogError($"Failed Operation Unload({unload}, {transition}).");
                    onFail?.Invoke();
                    yield break;
                }

                yield return null;
            }
            #endregion

            #region Set Menu Status
            unload.Status = MenuStatus.InUnloading;
            unload.SetStateOfConnectedGameObjects();
            #endregion

            #region Transition 
            if (transition != null)
            {
                IEnumerator trans = transition.Animate(unload, null);
                while (trans.MoveNext()) yield return trans.Current;
            }
            #endregion

            #region Callback: UnloadMenu.onUnload 
            unload.OnUnload();
            unload.onUnload?.Invoke();
            unload.SetStateOfConnectedGameObjects();
            #endregion

            #region Execute UnloadMenu.AfterUnload Coroutine
            IEnumerator afterUnload = unload.AfterUnload();
            while (afterUnload.MoveNext()) yield return afterUnload.Current;
            #endregion

            #region Set Menu Status
            unload.Status = MenuStatus.Unloaded;
            #endregion

            #region Finish Up
            onComplete?.Invoke();
            // 12. 1 frame delay
            yield return null;

            // 13. Cleanup any mess made during transition
            if (transition != null)
                transition.Cleanup(unload, null);

            // 14. 1 frame delay
            yield return null;

            // 15. Delayed Callbacks
            unload.OnUnload();
            unload.onUnloadDelayed?.Invoke();
            #endregion
        }

        internal static IEnumerator Refresh([NotNull] BaseMenu menu, Action onComplete, Action onFail)
        {
            if (menu.Status != MenuStatus.Loaded) yield break;

            menu.Status = MenuStatus.InRefresh;

            foreach (var go in menu.activeWhileRefresh) go.SetActive(true);
            foreach (var go in menu.inactiveWhileRefresh) go.SetActive(false);

            IEnumerator beforeRefresh = menu.BeforeRefresh();
            while (beforeRefresh.MoveNext()) yield return beforeRefresh.Current;

            menu.OnRefresh();
            menu.onRefresh?.Invoke();

            foreach (var go in menu.activeWhileRefresh) go.SetActive(false);
            foreach (var go in menu.inactiveWhileRefresh) go.SetActive(true);

            menu.Status = MenuStatus.Loaded;
        }
        #endregion
    }
}