using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Omnix.Monos
{
    public class SceneController : MonoBehaviour
    {
        private static SceneController m_Instance;
        public static Action<string> OnNewLevelLoaded;
        public static Action<string> OnUnloadSceneCompleted;

        [SerializeField] private GameObject loadingCanvas;
        [SerializeField] private UnityEvent<float, string> loadingView;
        [SerializeField] private string loadingText;

        public static string CurrentScene;

        private void Awake()
        {
            m_Instance = this;
            SceneManager.sceneLoaded += OnLevelLoaded;
        }

        private void Start()
        {
            //PlayerData.Create();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnLevelLoaded;
        }

        public static void ReloadCurrentScene()
        {
            SceneProperties sceneProperties = new SceneProperties()
            {
                isAsync = true,
                loadSceneMode = LoadSceneMode.Single,
                sceneName = CurrentScene,
                showLoading = true
            };

            LoadScene(sceneProperties);
        }

        private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 1f;
            SceneManager.SetActiveScene(scene);
            OnNewLevelLoaded?.Invoke(scene.name);
            CurrentScene = scene.name;
        }

        public static void LoadScene(SceneProperties sceneProperties, Action callback = null)
        {
            string sceneName = sceneProperties.sceneName;

            Debug.LogWarning("load scene ============" + sceneName);

            if (sceneProperties.isAsync)
            {
                m_Instance.StartCoroutine(m_Instance.LoadSceneAsync(sceneName, sceneProperties, callback));
            }
            else
            {
                SceneManager.LoadScene(sceneName, sceneProperties.loadSceneMode);
            }
        }

        private IEnumerator LoadSceneAsync(string sceneName, SceneProperties sceneProperties, Action callback)
        {
            yield return StartCoroutine(ShowLoading(sceneProperties));

            //Debug.Log("called again");       
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, sceneProperties.loadSceneMode);

            float dummyProgress = 0.1f;

            while (op.progress < 1)
            {
                float progress = (op.progress < 9) ? dummyProgress : op.progress;
                dummyProgress += 0.1f;
                dummyProgress = Mathf.Clamp(dummyProgress, 0f, 0.9f);

                loadingView?.Invoke(progress, loadingText);
                yield return null;
            }

            loadingView?.Invoke(1f, loadingText);
            yield return new WaitForSeconds(1f);

            if (loadingCanvas != null)
            {
                loadingCanvas.SetActive(false);
            }

            callback?.Invoke();
        }

        private IEnumerator ShowLoading(SceneProperties sceneProperties)
        {
            if (sceneProperties.showLoading)
            {
                loadingCanvas?.SetActive(true);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                loadingCanvas?.SetActive(false);
            }
        }

        private IEnumerator WaitForUnloadScene(string sceneName)
        {
            AsyncOperation sceneUnload = SceneManager.UnloadSceneAsync(sceneName);
            while (sceneUnload != null && !sceneUnload.isDone)
            {
                yield return null;
            }

            OnUnloadSceneCompleted?.Invoke(sceneName);
        }
    }

    [System.Serializable]
    public class SceneProperties
    {
        public string sceneName;
        public LoadSceneMode loadSceneMode;
        public bool isAsync;
        public bool showLoading;

        public SceneProperties()
        {
        }

        public SceneProperties(string sceneName)
        {
            this.sceneName = sceneName;
            loadSceneMode = LoadSceneMode.Single;
            isAsync = true;
            showLoading = true;
        }
    }
}