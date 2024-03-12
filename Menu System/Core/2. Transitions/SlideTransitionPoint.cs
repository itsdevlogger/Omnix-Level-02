using UnityEngine;

namespace MenuManagement.Transitions
{
    public class SlideTransitionPoint : MonoBehaviour
    {
        [SerializeField] private SlideTransition parent;

        public void Init(SlideTransition parent)
        {
            this.parent = parent;
        }

        private void OnDrawGizmosSelected()
        {
            parent.OnDrawGizmosSelected();
        }
    }
}