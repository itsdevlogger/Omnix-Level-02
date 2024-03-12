using JetBrains.Annotations;
using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Transitions
{
    [GroupProperties(_.EndPoints, "loadStartAlpha", "unloadEndAlpha")]
    public class FadeTransition : BaseTransitionBlendable
    {
        [SerializeField, Range(0f, 1f)] private float loadStartAlpha;
        [SerializeField, Range(0f, 1f)] private float unloadEndAlpha;

        private float _startAlphaUnload, _endAlphaLoad;
        private bool _destroyCanvasUnload = false;
        private bool _destroyCanvasLoad = false;
        private CanvasGroup _canvasUnload, _canvasLoad;

        public override void Prepare([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load)
        {
            if (unload)
            {
                _canvasUnload = unload.GetComponent<CanvasGroup>();
                if (_canvasUnload == null)
                {
                    _destroyCanvasUnload = true;
                    _canvasUnload = unload.gameObject.AddComponent<CanvasGroup>();
                    _canvasUnload.alpha = 1f;
                }

                _startAlphaUnload = _canvasUnload.alpha;
                unloadEndAlpha = 0f;
            }

            if (load)
            {
                _canvasLoad = load.GetComponent<CanvasGroup>();
                if (_canvasLoad == null)
                {
                    _destroyCanvasLoad = true;
                    _canvasLoad = load.gameObject.AddComponent<CanvasGroup>();
                    _canvasLoad.alpha = 0f;
                }

                loadStartAlpha = _canvasLoad.alpha;
                _endAlphaLoad = 1f;
            }
        }

        public override void SetUnloadingFrame(BaseMenu unload, float t, bool playingInReversed)
        {
            _canvasUnload.alpha = Mathf.LerpUnclamped(_startAlphaUnload, unloadEndAlpha, t);
        }

        public override void SetLoadingFrame(BaseMenu load, float t, bool playingInReversed)
        {
            _canvasLoad.alpha = Mathf.LerpUnclamped(loadStartAlpha, _endAlphaLoad, t);
        }

        public override void Cleanup(BaseMenu unload, BaseMenu load)
        {
            base.Cleanup(unload, load);
            if (_destroyCanvasUnload) Destroy(_canvasUnload);
            if (_destroyCanvasLoad) Destroy(_canvasLoad);
        }
    }
}