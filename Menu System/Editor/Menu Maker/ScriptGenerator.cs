using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MenuManagement.Editor
{
    public class ScriptGenerator
    {
        public readonly HashSet<string> compileVariables;
        public readonly Dictionary<string, string> placeholderValues;

        private Parser _parser;
        private StringBuilder _lines;

        public ScriptGenerator()
        {
            this.compileVariables = new HashSet<string>();
            this.placeholderValues = new Dictionary<string, string>();
        }

        private string ReplacePlaceholders(string template)
        {
            return Regs.PLACEHOLDER.Replace(template, match =>
            {
                string key = match.Groups[1].Value;
                return placeholderValues.TryGetValue(key, out string value) ? value : match.Value;
            });
        }

        public string Process(string template)
        {
            _parser = new Parser(ReplacePlaceholders(template));
            _lines = new StringBuilder();
            while (_parser.ReachedEnd == false)
            {
                HandleAll();
            }

            return _lines.ToString();
        }

        private void HandleAll()
        {
            Match indentMatch = Regs.INDENT.Match(_parser.Current);
            if (indentMatch.Success)
            {
                int value = int.Parse(indentMatch.Groups[2].Value);
                // @formatter:off
                switch (indentMatch.Groups[1].Value)
                {
                    case "+=": _parser.Indent += value; _parser.MoveNext(); return;
                    case "-=": _parser.Indent -= value; _parser.MoveNext(); return;
                    case "*=": _parser.Indent *= value; _parser.MoveNext(); return;
                    case "/=": _parser.Indent /= value; _parser.MoveNext(); return;
                    case "=": _parser.Indent += value; _parser.MoveNext(); return;
                }
                // @formatter:on
            }

            if (Regs.STOP_READING.IsMatch(_parser.Current))
            {
                SkipTill(Regs.START_READING, "Start Reading");
                _parser.MoveNext();
                return;
            }

            Match ifMatch = Regs.START_IF.Match(_parser.Current);
            if (ifMatch.Success)
            {
                if (!compileVariables.Contains(ifMatch.Groups[1].Value))
                {
                    SkipTill(Regs.STOP_IF, "End If");
                }

                _parser.MoveNext();
                return;
            }

            if (Regs.STOP_IF.IsMatch(_parser.Current))
            {
                _parser.MoveNext();
                return;
            }

            _lines.AppendLine(_parser.CurrentWithIndented);
            _parser.MoveNext();
        }

        private void SkipTill(Regex regex, string errorCode)
        {
            while (_parser.ReachedEnd == false)
            {
                _parser.MoveNext();
                if (regex.IsMatch(_parser.Current)) return;
            }

            Debug.LogError("Reached end of document. Expected token: " + errorCode);
        }
    }
}