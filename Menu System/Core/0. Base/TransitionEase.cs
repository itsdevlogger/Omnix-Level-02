using System;
using UnityEngine;

namespace MenuManagement.Base
{
    [Serializable]
    public class TransitionEase
    {
        [SerializeField] private EaseMaster.Kind ease;
        [SerializeField] private AnimationCurve curve;
        public EaseMaster.Function Value;

        public void Init()
        {
            if (ease == EaseMaster.Kind.Custom)
            {
                Value = (elapsed, duration) => curve.Evaluate(elapsed / duration);
            }
            else
            {
                Value = EaseMaster.GetFunction(ease);
            }
        }
    }
}