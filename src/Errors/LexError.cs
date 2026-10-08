namespace miniLang;

public class LexError : Exception
{
    public int Line { get; }

    public LexError(string message, int line) : base($"[line {line}] {message}")
    {
        Line = line;
    }
}
