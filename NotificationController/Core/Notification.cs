using Omnix.DesignPatterns;
using UnityEngine;
using UnityEngine.Events;

namespace Omnix.Notification
{
    /// <summary>
    /// A singleton class that manages different notification screens
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class Notification : TaskQueueMono
    {
        private static Notification Instance;

        [SerializeField, Tooltip("Not Null, Must Be A Child. Canvas that will be used to spawn all notifications.")] private Transform canvas;
        [SerializeField, Tooltip("Not Null. Theme to use when no theme is specified.")] private NotiTheme defaultTheme;
        [SerializeField, Tooltip("Can Be Empty. Themes that you intend to use for different purposes.")] private NotiTheme[] extraThemes;

        private LoadingScreen defaultLoadingScreen;
        private MessageScreen defaultMessageScreen;
        private MessageScreen defaultSuccessScreen;
        private MessageScreen defaultErrorScreen;
        private ConfirmScreen defaultConfirmScreen;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
            InitScreens();
        }

        private void InitScreens()
        {
            defaultLoadingScreen = Instantiate(defaultTheme.LoadingScreen, canvas);
            defaultMessageScreen = Instantiate(defaultTheme.MessageScreen, canvas);
            defaultSuccessScreen = Instantiate(defaultTheme.SuccessScreen, canvas);
            defaultErrorScreen = Instantiate(defaultTheme.ErrorScreen, canvas);
            defaultConfirmScreen = Instantiate(defaultTheme.ConfirmScreen, canvas);

            defaultLoadingScreen.destroyOnHide = false;
            defaultMessageScreen.destroyOnHide = false;
            defaultSuccessScreen.destroyOnHide = false;
            defaultErrorScreen.destroyOnHide = false;
            defaultConfirmScreen.destroyOnHide = false;
            
            defaultLoadingScreen.gameObject.SetActive(false);
            defaultMessageScreen.gameObject.SetActive(false);
            defaultSuccessScreen.gameObject.SetActive(false);
            defaultErrorScreen.gameObject.SetActive(false);
            defaultConfirmScreen.gameObject.SetActive(false);
        }

        private bool TryGetTheme(int index, out NotiTheme theme)
        {
            if (index < 0 || index > extraThemes.Length)
            {
                theme = null;
                return false;
            }

            theme = extraThemes[index];
            return true;
        }

        private static T ActivateScreen<T>(T screen) where T : BaseNotificationScreen
        {
            T scr = Instantiate(screen, Instance.canvas);
            scr.destroyOnHide = true;
            Instance.BeginTask(BaseNotificationScreen.Activate, scr);
            return screen;
        }

        /// <summary> Close active screen and process next screen request in queue. </summary>
        public static void CloseActiveScreen()
        {
            BaseNotificationScreen.HideCurrent();
            Instance.TaskDone();
        }

        /// <summary> Activate or Update loading screen </summary>
        /// <param name="text"> Loading info, details of loading. </param>
        /// <param name="progress"> Progress between 0 to 100, If negative then hides progress bar.  </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which loading should auto stop. Negative or zero means stop only when <see cref="HideLoading"/> is called. </param>
        public static void ShowLoading(string text = "", float progress = 0, float autohideDuration = -1)
        {
            Instance.defaultLoadingScreen.Init(text, progress, autohideDuration);
            Instance.defaultLoadingScreen.gameObject.SetActive(true);
        }

        /// <summary> Immediately hide the loading screen. </summary>
        public static void HideLoading()
        {
            Instance.defaultLoadingScreen.gameObject.SetActive(false);
        }
        
        /// <summary> Show a message or info with default theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onOkayClicked"> Callback for okay press. </param>
        /// <param name="okayText"> Set the text for okay button. If null then keeps default text set in UI </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which this screen should close. Negative or zero means close only when user clicks on a button in the screen. </param>
        public static void Info(string title, string details, UnityAction onOkayClicked = null, string okayText = null, float autohideDuration = -1)
        {
            Instance.BeginTask(Instance.defaultMessageScreen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
        }

        /// <summary> Show a message or info with one of the extra theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onOkayClicked"> Callback for okay press. </param>
        /// <param name="okayText"> Set the text for okay button. If null then keeps default text set in UI </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which this screen should close. Negative or zero means close only when user clicks on a button in the screen. </param>
        public static void Info(int themeIndex, string title, string details, UnityAction onOkayClicked = null, string okayText = null, float autohideDuration = -1)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
            {
                var screen = Instantiate(theme.MessageScreen, Instance.canvas);
                screen.destroyOnHide = true;
                Instance.BeginTask(screen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
            }
            else
            {
                Instance.BeginTask(Instance.defaultMessageScreen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
            }
        }

        /// <summary> Show a success message with default theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onOkayClicked"> Callback for okay press. </param>
        /// <param name="okayText"> Set the text for okay button. If null then keeps default text set in UI </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which this screen should close. Negative or zero means close only when user clicks on a button in the screen. </param>
        public static void Success(string title, string details, UnityAction onOkayClicked = null, string okayText = null, float autohideDuration = -1)
        {
            Instance.BeginTask(Instance.defaultSuccessScreen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
        }

        /// <summary> Show a success message with one of the extra theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onOkayClicked"> Callback for okay press. </param>
        /// <param name="okayText"> Set the text for okay button. If null then keeps default text set in UI </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which this screen should close. Negative or zero means close only when user clicks on a button in the screen. </param>
        public static void Success(int themeIndex, string title, string details, UnityAction onOkayClicked = null, string okayText = null, float autohideDuration = -1)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
            {
                var screen = Instantiate(theme.SuccessScreen, Instance.canvas);
                screen.destroyOnHide = true;
                Instance.BeginTask(screen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
            }
            else
            {
                Instance.BeginTask(Instance.defaultSuccessScreen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
            }
        }

        /// <summary> Show a error message with default theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onOkayClicked"> Callback for okay press. </param>
        /// <param name="okayText"> Set the text for okay button. If null then keeps default text set in UI </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which this screen should close. Negative or zero means close only when user clicks on a button in the screen. </param>
        public static void Error(string title, string details, UnityAction onOkayClicked = null, string okayText = null, float autohideDuration = -1)
        {
            Instance.BeginTask(Instance.defaultErrorScreen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
        }

        /// <summary> Show a error message with one of the extra theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onOkayClicked"> Callback for okay press. </param>
        /// <param name="okayText"> Set the text for okay button. If null then keeps default text set in UI </param>
        /// <param name="autohideDuration"> Duration (in seconds) after which this screen should close. Negative or zero means close only when user clicks on a button in the screen. </param>
        public static void Error(int themeIndex, string title, string details, UnityAction onOkayClicked = null, string okayText = null, float autohideDuration = -1)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
            {
                var screen = Instantiate(theme.ErrorScreen, Instance.canvas);
                screen.destroyOnHide = true;
                Instance.BeginTask(screen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
            }
            else
            {
                Instance.BeginTask(Instance.defaultErrorScreen.Init, title, details, new ButtonConfig(okayText, onOkayClicked), autohideDuration);
            }
        }

        /// <summary> Ask a yes-no question with default theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onYesClicked"> Callback for yes press. </param>
        /// <param name="onNoClicked"> Callback for no press. </param>
        /// <param name="yesText"> Set the text for yes button. If null then keeps default text set in UI </param>
        /// <param name="noText"> Set the text for no button. If null then keeps default text set in UI </param>
        public static void Confirm(string title, string details, UnityAction onYesClicked = null, UnityAction onNoClicked = null, string yesText = null, string noText = null)
        {
            Instance.BeginTask(Instance.defaultConfirmScreen.Init, title, details, new ButtonConfig(yesText, onYesClicked), new ButtonConfig(noText, onNoClicked), ButtonConfig.Inactive);
        }

        /// <summary> Ask a yes-no question one of the extra theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onYesClicked"> Callback for yes press. </param>
        /// <param name="onNoClicked"> Callback for no press. </param>
        /// <param name="yesText"> Set the text for yes button. If null then keeps default text set in UI </param>
        /// <param name="noText"> Set the text for no button. If null then keeps default text set in UI </param>
        public static void Confirm(int themeIndex, string title, string details, UnityAction onYesClicked = null, UnityAction onNoClicked = null, string yesText = null, string noText = null)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
            {
                var screen = Instantiate(theme.ConfirmScreen, Instance.canvas);
                screen.destroyOnHide = true;
                Instance.BeginTask(screen.Init, title, details, new ButtonConfig(yesText, onYesClicked), new ButtonConfig(noText, onNoClicked), ButtonConfig.Inactive);
            }
            else
            {
                Instance.BeginTask(Instance.defaultConfirmScreen.Init, title, details, new ButtonConfig(yesText, onYesClicked), new ButtonConfig(noText, onNoClicked), ButtonConfig.Inactive);
            }
        }

        /// <summary> Ask a yes-no-cancel question with default theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onYesClicked"> Callback for yes press. </param>
        /// <param name="onNoClicked"> Callback for no press. </param>
        /// <param name="onCancelClicked"> Callback for cancel press. </param>
        /// <param name="yesText"> Set the text for yes button. If null then keeps default text set in UI </param>
        /// <param name="noText"> Set the text for no button. If null then keeps default text set in UI </param>
        /// <param name="cancelText"> Set the text for cancel button. If null then keeps default text set in UI </param>
        public static void Confirm(string title, string details, UnityAction onYesClicked = null, UnityAction onNoClicked = null, UnityAction onCancelClicked = null, string yesText = null, string noText = null, string cancelText = null)
        {
            Instance.BeginTask(Instance.defaultConfirmScreen.Init, title, details, new ButtonConfig(yesText, onYesClicked), new ButtonConfig(noText, onNoClicked), new ButtonConfig(cancelText, onCancelClicked));
        }

        /// <summary> Show a error message with one of the extra theme. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <param name="title"> Title </param>
        /// <param name="details"> Details </param>
        /// <param name="onYesClicked"> Callback for yes press. </param>
        /// <param name="onNoClicked"> Callback for no press. </param>
        /// <param name="onCancelClicked"> Callback for cancel press. </param>
        /// <param name="yesText"> Set the text for yes button. If null then keeps default text set in UI </param>
        /// <param name="noText"> Set the text for no button. If null then keeps default text set in UI </param>
        /// <param name="cancelText"> Set the text for cancel button. If null then keeps default text set in UI </param>
        public static void Confirm(int themeIndex, string title, string details, UnityAction onYesClicked = null, UnityAction onNoClicked = null, UnityAction onCancelClicked = null, string yesText = null, string noText = null, string cancelText = null)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
            {
                var screen = Instantiate(theme.ConfirmScreen, Instance.canvas);
                screen.destroyOnHide = true;
                Instance.BeginTask(screen.Init, title, details, new ButtonConfig(yesText, onYesClicked), new ButtonConfig(noText, onNoClicked), new ButtonConfig(cancelText, onCancelClicked));
            }
            else
            {
                Instance.BeginTask(Instance.defaultConfirmScreen.Init, title, details, new ButtonConfig(yesText, onYesClicked), new ButtonConfig(noText, onNoClicked), new ButtonConfig(cancelText, onCancelClicked));
            }
        }

        /// <summary> Show a message or info with default theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <returns> <see cref="MessageScreen"/> that's the entry point of method chain. </returns>
        public static MessageScreen Info()
        {
            return ActivateScreen(Instance.defaultTheme.MessageScreen);
        }

        /// <summary> Show a message or info with one of the extra theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <returns> <see cref="MessageScreen"/> that's the entry point of method chain. </returns>
        public static MessageScreen Info(int themeIndex)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
                return ActivateScreen(theme.MessageScreen);
            return ActivateScreen(Instance.defaultTheme.MessageScreen);
        }

        /// <summary> Show a success message with default theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <returns> <see cref="MessageScreen"/> that's the entry point of method chain. </returns>
        public static MessageScreen Success()
        {
            return ActivateScreen(Instance.defaultTheme.SuccessScreen);
        }

        /// <summary> Show a success message with one of the extra theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <returns> <see cref="MessageScreen"/> that's the entry point of method chain. </returns>
        public static MessageScreen Success(int themeIndex)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
                return ActivateScreen(theme.SuccessScreen);
            return ActivateScreen(Instance.defaultTheme.SuccessScreen);
        }

        /// <summary> Show a error message with default theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <returns> <see cref="MessageScreen"/> that's the entry point of method chain. </returns>
        public static MessageScreen Error()
        {
            return ActivateScreen(Instance.defaultTheme.ErrorScreen);
        }

        /// <summary> Show a error message with one of the extra theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <returns> <see cref="MessageScreen"/> that's the entry point of method chain. </returns>
        public static MessageScreen Error(int themeIndex)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
                return ActivateScreen(theme.ErrorScreen);
            return ActivateScreen(Instance.defaultTheme.ErrorScreen);
        }

        /// <summary> Ask a question (could be yes only, yes-no type or yes-no-cancel type) with default theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <returns> <see cref="ConfirmScreen"/> that's the entry point of method chain. </returns>
        public static ConfirmScreen Confirm()
        {
            return ActivateScreen(Instance.defaultTheme.ConfirmScreen);
        }

        /// <summary> Ask a question (could be yes only, yes-no type or yes-no-cancel type) one of the extra theme. Use this if you like Method Chaining. </summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        /// <param name="themeIndex"> Index of extraThemes to be used. If out of range of extraThemes, then uses default theme </param>
        /// <returns> <see cref="ConfirmScreen"/> that's the entry point of method chain. </returns>
        public static ConfirmScreen Confirm(int themeIndex)
        {
            if (Instance.TryGetTheme(themeIndex, out var theme))
                return ActivateScreen(theme.ConfirmScreen);
            return ActivateScreen(Instance.defaultTheme.ConfirmScreen);
        }

        /// <summary> Show a message/success/error or ask a question to user based on <see cref="NotificationInfo.type"/> of <see cref="NotificationInfo"/></summary>
        /// <remarks> This will add a request in the queue, instead of immediately activating the UI. So the user will see one UI at a time even if multiple requests are send simultaneously. </remarks>
        public static void Show(NotificationInfo info)
        {
            switch (info.type)
            {
                case NotificationInfo.Type.Info:
                    Instance.BeginTask(Instance.defaultMessageScreen.Init, info.title, info.details, info.okayButton, info.autohideDuration);
                    break;
                case NotificationInfo.Type.Success:
                    Instance.BeginTask(Instance.defaultSuccessScreen.Init, info.title, info.details, info.okayButton, info.autohideDuration);
                    break;
                case NotificationInfo.Type.Error:
                    Instance.BeginTask(Instance.defaultErrorScreen.Init, info.title, info.details, info.okayButton, info.autohideDuration);
                    break;
                case NotificationInfo.Type.Confirm:
                    Instance.BeginTask(Instance.defaultConfirmScreen.Init, info.title, info.details, info.okayButton, info.noButton, info.cancelButton);
                    break;
            }
        }
    }
}