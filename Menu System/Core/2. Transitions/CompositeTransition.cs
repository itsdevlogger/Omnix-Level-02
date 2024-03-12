using JetBrains.Annotations;
using MenuManagement.Base;
using UnityEngine;


namespace MenuManagement.Transitions
{
    public class CompositeTransition : BaseTransitionBlendable
    {
        [SerializeField] private BaseTransitionBlendable[] transitions;

        public override void Prepare([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load)
        {
            foreach (BaseTransitionBlendable trans in transitions)
            {
                trans.Prepare(unload, load);
            }
        }

        public override void SetLoadingFrame([NotNull] BaseMenu load, float t, bool playingInReversed)
        {
            foreach (BaseTransitionBlendable trans in transitions)
            {
                trans.SetLoadingFrame(load, t, playingInReversed);
            }
        }

        public override void SetUnloadingFrame([NotNull] BaseMenu unload, float t, bool playingInReversed)
        {
            foreach (BaseTransitionBlendable trans in transitions)
            {
                trans.SetUnloadingFrame(unload, t, playingInReversed);
            }
        }
    }
}