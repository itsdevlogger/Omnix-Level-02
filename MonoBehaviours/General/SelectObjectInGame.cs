#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Omnix.Monos
{
    public class SelectObjectInGame : MonoBehaviour
    {
#if UNITY_EDITOR
        private static SelectObjectInGame Instance;
        [SerializeField, Range(0, 2)] private int mouseButton = 2;
        [SerializeField] private KeyCode keyboardButton = KeyCode.None;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (!Input.GetMouseButtonDown(mouseButton)) return;
            if (keyboardButton != KeyCode.None && !Input.GetKey(keyboardButton)) return;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
                Selection.activeTransform = hitInfo.transform;
        }
#endif
    }


}