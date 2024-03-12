using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Transitions
{
    [GroupProperties(_.EndPoints, "loadStartSize", "unloadEndSize")]
    public class ResizeTransition : BaseTransitionBlendable
    {
        [SerializeField] private Vector2 loadStartSize;
        [SerializeField] private Vector2 unloadEndSize;

        private RectTransform _unloadMenuRect;
        private RectTransform _loadMenuRect;
        private Vector2 _unloadMenuStartPoint;
        private Vector2 _loadMenuEndPoint;

        public override void Prepare(BaseMenu unload, BaseMenu load)
        {
            if (unload)
            {
                _unloadMenuRect = unload.GetComponent<RectTransform>();
                _unloadMenuStartPoint = _unloadMenuRect.sizeDelta;
            }
            if (load)
            {
                _loadMenuRect = load.GetComponent<RectTransform>();
                _loadMenuEndPoint = _loadMenuRect.sizeDelta;
                _loadMenuRect.sizeDelta = loadStartSize;
            }
        }

        public override void SetLoadingFrame(BaseMenu load, float t, bool playingInReversed)
        {
            if (playingInReversed)
            {
                _loadMenuRect.sizeDelta = Vector2.LerpUnclamped(unloadEndSize, _loadMenuEndPoint, t);
            }
            else
            {
                _loadMenuRect.sizeDelta = Vector2.LerpUnclamped(loadStartSize, _loadMenuEndPoint, t);
            }
        }

        public override void SetUnloadingFrame(BaseMenu unload, float t, bool playingInReversed)
        {
            if (playingInReversed)
            {
                _unloadMenuRect.sizeDelta = Vector2.LerpUnclamped(_unloadMenuStartPoint, loadStartSize, t);
            }
            else
            {
                _unloadMenuRect.sizeDelta = Vector2.LerpUnclamped(_unloadMenuStartPoint, unloadEndSize, t);
            }
        }
    }
}