using JetBrains.Annotations;
using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Transitions
{
    [GroupProperties(_.EndPoints, "loadStartScale", "unloadEndScale")]
    public class ScaleTransition : BaseTransitionBlendable
    {
        [SerializeField] Vector3 loadStartScale;
        [SerializeField] Vector3 unloadEndScale;

        private Vector3 _loadEndPoint;
        private Vector3 _unloadStartScale;

        public override void Prepare([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load)
        {
            if (load)
            {
                _loadEndPoint = load.transform.localScale;
                load.transform.localScale = loadStartScale;
            }

            if (unload)
            {
                _unloadStartScale = unload.transform.localScale;
            }
        }

        public override void SetLoadingFrame([NotNull] BaseMenu load, float t, bool playingInReversed)
        {
            load.transform.localScale = Vector2.LerpUnclamped(loadStartScale, _loadEndPoint, t);
        }

        public override void SetUnloadingFrame([NotNull] BaseMenu unload, float t, bool playingInReversed)
        {
            unload.transform.localScale = Vector2.LerpUnclamped(_unloadStartScale, unloadEndScale, t);
        }
    }
}