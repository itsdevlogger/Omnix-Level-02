using System.Text.RegularExpressions;

namespace MenuManagement.Editor
{
    public static class Regs
    {
        public static readonly Regex START_READING = new Regex(@"^//\s+\@cst_start");
        public static readonly Regex STOP_READING = new Regex(@"^//\s+\@cst_stop");
        public static readonly Regex START_IF = new Regex(@"^\s*#if\s+cst_([a-zA-Z_][a-zA-Z0-9_]*)");
        public static readonly Regex STOP_IF = new Regex(@"^\s*#endif");
        public static readonly Regex PLACEHOLDER = new Regex(@"(?:\/\*\s*)?CST_([a-zA-Z_][a-zA-Z0-9_]*)(?:\s*\*\/)?");
        public static readonly Regex INDENT = new Regex(@"^\/\/\s*\@cst_indent\s*(\+\=|\-\=|\=|\*\=|\/\=)\s*(\d+)");
    }
}