using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Omnix.Monos
{
    public class SelectablesArray : MonoBehaviour
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private KeyCode _deselectKeycode;
        [SerializeField] private Selectable[] _selectables;

        private void Reset()
        {
            _keyCode = KeyCode.Tab;
            _deselectKeycode = KeyCode.Escape;
            _selectables = GetComponentsInChildren<Selectable>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_keyCode))
            {
                bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                Select(isShift ? -1 : 1);
            }

            if (Input.GetKeyDown(_deselectKeycode))
            {
                Deselect();
            }
        }

        private static bool CanInteractWith(Selectable selectable)
        {
            return    selectable.enabled
                   && selectable.interactable
                   && selectable.targetGraphic.enabled
                   && selectable.gameObject.activeInHierarchy
                   && selectable.targetGraphic.gameObject.activeInHierarchy;
        }

        private int GetCurrentSelectedIndex()
        {
            GameObject currentObject = EventSystem.current.currentSelectedGameObject;
            if (currentObject == null) return -1;

            for (var i = 0; i < _selectables.Length; i++)
            {
                Selectable selectable = _selectables[i];
                if (selectable == null) continue;
                if (selectable.gameObject == currentObject) return i;
            }

            return -1;
        }

        /// <param name="direction"> +1 to select next, -1 to select previous. </param>
        private void Select(int direction)
        {
            int counting = 0;
            int max = _selectables.Length;
            
            int nextIndex = GetCurrentSelectedIndex();
            if (nextIndex <= -1)
            {
                _selectables[0].Select();
                return;
            } 
            
            Increment();
            while (CanInteractWith(_selectables[nextIndex]) == false)
            {
                Increment();
                counting++;

                if (counting > max)
                {
                    Debug.LogError("Unable to find selectable element");
                    return;
                }
            }

            _selectables[nextIndex].Select();
            return;

            void Increment() => nextIndex = (nextIndex + direction + max) % max;
        }
        
        private void Deselect()
        {
            int nextIndex = GetCurrentSelectedIndex();
            if (nextIndex <= -1) return;

            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}