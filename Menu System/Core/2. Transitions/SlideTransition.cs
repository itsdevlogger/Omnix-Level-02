using MenuManagement.Base;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MenuManagement.Transitions
{
    public class SlideTransition : BaseTransitionBlendable
    {
        [SerializeField] private RectTransform loadStartTrans;
        [SerializeField] private RectTransform unloadEndTrans;

        private Vector2 loadEnd;
        private Vector2 unloadStart;
        private RectTransform loadRect;
        private RectTransform unloadRect;
        private Transform loadStartDefParent;
        private Transform unloadEndDefParent;

        public override void Prepare(BaseMenu unload, BaseMenu load)
        {
            if (loadStartTrans != null) loadStartDefParent = loadStartTrans.parent;
            if (unloadEndTrans != null)  unloadEndDefParent = unloadEndTrans.parent;

            if (load)
            {
                loadStartTrans.SetParent(load.transform.parent);

                loadRect = load.GetComponent<RectTransform>();
                loadEnd = loadRect.anchoredPosition;
                loadRect.anchoredPosition = loadStartTrans.anchoredPosition;
            }

            if (unload)
            {
                unloadEndTrans.SetParent(unload.transform.parent);

                unloadRect = unload.GetComponent<RectTransform>();
                unloadStart = unloadRect.anchoredPosition;
            }
        }

        public override void Cleanup(BaseMenu unload, BaseMenu load)
        {
            base.Cleanup(unload, load);
            loadStartTrans.SetParent(loadStartDefParent);
            unloadEndTrans.SetParent(unloadEndDefParent);

            loadStartDefParent = null;
            unloadEndDefParent = null;
            loadRect = null;
            unloadRect = null;
        }


        [InspectorButton("Recreate load & unload points")]
        private void RecreateLoadUnload()
        {
            foreach (Transform child in transform)
            {
                Object.DestroyImmediate(child.gameObject);
            }

            Reset();
        }

        private void Reset()
        {
            loadStartTrans = new GameObject("Loading Start Point").AddComponent<RectTransform>();
            unloadEndTrans = new GameObject("Unload End Point").AddComponent<RectTransform>();

            loadStartTrans.SetParent(transform);
            unloadEndTrans.SetParent(transform);

            loadStartTrans.localPosition = Vector3.zero;
            unloadEndTrans.localPosition = Vector3.zero;

            loadStartTrans.gameObject.AddComponent<SlideTransitionPoint>().Init(this);
            unloadEndTrans.gameObject.AddComponent<SlideTransitionPoint>().Init(this);
        }

        public override void SetLoadingFrame(BaseMenu load, float t, bool playingInReversed)
        {
            Vector2 learpMinima = playingInReversed ? unloadEndTrans.anchoredPosition : loadStartTrans.anchoredPosition;
            loadRect.anchoredPosition = Vector2.LerpUnclamped(learpMinima, loadEnd, t);
        }

        public override void SetUnloadingFrame(BaseMenu unload, float t, bool playingInReversed)
        {
            Vector2 learpMaxima = playingInReversed ? loadStartTrans.anchoredPosition : unloadEndTrans.anchoredPosition;
            unloadRect.anchoredPosition = Vector2.LerpUnclamped(unloadStart, learpMaxima, t);
        }

        public void OnDrawGizmosSelected()
        {
            #if UNITY_EDITOR
            Vector3 middle = transform.position;

            Vector3 start = loadStartTrans.position;
            Handles.color = Color.green;
            Handles.DrawDottedLine(start, middle, 7f);

            Vector3 end = unloadEndTrans.position;
            Handles.color = Color.red;
            Handles.DrawDottedLine(middle, end, 7f);
            #endif
        }
    }
}