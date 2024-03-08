using System;
using UnityEngine;

namespace Omnix.Monos
{
    [ExecuteAlways]
    public class ArrangeChildren : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum ArrangeMode
        {
            Position,
            Rotation,
            Scale
        }

        [SerializeField] private ArrangeMode actsOn = ArrangeMode.Position;
        [SerializeField] private bool arrangeEnabledOnly;
        [SerializeField] private ArrangeOnAxis X, Y, Z;

        private Action updateAction;

        public ArrangeMode ActsOn => actsOn;
        public bool ArrangeEnabledOnly => arrangeEnabledOnly;

        private void Start()
        {
            SetUpdateAction();
        }

        private void Update()
        {
            updateAction.Invoke();
        }

        private void SetUpdateAction()
        {
            if (this.actsOn == ArrangeMode.Position) updateAction = ActPosition;
            else if (this.actsOn == ArrangeMode.Rotation) updateAction = ActRotation;
            else if (this.actsOn == ArrangeMode.Scale) updateAction = ActScale;
        }

        private void ActPosition()
        {
            int cc = transform.childCount;
            int counter = 0;
            Vector3 current;
            foreach (Transform child in transform)
            {
                if (arrangeEnabledOnly && !child.gameObject.activeSelf) continue;
                current = child.localPosition;
                current.x = X.GetNextValue(counter, cc, current.x);
                current.y = Y.GetNextValue(counter, cc, current.y);
                current.z = Z.GetNextValue(counter, cc, current.z);
                child.localPosition = current;
                counter++;
            }
        }

        private void ActRotation()
        {
            int cc = transform.childCount;
            int counter = 0;
            Vector3 current;
            foreach (Transform child in transform)
            {
                if (arrangeEnabledOnly && !child.gameObject.activeSelf) continue;

                current = child.localRotation.eulerAngles;
                current.x = X.GetNextValue(counter, cc, current.x);
                current.y = Y.GetNextValue(counter, cc, current.y);
                current.z = Z.GetNextValue(counter, cc, current.z);
                child.localRotation = Quaternion.Euler(current);
                counter++;
            }
        }

        private void ActScale()
        {
            int cc = transform.childCount;
            int counter = 0;
            Vector3 current;
            foreach (Transform child in transform)
            {
                if (arrangeEnabledOnly && !child.gameObject.activeSelf) continue;

                current = child.localScale;
                current.x = X.GetNextValue(counter, cc, current.x);
                current.y = Y.GetNextValue(counter, cc, current.y);
                current.z = Z.GetNextValue(counter, cc, current.z);
                child.localScale = current;
                counter++;
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            SetUpdateAction();
        }
    }
    
    
    [Serializable]
    public class ArrangeOnAxis
    {
        public enum ArrangeMode
        {
            DontArrange,
            ZigZag,
            StartStep,
            StartEnd,
            CenterStep,
            Scatter
        }

        public ArrangeMode mode = ArrangeMode.DontArrange;
        public float val1;
        public float val2;
        public int alternate;

        public float GetNextValue(int counter, int totalCount, float defVal)
        {
            if (alternate > 1) counter /= alternate;

            if (mode == ArrangeMode.DontArrange) return defVal;
            if (mode == ArrangeMode.ZigZag) return ((((float)counter) % 2f) == 0f) ? val1 : val2;
            if (mode == ArrangeMode.StartStep) return val2 * counter + val1;
            if (mode == ArrangeMode.StartEnd) return ((val2 - val1) * counter / totalCount) + val1;
            if (mode == ArrangeMode.CenterStep) return val1 + (val2 * (counter + 0.5f - ((float)totalCount) / 2f));
            return defVal;
        }
    }
}