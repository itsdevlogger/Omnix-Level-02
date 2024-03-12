using System;
using TMPro;
using UnityEngine.UI;

namespace Omnix.Notification
{
    [Serializable]
    public class ButtonAndText
    {
        public Button button;
        public TextMeshProUGUI textMesh;
        public string defaultText;
    }
}