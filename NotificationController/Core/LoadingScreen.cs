using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Notification
{
    public class LoadingScreen : BaseScreen
    {
        [SerializeField, Tooltip("[Can Be Null] Text that represents title of this window"), CanBeNull]
        private TextMeshProUGUI _title;

        [SerializeField, Tooltip("[Can Be Null] Progress bar"), CanBeNull]
        private Slider _progressBar;

        [Space, Header("Rotor")] [SerializeField, Tooltip("[Can Be Null] Object to rotate"), CanBeNull]
        private Transform _rotor;

        [SerializeField, Tooltip("Decide look and feel of rotation animation")]
        private AnimationCurve _speedCurve;

        [SerializeField, Tooltip("Animation Length (in seconds)")]
        private float _animationLength;

        [SerializeField, Tooltip("Speed up animation")]
        private float _speedMultiplier = -1f;

        /// <summary> Normalized (0 to 1) progress </summary>
        public float Progress
        {
            get
            {
                if (_progressBar != null) return _progressBar.normalizedValue;
                return 0f;
            }
            set
            {
                if (_progressBar == null) return;
                _progressBar.normalizedValue = value;
            }
        }

        public string Text
        {
            get => (_title == null) ? "" : _title.text;
            set
            {
                if (_title) _title.SetText(value);
            }
        }

        /// <summary> Time left for progress bar to reach 1 (set -1 if you want to manually set the progress) </summary>
        [NonSerialized] public float progressAutoFillTime = -1;

        private void Update()
        {
            if (_rotor != null)
            {
                float sing = Mathf.Sign(_speedMultiplier);
                float t = Mathf.Repeat(Mathf.Abs(_speedMultiplier) * Time.time / _animationLength, 1f);
                float rotationAngle = sing * _speedCurve.Evaluate(t) * 360f;
                Vector3 currentEulerAngles = transform.rotation.eulerAngles;
                _rotor.rotation = Quaternion.Euler(currentEulerAngles.x, currentEulerAngles.y, rotationAngle);
            }

            if (_progressBar && progressAutoFillTime > 0)
            {
                _progressBar.normalizedValue += (1f - _progressBar.normalizedValue) * Time.deltaTime / progressAutoFillTime;
            }
        }

        public void Init()
        {
            // Do not call ActivateScreen
            // As loading screen is out of NotiCon queue
            // Meaning, loading screen can be activated while another screen is active.
            gameObject.SetActive(true);
            progressAutoFillTime = -1;
            Progress = 0f;
            Text = null;
        }
    }
}