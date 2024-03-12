using System;
using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Transitions
{
    public class TransformTransition : BaseTransitionBlendable
    {
        [Serializable]
        private class ExtremeHolder<T>
        {
            public T loadStart;
            public T loadEnd;
            public T unloadStart;
            public T unloadEnd;
        }

        private class Targets
        {
            public RectTransform load;
            public RectTransform unload;
        }


        [SerializeField] private TransformTransitionTargets targets; // needed for the editor to work properly
        [SerializeField] private ExtremeHolder<RectTransform> extreme;

        private Targets target;
        private ExtremeHolder<Transform> parents;
        private bool destroyLoadEnd;
        private bool destroyUnloadStart;
        private event Action<float, RectTransform> LoadingSetter;
        private event Action<float, RectTransform> UnloadingSetter;
        private TransformTransitionTargets settersTarget;


        [InspectorButton("Recreate load & unload points")]
        private void RecreateLoadUnload()
        {
            if (extreme.loadStart != null) DestroyImmediate(extreme.loadStart.gameObject);
            if (extreme.unloadEnd != null) DestroyImmediate(extreme.unloadEnd.gameObject);
            Reset();
        }

        private void Reset()
        {
            extreme = new ExtremeHolder<RectTransform>();
            extreme.loadStart = new GameObject("Loading Start Point").AddComponent<RectTransform>();
            extreme.unloadEnd = new GameObject("Unload End Point").AddComponent<RectTransform>();

            extreme.loadStart.SetParent(transform);
            extreme.unloadEnd.SetParent(transform);

            extreme.loadStart.localPosition = Vector3.zero;
            extreme.unloadEnd.localPosition = Vector3.zero;
        }

        private void UpdateSetters()
        {
            if (Application.isPlaying && settersTarget == targets) return;

            if (targets.HasFlag(TransformTransitionTargets.LocalPosition))
            {
                LoadingSetter += LoadingLocalPosition;
                UnloadingSetter += UnloadingLocalPosition;
            }
            else if (targets.HasFlag(TransformTransitionTargets.GlobalPosition))
            {
                LoadingSetter += LoadingGlobalPosition;
                UnloadingSetter += UnloadingGlobalPosition;
            }
            else
            {
                if (targets.HasFlag(TransformTransitionTargets.RectAnchorPosition))
                {
                    LoadingSetter += LoadingRectAnchorPosition;
                    UnloadingSetter += UnloadingRectAnchorPosition;
                }

                if (targets.HasFlag(TransformTransitionTargets.RectPivot))
                {
                    LoadingSetter += LoadingRectPivot;
                    UnloadingSetter += UnloadingRectPivot;
                }

                if (targets.HasFlag(TransformTransitionTargets.RectAnchorMin))
                {
                    LoadingSetter += LoadingRectAnchorMin;
                    UnloadingSetter += UnloadingRectAnchorMin;
                }

                if (targets.HasFlag(TransformTransitionTargets.RectAnchorMax))
                {
                    LoadingSetter += LoadingRectAnchorMax;
                    UnloadingSetter += UnloadingRectAnchorMax;
                }
            }


            if (targets.HasFlag(TransformTransitionTargets.LocalRotation))
            {
                LoadingSetter += LoadingLocalRotation;
                UnloadingSetter += UnloadingLocalRotation;
            }
            else if (targets.HasFlag(TransformTransitionTargets.GlobalRotation))
            {
                LoadingSetter += LoadingGlobalRotation;
                UnloadingSetter += UnloadingGlobalRotation;
            }


            if (targets.HasFlag(TransformTransitionTargets.Scale))
            {
                LoadingSetter += LoadingScale;
                UnloadingSetter += UnloadingScale;
            }

            if (targets.HasFlag(TransformTransitionTargets.RectSizeDelta))
            {
                LoadingSetter += LoadingRectSizeDelta;
                UnloadingSetter += UnloadingRectSizeDelta;
            }

            settersTarget = targets;
        }

        private static RectTransform DuplicateTransform(RectTransform source)
        {
            RectTransform target = new GameObject().AddComponent<RectTransform>();
            target.SetParent(source.parent);
            target.position = source.position;
            target.rotation = source.rotation;
            target.localScale = source.localScale;
            target.sizeDelta = source.sizeDelta;
            target.anchoredPosition = source.anchoredPosition;
            target.pivot = source.pivot;
            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            return target;
        }

        // private static bool failSafe = false;
        public override void Prepare(BaseMenu unload, BaseMenu load)
        {
            /*string unN = unload == null ? null : unload.name;
            string loN = load == null ? null : load.name;
            Debug.Log($"Called Prepare({unN}, {loN})");
            if (failSafe) return;
            failSafe = true;*/
            
            UpdateSetters();
            target = new Targets();
            parents = new ExtremeHolder<Transform>();

            if (load != null)
            {
                target.load = load.GetComponent<RectTransform>();

                Transform loadParent = target.load.parent;
                parents.loadStart = extreme.loadStart.parent;
                extreme.loadStart.SetParent(loadParent);
                destroyLoadEnd = extreme.loadEnd == null;

                if (destroyLoadEnd)
                {
                    extreme.loadEnd = DuplicateTransform(target.load);
                }
                else
                {
                    parents.loadEnd = extreme.loadEnd.parent;
                    extreme.loadEnd.SetParent(target.load.parent);
                }
            }

            if (unload != null)
            {
                target.unload = unload.GetComponent<RectTransform>();

                Transform unloadParent = target.unload.parent;
                parents.unloadEnd = extreme.unloadEnd.parent;
                extreme.unloadEnd.SetParent(unloadParent);
                destroyUnloadStart = extreme.unloadStart == null;

                if (destroyUnloadStart)
                {
                    extreme.unloadStart = DuplicateTransform(target.unload);
                }
                else
                {
                    parents.unloadStart = extreme.unloadStart.parent;
                    extreme.unloadStart.SetParent(target.unload.parent);
                }
            }
        }

        public override void SetLoadingFrame(BaseMenu load, float t, bool playingInReversed)
        {
            LoadingSetter?.Invoke(t, playingInReversed ? extreme.unloadEnd : extreme.loadStart);
        }

        public override void SetUnloadingFrame(BaseMenu unload, float t, bool playingInReversed)
        {
            UnloadingSetter?.Invoke(t, playingInReversed ? extreme.loadStart : extreme.unloadEnd);
        }

        public override void Cleanup(BaseMenu unload, BaseMenu load)
        {
            base.Cleanup(unload, load);

            if (target.load != null && extreme.loadStart != null)
            {
                extreme.loadStart.SetParent(parents.loadStart);
            }

            if (target.unload != null && extreme.unloadEnd != null)
            {
                extreme.unloadEnd.SetParent(parents.unloadEnd);
            }

            if (target.load != null && extreme.loadEnd != null)
            {
                if (destroyLoadEnd)
                {
                    DestroyImmediate(extreme.loadEnd.gameObject);
                }
                else
                {
                    extreme.loadEnd.SetParent(parents.loadEnd);
                }
            }

            if (target.unload != null && extreme.unloadStart != null)
            {
                if (destroyUnloadStart)
                {
                    DestroyImmediate(extreme.unloadStart.gameObject);
                }
                else
                {
                    extreme.unloadStart.SetParent(parents.unloadStart);
                }
            }
        }


        // @formatter:off
        private void LoadingLocalPosition(float t, RectTransform startBasedOnReverse)      => target.load.localPosition = Vector3.LerpUnclamped(startBasedOnReverse.localPosition, extreme.loadEnd.localPosition, t);
        private void LoadingGlobalPosition(float t, RectTransform startBasedOnReverse)     => target.load.position         = Vector3.LerpUnclamped   (startBasedOnReverse.position,         extreme.loadEnd.position,         t);
        private void LoadingLocalRotation(float t, RectTransform startBasedOnReverse)      => target.load.localRotation    = Quaternion.LerpUnclamped(startBasedOnReverse.localRotation,    extreme.loadEnd.localRotation,    t);
        private void LoadingGlobalRotation(float t, RectTransform startBasedOnReverse)     => target.load.rotation         = Quaternion.LerpUnclamped(startBasedOnReverse.rotation,         extreme.loadEnd.rotation,         t);
        private void LoadingScale(float t, RectTransform startBasedOnReverse)              => target.load.localScale       = Vector3.LerpUnclamped   (startBasedOnReverse.localScale,       extreme.loadEnd.localScale,       t);
        private void LoadingRectSizeDelta(float t, RectTransform startBasedOnReverse)      => target.load.sizeDelta        = Vector2.LerpUnclamped   (startBasedOnReverse.sizeDelta,        extreme.loadEnd.sizeDelta,        t);
        private void LoadingRectAnchorPosition(float t, RectTransform startBasedOnReverse) => target.load.anchoredPosition = Vector2.LerpUnclamped   (startBasedOnReverse.anchoredPosition, extreme.loadEnd.anchoredPosition, t);
        private void LoadingRectPivot(float t, RectTransform startBasedOnReverse)          => target.load.pivot            = Vector2.LerpUnclamped   (startBasedOnReverse.pivot,            extreme.loadEnd.pivot,            t);
        private void LoadingRectAnchorMin(float t, RectTransform startBasedOnReverse)      => target.load.anchorMin        = Vector2.LerpUnclamped   (startBasedOnReverse.anchorMin,        extreme.loadEnd.anchorMin,        t);
        private void LoadingRectAnchorMax(float t, RectTransform startBasedOnReverse)      => target.load.anchorMax        = Vector2.LerpUnclamped   (startBasedOnReverse.anchorMax,        extreme.loadEnd.anchorMax,        t);
        
        private void UnloadingLocalPosition(float t, RectTransform endBasedOnReverse)      => target.unload.localPosition = Vector3.LerpUnclamped(extreme.unloadStart.localPosition, endBasedOnReverse.localPosition, t);
        private void UnloadingGlobalPosition(float t, RectTransform endBasedOnReverse)     => target.unload.position         = Vector3.LerpUnclamped   (extreme.unloadStart.position,         endBasedOnReverse.position,         t);
        private void UnloadingLocalRotation(float t, RectTransform endBasedOnReverse)      => target.unload.localRotation    = Quaternion.LerpUnclamped(extreme.unloadStart.localRotation,    endBasedOnReverse.localRotation,    t);
        private void UnloadingGlobalRotation(float t, RectTransform endBasedOnReverse)     => target.unload.rotation         = Quaternion.LerpUnclamped(extreme.unloadStart.rotation,         endBasedOnReverse.rotation,         t);
        private void UnloadingScale(float t, RectTransform endBasedOnReverse)              => target.unload.localScale       = Vector3.LerpUnclamped   (extreme.unloadStart.localScale,       endBasedOnReverse.localScale,       t);
        private void UnloadingRectSizeDelta(float t, RectTransform endBasedOnReverse)      => target.unload.sizeDelta        = Vector2.LerpUnclamped   (extreme.unloadStart.sizeDelta,        endBasedOnReverse.sizeDelta,        t);
        private void UnloadingRectAnchorPosition(float t, RectTransform endBasedOnReverse) => target.unload.anchoredPosition = Vector2.LerpUnclamped   (extreme.unloadStart.anchoredPosition, endBasedOnReverse.anchoredPosition, t);
        private void UnloadingRectPivot(float t, RectTransform endBasedOnReverse)          => target.unload.pivot            = Vector2.LerpUnclamped   (extreme.unloadStart.pivot,            endBasedOnReverse.pivot,            t);
        private void UnloadingRectAnchorMin(float t, RectTransform endBasedOnReverse)      => target.unload.anchorMin        = Vector2.LerpUnclamped   (extreme.unloadStart.anchorMin,        endBasedOnReverse.anchorMin,        t);
        private void UnloadingRectAnchorMax(float t, RectTransform endBasedOnReverse)      => target.unload.anchorMax        = Vector2.LerpUnclamped   (extreme.unloadStart.anchorMax,        endBasedOnReverse.anchorMax,        t);
        // @formatter:on
    }
}