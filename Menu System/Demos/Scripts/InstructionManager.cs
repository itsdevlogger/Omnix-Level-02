using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MenuManagement.Demos
{
    public class InstructionManager : MonoBehaviour
    {
        [Serializable]
        public class Instruction
        {
            [TextArea] public string text;
            public AudioClip clip;
            public bool autoLoadNext;
            public float autoLoadDelay;
            public UnityEvent onStart;
        }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button stopButton;
        [SerializeField] private AudioSource source;
        [SerializeField] private Instruction[] instructions;
        private int currentIndex = -1;

        private void Start()
        {
            nextButton.onClick.AddListener(LoadNext);
            prevButton.onClick.AddListener(LoadPrev);
            stopButton.onClick.AddListener(() => source.Stop());
            LoadNext();
        }

        private void Refresh()
        {
            Instruction item = instructions[currentIndex];
            text.text = item.text;
            source.clip = item.clip;
            source.Play();
            item.onStart?.Invoke();
        }
        
        private void LoadPrev()
        {
            currentIndex--;
            if (currentIndex <= 0) currentIndex = 0;
            Refresh();
        }

        private void LoadNext()
        {
            currentIndex++;
            if (currentIndex >= instructions.Length) currentIndex = 0;
            Refresh();
        }
    }
}