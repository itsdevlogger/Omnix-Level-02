using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace MenuManagement.Base
{
    /// <summary>
    /// Base class for menu transition that can 
    /// </summary>
    public abstract class BaseTransitionBlendable : MonoBehaviour, IMenuTransition
    {
        [SerializeField] protected float transitionDuration = 0.25f;

        [SerializeField, Range(-1f, 1f), Tooltip("-1 meaning In-Anim completes then Out-Anim starts, 0 meaning both start together and complete together, +1 meaning Out-Anim completes then In-Anim starts")]
        protected float inOutBlend = 0f;

        [SerializeField, Tooltip("Menu transition will be reversed while moving up this list.")]
        protected TransitionEase ease;

        [SerializeField, Tooltip("Menu transition will be reversed while moving up this list.")]
        protected BaseMenu[] transitionOrders;


        /// <summary> Prepare for a transition </summary>
        public abstract void Prepare([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load);

        /// <summary> Set properties of unloading menu based on the current progress in transition </summary>
        /// <param name="unload"> menu that we are unloading </param>
        /// <param name="t"> use this to learp, goes from 0 to 1 as the animation progresses</param>
        /// <param name="playingInReversed"> If true, then learp from unloadStartPoint to loadStartPoint, otherwise learp from unloadStartPoint to unloadEndPoint </param>
        public abstract void SetUnloadingFrame([NotNull] BaseMenu unload, float t, bool playingInReversed);

        /// <summary> Set properties of loading menu based on the current progress in transition </summary>
        /// <param name="load"> menu that we are loading </param>
        /// <param name="t"> use this to learp, goes from 0 to 1 as the animation progresses </param>
        /// <param name="playingInReversed"> If true, then learp from unloadEndPoint to loadEndPoint, otherwise learp from loadStartPoint to loadEndPoint </param>
        public abstract void SetLoadingFrame([NotNull] BaseMenu load, float t, bool playingInReversed);

        /// <summary>
        /// Animate transition between menus.
        /// </summary>
        /// <returns> null if both load & unload are null. </returns>
        public IEnumerator Animate(BaseMenu unload, BaseMenu load)
        {
            ease.Init();
            Prepare(unload, load);

            bool loadNotNull = (load != null);
            bool unloadNotNull = (unload != null);

            if (loadNotNull && unloadNotNull)
            {
                if (inOutBlend >= 0) return AnimateBothUnloadFirst(unload, load);
                return AnimateBothLoadFirst(unload, load);
            }

            if (loadNotNull) return AnimateSingle(load, SetLoadingFrame);
            if (unloadNotNull) return AnimateSingle(unload, SetUnloadingFrame);
            return null;
        }

        public virtual void Cleanup(BaseMenu unload, BaseMenu load)
        {
            if (unload) SetUnloadingFrame(unload, 0f, false); // _playingInReverse must be false cause sometimes loadStartPoint can be null
            if (load) SetLoadingFrame(load, 1f, false); // _playingInReverse must be false cause sometimes unloadEndPoint can be null
        }

        private bool ShouldReverseTransition([NotNull] BaseMenu unload, [NotNull] BaseMenu load)
        {
            int loadIndex = -1;
            int unloadIndex = -1;

            int index = 0;
            foreach (var menu in transitionOrders)
            {
                if (menu == load) loadIndex = index;
                if (menu == unload) unloadIndex = index;

                if (loadIndex != -1 && unloadIndex != -1) return loadIndex < unloadIndex;
                index++;
            }

            return false;
        }

        private IEnumerator AnimateSingle([NotNull] BaseMenu menu, Action<BaseMenu, float, bool> frameSetter)
        {
            float timeElapsed = 0f;
            while (timeElapsed < transitionDuration)
            {
                frameSetter(menu, ease.Value(timeElapsed, transitionDuration), false);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            frameSetter(menu, 1f, false);
        }

        private IEnumerator AnimateBothUnloadFirst([NotNull] BaseMenu unload, [NotNull] BaseMenu load)
        {
            bool playingInReverse = ShouldReverseTransition(unload, load);
            float blend = inOutBlend;
            float inAnimDelay = transitionDuration * blend * 0.5f;
            float eachAnimLength = transitionDuration - inAnimDelay;

            SetLoadingFrame(load, 0f, playingInReverse);
            SetUnloadingFrame(unload, 0f, playingInReverse);
            yield return null;

            float timeElapsed = Time.deltaTime;
            while (timeElapsed < inAnimDelay)
            {
                SetUnloadingFrame(unload, ease.Value(timeElapsed, eachAnimLength), playingInReverse);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            while (timeElapsed < eachAnimLength)
            {
                SetUnloadingFrame(unload, ease.Value(timeElapsed, eachAnimLength), playingInReverse);
                SetLoadingFrame(load, ease.Value(timeElapsed - inAnimDelay, eachAnimLength), playingInReverse);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            SetUnloadingFrame(unload, 1f, playingInReverse);

            while (timeElapsed < transitionDuration)
            {
                SetLoadingFrame(load, ease.Value((timeElapsed - inAnimDelay), eachAnimLength), playingInReverse);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            SetLoadingFrame(load, 1f, playingInReverse);
        }

        private IEnumerator AnimateBothLoadFirst([NotNull] BaseMenu unload, [NotNull] BaseMenu load)
        {
            bool playingInReverse = ShouldReverseTransition(unload, load);

            float outAnimDelay = transitionDuration * inOutBlend * -0.5f;
            float eachAnimLength = transitionDuration - outAnimDelay;

            SetLoadingFrame(load, 0f, playingInReverse);
            SetUnloadingFrame(unload, 0f, playingInReverse);
            yield return null;

            float timeElapsed = Time.deltaTime;
            while (timeElapsed < outAnimDelay)
            {
                SetLoadingFrame(load, ease.Value(timeElapsed, eachAnimLength), playingInReverse);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            while (timeElapsed < eachAnimLength)
            {
                SetLoadingFrame(load, ease.Value(timeElapsed, eachAnimLength), playingInReverse);
                SetUnloadingFrame(unload, ease.Value((timeElapsed - outAnimDelay), eachAnimLength), playingInReverse);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            SetLoadingFrame(load, 1f, playingInReverse);

            while (timeElapsed < transitionDuration)
            {
                SetUnloadingFrame(unload, ease.Value((timeElapsed - outAnimDelay), eachAnimLength), playingInReverse);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        
            SetUnloadingFrame(unload, 1f, playingInReverse);
        }
    }
}