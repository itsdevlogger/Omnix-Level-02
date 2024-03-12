using System;

namespace MenuManagement.Editor
{
    public class Parser
    {
        private string[] _lines;
        private int _indent = 0;
        
        public string Current { get; private set; }
        public int CurrentNumber { get; private set; }
        
        public bool ReachedEnd { get; private set; } = false;
        private string _indentString = "";

        public string CurrentWithIndented => _indentString + Current;
        public int Indent
        {
            get => _indent;
            set
            {
                if (value <= 0)
                {
                    _indent = 0;
                    _indentString = "";
                }
                else
                {
                    _indent = value;
                    _indentString = new string(' ', value);
                }
            }
        }
        
        public Parser(string template)
        {
            _lines = template.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            CurrentNumber = -1;
            MoveNext();
        }

        public void MoveNext()
        {
            CurrentNumber++;

            if (CurrentNumber >= _lines.Length)
            {
                Current = string.Empty;
                ReachedEnd = true;
            }
            else
            {
                Current = _lines[CurrentNumber];
                ReachedEnd = false;
            }
        }
    }
}