using System;
using System.Collections;
using System.Collections.Generic;
using Omnix.DesignPatterns;
using UnityEngine;
using UnityEngine.Events;

namespace Omnix.Notification
{
    public class NotiCon : Singleton<NotiCon>
    {
        #region Fields
        [SerializeField] private Transform _canvas;
        [SerializeField] private NotiTheme _defaultTheme;
        [SerializeField] private NotiTheme[] _otherThemes;

        private TaskQueue _queue = new TaskQueue(); // first-in, first-out collection. 
        private Stack<LoadingScreen> _loadingStack = new Stack<LoadingScreen>(); // last-in, first-out collection.
        private MessageScreen _messageScreen;
        private MessageScreen _successScreen;
        private MessageScreen _errorScreen;
        private ConfirmScreen _confirmScreen;
        private LoadingScreen _loadingScreen;

        public static string LoadingText
        {
            get => Instance._loadingScreen.Text;
            set => Instance._loadingScreen.Text = value;
        }

        public static float LoadingProgress
        {
            get => Instance._loadingScreen.Progress;
            set => Instance._loadingScreen.Progress = value;
        }

        public static float LoadingAutofillTime
        {
            get => Instance._loadingScreen.progressAutoFillTime;
            set => Instance._loadingScreen.progressAutoFillTime = value;
        }
        #endregion

        #region Functionalities
        protected override void Init()
        {
            _messageScreen = CreateInstance(_defaultTheme.messageScreen, false);
            _successScreen = CreateInstance(_defaultTheme.successScreen, false);
            _errorScreen = CreateInstance(_defaultTheme.errorScreen, false);
            _confirmScreen = CreateInstance(_defaultTheme.confirmScreen, false);
            _loadingScreen = CreateInstance(_defaultTheme.loadingScreen, false);
        }

        private T CreateInstance<T>(T screen, bool destroyOnClose) where T : BaseScreen
        {
            T inst = Instantiate(screen, _canvas);
            inst.destroyOnClose = destroyOnClose;
            inst.gameObject.SetActive(false);
            return inst;
        }

        private static IEnumerator CloseScreenCr(int screenId, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (BaseScreen.CurrentScreen != null && BaseScreen.CurrentScreen.ScreenId == screenId)
            {
                BaseScreen.CurrentScreen.Close();
            }
        }

        private bool TryGetTheme(ThemeIndex index, out NotiTheme theme)
        {
            theme = null;
            if (_otherThemes == null || _otherThemes.Length == 0)
            {
                Debug.LogError("NotiCon: There are no extra themes");
                return false;
            }

            int indexInt = (int)index;
            theme = _otherThemes[Mathf.Clamp(indexInt - 1, 0, _otherThemes.Length)];
            return theme != null;
        }

        internal void ScreenClosed()
        {
            _queue.TaskDone();
        }

        internal void AutohideActiveScreen(float autoHideDuration)
        {
            StartCoroutine(CloseScreenCr(BaseScreen.CurrentScreen.ScreenId, autoHideDuration));
        }
        #endregion

        #region Access Default Screens
        public static void Info(string title, string description, float autoHideDuration = -1f, Action onOkayClicked = null, UnityAction onCloseClicked = null, bool okayButtonActive = true, string okayButtonText = null)
        {
            IButtonSettings okay = new ButtonSettings(okayButtonText, okayButtonActive, onOkayClicked);
            Instance._queue.BeginTask(Instance._messageScreen.Init, title, description, okay, onCloseClicked, autoHideDuration);
        }

        public static void Success(string title, string description, float autoHideDuration = -1f, Action onOkayClicked = null, UnityAction onCloseClicked = null, bool okayButtonActive = true, string okayButtonText = null)
        {
            IButtonSettings okay = new ButtonSettings(okayButtonText, okayButtonActive, onOkayClicked);
            Instance._queue.BeginTask(Instance._successScreen.Init, title, description, okay, onCloseClicked, autoHideDuration);
        }

        public static void Error(string title, string description, float autoHideDuration = -1f, Action onOkayClicked = null, UnityAction onCloseClicked = null, bool okayButtonActive = true, string okayButtonText = null)
        {
            IButtonSettings okay = new ButtonSettings(okayButtonText, okayButtonActive, onOkayClicked);
            Instance._queue.BeginTask(Instance._errorScreen.Init, title, description, okay, onCloseClicked, autoHideDuration);
        }

        public static void Confirm(string title, string description, Action onYesClicked = null, Action onNoClicked = null, string yesButtonText = null, string noButtonText = null)
        {
            IButtonSettings yes = new ButtonSettings(yesButtonText, onYesClicked);
            IButtonSettings no = new ButtonSettings(noButtonText, onNoClicked);
            IButtonSettings cancel = new ButtonSettings(false);
            Instance._queue.BeginTask(Instance._confirmScreen.Init, title, description, yes, no, cancel);
        }

        public static void Confirm(string title, string description, Action onYesClicked = null, Action onNoClicked = null, Action onCancelClicked = null, string yesButtonText = null, string noButtonText = null, string cancelButtonText = null)
        {
            IButtonSettings yes = new ButtonSettings(yesButtonText, onYesClicked);
            IButtonSettings no = new ButtonSettings(noButtonText, onNoClicked);
            IButtonSettings cancel = new ButtonSettings(cancelButtonText, onCancelClicked);
            Instance._queue.BeginTask(Instance._confirmScreen.Init, title, description, yes, no, cancel);
        }

        public static void ShowLoading()
        {
            Instance._loadingScreen.Init();
            if (Instance._loadingStack.Contains(Instance._loadingScreen) == false)
            {
                Instance._loadingStack.Push(Instance._loadingScreen);
            }
        }

        public static void HideLoading(bool hideAllActive = false)
        {
            if (Instance._loadingStack.Count == 0) return;

            Instance._loadingStack.Pop().Close();
            if (hideAllActive == false) return;

            while (Instance._loadingStack.Count > 0)
            {
                Instance._loadingStack.Pop().Close();
            }
        }
        #endregion

        #region Access Extra Themes
        public static void Info(ThemeIndex themeIndex, string title, string description, float autoHideDuration = -1f, Action onOkayClicked = null, UnityAction onCloseClicked = null, bool okayButtonActive = true, string okayButtonText = null)
        {
            if (themeIndex == ThemeIndex.Default)
            {
                Info(title, description, autoHideDuration, onOkayClicked, onCloseClicked, okayButtonActive, okayButtonText);
                return;
            }

            if (Instance.TryGetTheme(themeIndex, out NotiTheme theme) == false) return;

            MessageScreen screen = Instance.CreateInstance(theme.messageScreen, true);
            IButtonSettings okay = new ButtonSettings(okayButtonText, okayButtonActive, onOkayClicked);
            Instance._queue.BeginTask(screen.Init, title, description, okay, onCloseClicked, autoHideDuration);
        }

        public static void Success(ThemeIndex themeIndex, string title, string description, float autoHideDuration = -1f, Action onOkayClicked = null, UnityAction onCloseClicked = null, bool okayButtonActive = true, string okayButtonText = null)
        {
            if (themeIndex == ThemeIndex.Default)
            {
                Success(title, description, autoHideDuration, onOkayClicked, onCloseClicked, okayButtonActive, okayButtonText);
                return;
            }

            if (Instance.TryGetTheme(themeIndex, out NotiTheme theme) == false) return;

            MessageScreen screen = Instance.CreateInstance(theme.successScreen, true);
            IButtonSettings okay = new ButtonSettings(okayButtonText, okayButtonActive, onOkayClicked);
            Instance._queue.BeginTask(screen.Init, title, description, okay, onCloseClicked, autoHideDuration);
        }

        public static void Error(ThemeIndex themeIndex, string title, string description, float autoHideDuration = -1f, Action onOkayClicked = null, UnityAction onCloseClicked = null, bool okayButtonActive = true, string okayButtonText = null)
        {
            if (themeIndex == ThemeIndex.Default)
            {
                Error(title, description, autoHideDuration, onOkayClicked, onCloseClicked, okayButtonActive, okayButtonText);
                return;
            }

            if (Instance.TryGetTheme(themeIndex, out NotiTheme theme) == false) return;

            MessageScreen screen = Instance.CreateInstance(theme.errorScreen, true);
            IButtonSettings okay = new ButtonSettings(okayButtonText, okayButtonActive, onOkayClicked);
            Instance._queue.BeginTask(screen.Init, title, description, okay, onCloseClicked, autoHideDuration);
        }

        public static void Confirm(ThemeIndex themeIndex, string title, string description, Action onYesClicked = null, Action onNoClicked = null, string yesButtonText = null, string noButtonText = null)
        {
            if (themeIndex == ThemeIndex.Default)
            {
                Confirm(title, description, onYesClicked, onNoClicked, yesButtonText, noButtonText);
                return;
            }

            if (Instance.TryGetTheme(themeIndex, out NotiTheme theme) == false) return;

            ConfirmScreen screen = Instance.CreateInstance(theme.confirmScreen, true);
            IButtonSettings yes = new ButtonSettings(yesButtonText, onYesClicked);
            IButtonSettings no = new ButtonSettings(noButtonText, onNoClicked);
            IButtonSettings cancel = new ButtonSettings(false);
            Instance._queue.BeginTask(screen.Init, title, description, yes, no, cancel);
        }

        public static void Confirm(ThemeIndex themeIndex, string title, string description, Action onYesClicked = null, Action onNoClicked = null, Action onCancelClicked = null, string yesButtonText = null, string noButtonText = null, string cancelButtonText = null)
        {
            if (themeIndex == ThemeIndex.Default)
            {
                Confirm(title, description, onYesClicked, onNoClicked, onCancelClicked, yesButtonText, noButtonText, cancelButtonText);
                return;
            }

            if (Instance.TryGetTheme(themeIndex, out NotiTheme theme) == false) return;

            ConfirmScreen screen = Instance.CreateInstance(theme.confirmScreen, true);
            IButtonSettings yes = new ButtonSettings(yesButtonText, onYesClicked);
            IButtonSettings no = new ButtonSettings(noButtonText, onNoClicked);
            IButtonSettings cancel = new ButtonSettings(cancelButtonText, onCancelClicked);
            Instance._queue.BeginTask(screen.Init, title, description, yes, no, cancel);
        }

        public static LoadingScreen ShowLoading(ThemeIndex themeIndex)
        {
            if (themeIndex == ThemeIndex.Default)
            {
                ShowLoading();
                return Instance._loadingScreen;
            }

            if (!Instance.TryGetTheme(themeIndex, out NotiTheme theme)) return null;

            LoadingScreen loading = Instance.CreateInstance(theme.loadingScreen, true);
            loading.Init();
            Instance._loadingStack.Push(loading);
            return loading;
        }
        #endregion

        #region Common
        public static void CloseActiveScreen()
        {
            if (Instance._loadingStack.Count > 0)
            {
                Instance._loadingStack.Pop().Close();
            }
            else if (BaseScreen.CurrentScreen)
            {
                BaseScreen.CurrentScreen.Close();
            }
        }

        public static void Show(NotificationInfo info)
        {
            if (info.themeIndex == ThemeIndex.Default)
            {
                switch (info.type)
                {
                    case NotificationInfo.Type.Info:
                        Instance._queue.BeginTask<string, string, IButtonSettings, UnityAction, float>(Instance._messageScreen.Init, info.title, info.details, info.okayButton, info.cancelButton.Callback, info.autohideDuration);
                        break;
                    case NotificationInfo.Type.Success:
                        Instance._queue.BeginTask<string, string, IButtonSettings, UnityAction, float>(Instance._successScreen.Init, info.title, info.details, info.okayButton, info.cancelButton.Callback, info.autohideDuration);
                        break;
                    case NotificationInfo.Type.Error:
                        Instance._queue.BeginTask<string, string, IButtonSettings, UnityAction, float>(Instance._errorScreen.Init, info.title, info.details, info.okayButton, info.cancelButton.Callback, info.autohideDuration);
                        break;
                    case NotificationInfo.Type.Confirm:
                        Instance._queue.BeginTask(Instance._confirmScreen.Init, info.title, info.details, info.okayButton, info.noButton, info.cancelButton);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (Instance.TryGetTheme(info.themeIndex, out NotiTheme theme))
            {
                switch (info.type)
                {
                    case NotificationInfo.Type.Info:
                        MessageScreen messageScreen = Instance.CreateInstance(theme.messageScreen, true);
                        Instance._queue.BeginTask<string, string, IButtonSettings, UnityAction, float>(messageScreen.Init, info.title, info.details, info.okayButton, info.cancelButton.Callback, info.autohideDuration);
                        break;
                    case NotificationInfo.Type.Success:
                        MessageScreen successScreen = Instance.CreateInstance(theme.successScreen, true);
                        Instance._queue.BeginTask<string, string, IButtonSettings, UnityAction, float>(successScreen.Init, info.title, info.details, info.okayButton, info.cancelButton.Callback, info.autohideDuration);
                        break;
                    case NotificationInfo.Type.Error:
                        MessageScreen errorScreen = Instance.CreateInstance(theme.errorScreen, true);
                        Instance._queue.BeginTask<string, string, IButtonSettings, UnityAction, float>(errorScreen.Init, info.title, info.details, info.okayButton, info.cancelButton.Callback, info.autohideDuration);
                        break;
                    case NotificationInfo.Type.Confirm:
                        ConfirmScreen confirmScreen = Instance.CreateInstance(theme.confirmScreen, true);
                        Instance._queue.BeginTask(confirmScreen.Init, info.title, info.details, info.okayButton, info.noButton, info.cancelButton);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        #endregion
    }
}