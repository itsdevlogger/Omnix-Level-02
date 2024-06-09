using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MenuManagement.Perception;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Base
{
    public class MenuLoader : MonoBehaviour
    {
        public static MenuLoader Instance { get; private set; }
        private AudioSource[] defaultSources;
        private Dictionary<BaseMenu, IMenuTransition> cachedTransitions;

        #region Init Stuff
        private static readonly List<MenuScreenManager> _managers = new List<MenuScreenManager>();
        private static readonly List<BaseMenu> _menus = new List<BaseMenu>();
        private static bool _hasNoMenus = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateInstance()
        {
            var go = new GameObject("[Menu Management Center]");
            Instance = go.AddComponent<MenuLoader>();
            Instance.cachedTransitions = new Dictionary<BaseMenu, IMenuTransition>();
            Instance.defaultSources = new AudioSource[]
            {
                        go.AddComponent<AudioSource>(),
                        go.AddComponent<AudioSource>(),
                        go.AddComponent<AudioSource>(),
                        go.AddComponent<AudioSource>(),
                        go.AddComponent<AudioSource>()
            };
            DontDestroyOnLoad(Instance.gameObject);
        }

        public static void RegisterMenu(BaseMenu menu)
        {
            if (menu is MenuScreenManager manager) _managers.Add(manager);
            else _menus.Add(menu);
            _hasNoMenus = false;
        }

        private void Update()
        {
            if (_hasNoMenus) return;

            foreach (var manager in _managers)
            {
                if (manager.LoadInAwake) MenuLoader.Load(manager);
                else MenuLoader.UnloadWithoutTransition(manager);
            }

            int i = 0;
            while (i < _menus.Count)
            {
                var menu = _menus[i];
                i++;
                if (menu == null || menu.IsManaged) continue;

                if (menu.LoadInAwake) MenuLoader.Load(menu);
                else MenuLoader.UnloadWithoutTransition(menu);
            }
            _menus.Clear();
            _hasNoMenus = true;
            Instance.enabled = false;   // This wont stop Coroutine, but will stop this callback
        }
        #endregion

        #region Loading Stuff
        public static void LoadWithoutTransition([NotNull] BaseMenu menu, Action onComplete = null, Action onFail = null)
        {
            Instance.StartCoroutine(BaseMenu.Load(menu, null, onComplete, onFail));
        }
        
        public static void UnloadWithoutTransition([NotNull] BaseMenu menu, Action onComplete = null, Action onFail = null)
        {
            Instance.StartCoroutine(BaseMenu.Unload(menu, null, onComplete, onFail));
        }
        
        /// <summary> Coroutine to load given menu. </summary>
        /// <param name="menu"> Menu to load. </param>
        /// <param name="transition"> Transition to be used for this operation. </param>
        /// <param name="onComplete"> Callback after process is finished. </param>
        /// <param name="onFail"> Callback for when the menu state is not set properly which is causing the load operation to fail </param>
        public static void Load([NotNull] BaseMenu menu, IMenuTransition transition = null, Action onComplete = null, Action onFail = null)
        {
            if (transition == null) transition = GetCachedTransition(menu);
            Instance.StartCoroutine(BaseMenu.Load(menu, transition, onComplete, onFail));
        }

        /// <summary> Coroutine to unload given menu. </summary>
        /// <param name="menu"> Menu to unload. </param>
        /// <param name="transition"> Transition to be used for this operation. </param>
        /// <param name="onComplete"> Callback after process is finished. </param>
        /// <param name="onFail"> Callback for when the menu state is not set properly which is causing the load operation to fail </param>
        public static void Unload([NotNull] BaseMenu menu, IMenuTransition transition = null, Action onComplete = null, Action onFail = null)
        {
            if (transition == null) transition = GetCachedTransition(menu);
            Instance.StartCoroutine(BaseMenu.Unload(menu, transition, onComplete, onFail));
        }

        /// <summary> Coroutine to load one menu while unloading another. </summary>
        /// <param name="load"> Menu to load. </param>
        /// <param name="unload"> Menu to unload. </param>
        /// <param name="transition"> Transition to be used for this operation. </param>
        /// <param name="onComplete"> Callback after process is finished. </param>
        /// <param name="onFail"> Callback for when the menu state is not set properly which is causing the load operation to fail </param>
        public static void LoadAndUnload([CanBeNull] BaseMenu load, [CanBeNull] BaseMenu unload, IMenuTransition transition = null, Action onComplete = null, Action onFail = null)
        {
            if (load == null)
            {
                if (unload != null) Unload(unload, transition, onComplete, onFail);
                return;
            }

            if (unload == null)
            {
                Load(load, transition, onComplete, onFail);
                return;
            }


            if (transition == null) transition = GetCachedTransition(load);
            if (transition == null) transition = GetCachedTransition(unload);
            Instance.StartCoroutine(BaseMenu.LoadAndUnload(load, unload, transition, onComplete, onFail));
        }

        public static void Refresh(BaseMenu menu, Action onComplete = null, Action onFail = null)
        {
            Instance.StartCoroutine(BaseMenu.Refresh(menu, onComplete, onFail));
        }

        public static void PlayClip(AudioClip clip)
        {
            foreach (AudioSource source in Instance.defaultSources)
            {
                if (source.isPlaying == false)
                {
                    source.clip = clip;
                    source.Play();
                    return;
                }
            }

            Instance.StartCoroutine(PlayOnTempSource(clip));
        }

        private static IEnumerator PlayOnTempSource(AudioClip clip)
        {
            AudioSource tempSource = Instance.gameObject.AddComponent<AudioSource>();
            tempSource.clip = clip;
            tempSource.Play();
            while (tempSource.isPlaying)
            {
                yield return null;
            }

            DestroyImmediate(tempSource);
        }

        private static IMenuTransition GetCachedTransition([NotNull] BaseMenu menu)
        {
            IMenuTransition trans;
            if (Instance.cachedTransitions.TryGetValue(menu, out trans))
            {
                if (trans != null) return trans;

                Instance.cachedTransitions.Remove(menu);
            }

            trans = menu.GetComponent<IMenuTransition>();
            if (trans != null) Instance.cachedTransitions.Add(menu, trans);
            return trans;
        }
        #endregion
    }
}