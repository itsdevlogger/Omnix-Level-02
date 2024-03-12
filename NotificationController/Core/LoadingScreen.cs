using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Notification
{
    public class LoadingScreen : BaseNotificationScreen
    {
        [SerializeField, CanBeNull] private Slider progressBar;
        [SerializeField, CanBeNull] private Transform objectToRotate;
        [SerializeField] private Vector3 rotateSpeed;

        private void Awake()
        {
            enabled = objectToRotate != null;
        }

        private void Update()
        {
            if (objectToRotate == null)
            {
                enabled = false;
                return;
            }

            objectToRotate.Rotate(rotateSpeed * Time.deltaTime);
        }

        internal void Init(string title, float value, float autoHideDuration)
        {
            destroyOnHide = false;
            if (titleText != null) titleText.text = title;
            if (progressBar != null)
            {
                if (value > 0)
                {
                    progressBar.minValue = 0f;
                    progressBar.maxValue = 100f;
                    progressBar.gameObject.SetActive(true);
                    progressBar.value = value;
                }
                else
                {
                    progressBar.gameObject.SetActive(false);
                }
            }

            Activate(this);
            if (autoHideDuration >= 0) Close(autoHideDuration);
        }

        public LoadingScreen Title(string value)
        {
            if (titleText != null) titleText.text = value;
            return this;
        }

        public LoadingScreen Progress(float value)
        {
            if (progressBar != null) progressBar.value = value;
            return this;
        }
    }
}