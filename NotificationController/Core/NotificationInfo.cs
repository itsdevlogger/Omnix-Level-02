using System;



namespace Omnix.Notification
{
    [Serializable]
    public class NotificationInfo
    {
        public enum Type
        {
            Info = 0,
            Success = 1,
            Error = 2,
            Confirm = 3
        }

        public Type type;
        public string title;
        public string details;
        public ThemeIndex themeIndex = ThemeIndex.Default;
        public float autohideDuration;
        public ButtonSettingsSerializable okayButton = new ButtonSettingsSerializable() { Text = "Okay" };
        public ButtonSettingsSerializable noButton = new ButtonSettingsSerializable() { Text = "No" };
        public ButtonSettingsSerializable cancelButton = new ButtonSettingsSerializable() { Text = "Cancel" };
    }
}